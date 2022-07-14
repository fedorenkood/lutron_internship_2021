using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace com.ketra.APITools.KetraCalibration
{
    public class ChromaticityCheck : KetraCalibrationCondorChecksBase
    {
        private static readonly string LOG_FILE = $"CondorChromaCheckLog_{DateTime.Now.ToString("MMM_yyyy")}.csv";
        public string DeviceType { get; }
        public string StationID { get; }
        public string CalTemp { get; }
        public DateTime SessionTime { get; }
        public ChromaticityCheckDatum[] Data { get; }

        public ChromaticityCheck(
            string deviceType,
            string stationID,
            string serialNumber,
            string sessionID,
            string calTemp,
            DateTime sessionTime,
            ChromaticityCheckDatum[] chromaticityData,
            Endpoint endpoint = Endpoint.LutronDSProduction) : base(LOG_FILE, serialNumber, sessionID, endpoint)
        {
            DeviceType = deviceType;
            StationID = stationID;
            CalTemp = calTemp;
            SessionTime = sessionTime;
            Data = chromaticityData;
        }

        public async Task<Tuple<CheckStatus, string>> CheckData()
        {
            Tuple<string, object, RestSharp.ParameterType>[] parameters = GetParameters();
            Dictionary<string, string> header = GetHeader();
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
            ChromaticityCheckResult[] results = ParseResponse(response);
            if (results.Any(res => !res.XYStatus || (res.PhosphorStatus.HasValue && !res.PhosphorStatus.Value)))
            {
                var failures = results.Where(res => !res.XYStatus || (res.PhosphorStatus.HasValue && !res.PhosphorStatus.Value)).ToArray();
                var failString = String.Join("; ", failures.Select(f => $"C:{f.Color}, IC:{f.CurrentCode}"));
                Log(failString, "Chromaticity Check", 0);
                return new Tuple<CheckStatus, string>(CheckStatus.Fail, failString);
            }
            else
            {
                Log("Success", "Chromaticity Check", 0);
                return new Tuple<CheckStatus, string>(CheckStatus.Pass, String.Empty);
            }
        }

        private ChromaticityCheckResult[] ParseResponse(IRestResponse response)
        {
            var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            var results = (Newtonsoft.Json.Linq.JArray)responseDict["result"];
            var stringArrayResults = results.Select(res => res.ToObject<string[]>()).ToArray();
            return stringArrayResults
                        .Select(res => new ChromaticityCheckResult(
                                            res[0],
                                            res[1],
                                            res[2],
                                            res[3],
                                            res[4], 
                                            res[5],
                                            res[6], 
                                            res[7],
                                            res[8],
                                            res[9],
                                            res[10]))
                        .ToArray();
        }

        private string GetBody()
        {
            string body = "{\n\"data\":\n[" + GetDataBody() + "]\n}";
            return body;
        }

        private string GetDataBody()
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
                       $"{datum.X},\n" +
                       $"{datum.Y},\n" +
                       $"{datum.PhosphorX},\n" +
                       $"{datum.PhosphorY},\n" +
                       $"\"{SessionTime.ToString("MM/dd/yy HH:mm")}\"]");
            }

            return String.Join(",\n", dataStrings);
        }

        private Dictionary<string, string> GetHeader()
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

        private Tuple<string, object, ParameterType>[] GetParameters()
        {
            List<Tuple<string, object, RestSharp.ParameterType>> parameters = new List<Tuple<string, object, ParameterType>>();
            parameters.Add(new Tuple<string, object, ParameterType>("application/json", GetBody(), ParameterType.RequestBody));
            return parameters.ToArray();
        }

        private string GetResource()
        {
            return "v1/run/ketra-chromaticity-outlier-detection/1.0.2";

        }
    }


    public class ChromaticityCheckDatum
    {
        public string Color { get; }
        public int CurrentCode { get; }
        public double Current { get; }
        public double X { get; }
        public double Y { get; }
        public double PhosphorX { get; }
        public double PhosphorY { get; }

        public ChromaticityCheckDatum(
            string color,
            int currentCode,
            double current,
            double x,
            double y,
            double phosphorX,
            double phosphorY)
        {
            Color = color;
            CurrentCode = currentCode;
            Current = current;
            X = x;
            Y = y;
            PhosphorX = phosphorX;
            PhosphorY = phosphorY;
        }
    }

    public class ChromaticityCheckResult
    {
        public string DeviceType { get; }
        public string StationID { get; }
        public string SerialNumber { get; }
        public string SessionID { get; }
        public string CalTemp { get; }
        public string Color { get; }
        public UInt16 CurrentCode { get; }
        public double Current { get; }
        public bool XYStatus { get; }
        public bool? PhosphorStatus { get; }
        public DateTime SessionTime { get; }

        public ChromaticityCheckResult(
            string deviceType,
            string stationID,
            string serialNumber,
            string sessionID,
            string calTemp,
            string color,
            string currentCodeString,
            string currentString,
            string xyStatusString,
            string phosphorStatusString,
            string sessionTimeString)
        {
            DeviceType = deviceType;
            StationID = stationID;
            SerialNumber = serialNumber;
            SessionID = sessionID;
            CalTemp = calTemp;
            Color = color;
            CurrentCode = UInt16.Parse(currentCodeString);
            Current = Double.Parse(currentString);
            XYStatus = Boolean.Parse(xyStatusString);
            if (String.IsNullOrWhiteSpace(phosphorStatusString))
            {
                PhosphorStatus = null;
            }
            else
            {
                PhosphorStatus = Boolean.Parse(phosphorStatusString);
            }
            SessionTime = DateTime.Parse(sessionTimeString);
        }
    }
}
