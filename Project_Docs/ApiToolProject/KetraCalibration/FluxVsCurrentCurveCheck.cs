using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace com.ketra.APITools.KetraCalibration
{
    public class FluxVsCurrentCurveCheck : KetraCalibrationCondorChecksBase
    {
        private static readonly string LOG_FILE = $"CondorFluxCheck_{DateTime.Now.ToString("MMM_yyyy")}.csv";
        public string DeviceType { get; }
        public string StationID { get; }
        public string CalTemp { get; }
        public string BeamAngle { get; }
        public DateTime SessionTime { get; }
        public FluxCheckDatum[] Data { get; }

        public FluxVsCurrentCurveCheck(
            string devType,
            string stationID,
            string serialNumber,
            string sessionID,
            string calTemp,
            string beamAngle,
            DateTime sessionTime,
            FluxCheckDatum[] data,
            Endpoint endpoint = Endpoint.LutronDSProduction) : base(LOG_FILE, serialNumber, sessionID, endpoint)
        {
            DeviceType = devType;
            StationID = stationID;
            CalTemp = calTemp;
            BeamAngle = beamAngle;
            SessionTime = sessionTime;
            Data = data;
        }

        public async Task<Tuple<CheckStatus, string>> CheckData()
        {
            Tuple<string, object, RestSharp.ParameterType>[] parameters = GetParameters();
            Dictionary<string, string> header = GetHeaders();
            string resourceString = GetResource();
            IRestResponse response = await base.ExecuteRestRequestAsync(
                                                    Method.POST,
                                                    header,
                                                    parameters,
                                                    resourceString);
            if (response == null)
            {
                return new Tuple<CheckStatus, string>(CheckStatus.ServerError, $"Request failed after {MAX_ATTEMPTS} attempts.");
            }
            var results = ParseResponse(response);
            if (results.Any(res => !res.FluxResult || (res.PhosphorFluxResult.HasValue && !res.PhosphorFluxResult.Value)))
            {
                var failures = results.Where(res => !res.FluxResult || (res.PhosphorFluxResult.HasValue && !res.PhosphorFluxResult.Value)).ToArray();
                var failString = String.Join("; ", failures.Select(f => $"C:{f.Color}"));
                Log(failString, "Flux Check", 0);
                return new Tuple<CheckStatus, string>(CheckStatus.Fail, failString);
            }
            else
            {
                Log("Success", "Flux Check", 0);
                return new Tuple<CheckStatus, string>(CheckStatus.Pass, String.Empty);
            }
        }

        private FluxCheckResult[] ParseResponse(IRestResponse response)
        {
            var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            var results = (Newtonsoft.Json.Linq.JArray)responseDict["result"];
            var stringArrayResults = results.Select(res => res.ToObject<string[]>()).ToArray();
            return stringArrayResults
                        .Select(res => new FluxCheckResult(
                                            res[0],
                                            res[1],
                                            res[2],
                                            res[3],
                                            res[4],
                                            res[5],
                                            res[6],
                                            res[7],
                                            res[8],
                                            res[12],
                                            res[9],
                                            res[13]))
                        .ToArray();
        }

        private Tuple<string, object, RestSharp.ParameterType>[] GetParameters()
        {
            var parameters = new List<Tuple<string, object, RestSharp.ParameterType>>();
            parameters.Add(new Tuple<string, object, ParameterType>("application/json", GetBody(), ParameterType.RequestBody));
            return parameters.ToArray();
        }

        private Dictionary<string, string> GetHeaders()
        {
            switch (Endpoint)
            {
                case Endpoint.LutronDSProduction:
                    return new Dictionary<string, string>
                    {
                        { "x-api-key", "xuAAVWGDxt6q1QwQH2pba1oH7wNgCfBa8DBn7MG1" }
                    };
                case Endpoint.LutronDSDev:
                    return new Dictionary<string, string>
                    {
                        { "x-api-key", "XItmyYcr4DrvmZaaQGFZ4TNCOQqO96Q7b2mdrFc7" }
                    };
                default:
                    throw new Exception($"Unexpected endpoint value: {Endpoint}.");
            }
        }

        private string GetResource()
        {
            return "v1/run/ketra-flux-outlier-detection/1.1.0";
        }

        private string GetBody()
        {
            return $"{{\n\"data\": [{GetFormattedData()}]\n}}";
        }

        private string GetFormattedData()
        {
            List<string> dataStrings = new List<string>();
            foreach (var datum in Data)
            {
                dataStrings.Add(
                    $"[\"{DeviceType}\",\n" +
                    $"\"{StationID}\",\n" +
                    $"\"{SerialNumber}\",\n" +
                    $"\"{SessionID}\",\n" +
                    $"\"{CalTemp}\",\n" +
                    $"\"{datum.Color}\",\n" +
                    $"{datum.CurrentCode},\n" +
                    $"{datum.Current},\n" +
                    $"{datum.Flux},\n" +
                    $"{datum.PhosphorFlux},\n" +
                    $"\"{BeamAngle}\",\n" +
                    $"\"{SessionTime.ToString("MM/dd/yy HH:mm")}\"]");
            }

            return String.Join(",\n", dataStrings);
        }
    }

    public class FluxCheckDatum
    {
        public string Color { get; }
        public int CurrentCode { get; }
        public double Current { get; }
        public double Flux { get; }
        public double PhosphorFlux { get; }

        public FluxCheckDatum(
            string color,
            int currentCode,
            double current,
            double flux,
            double phosphorFlux)
        {
            Color = color;
            CurrentCode = currentCode;
            Current = current;
            Flux = flux;
            PhosphorFlux = phosphorFlux;
        }
    }

    public class FluxCheckResult
    {
        public string DeviceType { get; }
        public string StationID { get; }
        public string SerialNumber { get; }
        public string SessionID { get; }
        public string CalTemp { get; }
        public string Color { get; }
        public string BeamAngle { get; }
        public DateTime SessionTime { get; }
        public bool FluxResult { get; }
        public Tuple<double, double> FluxPCACoordinates { get; }
        public bool? PhosphorFluxResult { get; }
        public Tuple<double, double> PhosphorFluxPCACoordinates { get; }

        public FluxCheckResult(
            string devType,
            string stationID,
            string sn,
            string sessionID,
            string calTemp,
            string color,
            string beamAngle,
            string sessionTimeString,
            string fluxResultString,
            string fluxPcaString,
            string phosphorFluxResultString,
            string phosphorFluxPcaString)
        {
            DeviceType = devType;
            StationID = stationID;
            SerialNumber = sn;
            SessionID = sessionID;
            CalTemp = calTemp;
            Color = color;
            BeamAngle = beamAngle;
            SessionTime = DateTime.Parse(sessionTimeString);
            FluxResult = bool.Parse(fluxResultString);
            var fluxPcaMatch = Regex.Match(fluxPcaString, @"\[(.+), (.+)\]");
            FluxPCACoordinates =
                new Tuple<double, double>(
                    double.Parse(fluxPcaMatch.Groups[1].Value),
                    double.Parse(fluxPcaMatch.Groups[2].Value));

            if (!String.IsNullOrWhiteSpace(phosphorFluxResultString))
            {
                PhosphorFluxResult = bool.Parse(phosphorFluxResultString);
                var phosphorFluxPcaMatch = Regex.Match(phosphorFluxPcaString, @"\[(.+), (.+)\]");
                PhosphorFluxPCACoordinates =
                    new Tuple<double, double>(
                        double.Parse(phosphorFluxPcaMatch.Groups[1].Value),
                        double.Parse(phosphorFluxPcaMatch.Groups[2].Value));
            }
            else
            {
                PhosphorFluxResult = null;
                PhosphorFluxPCACoordinates = null;
            }
        }
    }
}
