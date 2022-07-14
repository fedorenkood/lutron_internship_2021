using Lutron.Gulliver.Infrastructure.LoggingFramework;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lutron.CondorApiIntegration.AreaTypePredictionClient
{
    /// <summary>
    /// 
    /// </summary>
    public class AreaTypePredictionCondorApiClient : CondorApiClient<AreaTypePredictionData, AreaTypePredictionResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaDatum"></param>
        /// <param name="endpoint"></param>
        public AreaTypePredictionCondorApiClient(AreaTypePredictionCondorApiVariables apiConstants) : base(apiConstants)
        {

        }

        /// <summary>
        /// Outputs a list of AreaTypePredictionResult for each AreaTypePredictionDatum included in the request
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected override List<AreaTypePredictionResult> ParseResponse(IRestResponse response)
        {
            try
            {
                // Format {"result": [["Bedroom", "Living Room", "Kitchen", "type1", "type2"]]}
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                // parse results
                var results = (Newtonsoft.Json.Linq.JArray)responseDict["result"];
                var stringArrayResults = results.Select(res => res.ToObject<string[]>()).ToArray();
                // parse data 
                var data = (Newtonsoft.Json.Linq.JArray)responseDict["data"];
                var stringArrayData = data.Select(res => res.ToObject<string[]>()).ToArray();
                // construct AreaTypePredictionResult array
                var areaTypePredictionResults = new List<AreaTypePredictionResult>();
                for (int i = 0; i < stringArrayResults.Length; i++)
                {
                    areaTypePredictionResults.Add(new AreaTypePredictionResult(stringArrayData[i][0], stringArrayResults[i].ToList()));
                }
                return areaTypePredictionResults;
            } catch (Exception ex)
            {
                OutputLog(ex.Message, LogExceptionType.Error, 0);
                return new List<AreaTypePredictionResult> ();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetBody()
        {
            string body = "{\n\"data\":\n[" + GetDataBody() + "]\n}";
            return body;
        }

        /// <summary>
        /// Creates a list of lists with area names
        /// </summary>
        /// <returns></returns>
        private string GetDataBody()
        {
            // Format? {"data": [["dylans room"], ["dylans room"]]}
            List<string> dataStrings = new List<string>();
            foreach (var datum in dataList)
            {
                dataStrings.Add(
                       $"[\"{datum.Name}\"]"
                       );
            }

            return String.Join(",\n", dataStrings);
        }

        /// <summary>
        /// This api does not requeire any headers
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string> { };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Tuple<string, object, ParameterType>[] GetParameters()
        {
            List<Tuple<string, object, RestSharp.ParameterType>> parameters = new List<Tuple<string, object, ParameterType>>();
            parameters.Add(new Tuple<string, object, ParameterType>("application/json", GetBody(), ParameterType.RequestBody));
            return parameters.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetResource()
        {
            return base.apiVariables.ModelExtension;

        }
    }

}
