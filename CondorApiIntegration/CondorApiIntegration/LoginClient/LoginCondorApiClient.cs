
using Lutron.Gulliver.Infrastructure.LoggingFramework;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Lutron.CondorApiIntegration.LoginClient
{
    public class LoginCondorApiClient : CondorApiClient<LoginData, LoginResult>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDatum"></param>
        /// <param name="apiConstants"></param>
        public LoginCondorApiClient(LoginCondorApiVariables apiConstants) : base(apiConstants)
        {

        }

        /// <summary>
        /// Parses response and outputs LoginResult class
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected override List<LoginResult> ParseResponse(IRestResponse response)
        {
            /* Format: { "aws_api_key": "somevalue",
               "aws_access_key_id": "somevalue",
               "aws_secret_access_key": "somevalue" }*/
            try
            {
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                var allConstants = (responseDict.ContainsKey("aws_access_key_id"))
                                && (responseDict.ContainsKey("aws_secret_access_key"))
                                && (responseDict.ContainsKey("aws_api_key"));
                if (allConstants)
                {
                    return new List<LoginResult> { new LoginResult(responseDict) };
                }
                OutputLog("Missing secrets", LogExceptionType.Error, 0);
                return new List<LoginResult>();
            } catch (Exception ex)
            {
                OutputLog(ex.Message, LogExceptionType.Error, 0);
                return new List<LoginResult>();
            }
        }

        protected override void AddAuthentificator(IRestClient restClient, RestRequest request, bool ResetSecrets)
        {
            // Does not need an authentificator
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetBody()
        {
            string body = "{}";
            return body;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>
                {
                    { "x-auth-token", apiVariables.UserManagementToken }
                };
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

