using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace com.ketra.APITools
{
    /// <summary>
    /// Endpoint handling requests
    /// </summary>
    public enum Endpoint
    {
        LutronDSDev,
        LutronDSProduction,
    }

    /// <summary>
    /// API client helper to assist in making requests to a particular endpoint
    /// </summary>
    public class ApiClientHelper
    {
        #region Private members
        IRestClient m_restClient;
        IAuthenticator m_authenticator;
        string m_userName;
        string m_password;
        #endregion Private members
        #region Properties
        public string BaseUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint">
        /// Endpoint to which requests are sent.
        /// </param>
        public ApiClientHelper(Endpoint endpoint)
        {
            switch (endpoint)
            {
                case Endpoint.LutronDSDev:
                    BaseUrl = @"https://models.dev.data.lutron.io/";
                    AccessKey = @"AKIAUVH2K7CET3ZMOZPY";
                    SecretKey = @"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";
                    m_authenticator = new AwsAuthenticator(AccessKey, SecretKey, "us-east-1");
                    break;
                case Endpoint.LutronDSProduction:
                    var secretsMgrHelper = new AwsSecretsManagerHelper();
                    Tuple<string, string> keys = secretsMgrHelper.GetKeys(
                                                        "arn:aws:secretsmanager:us-east-1" +
                                                        ":538420592509:secret" +
                                                        ":CondorModelRunnerUsercreden-YgSmQfFK2K0F-AO5Man");
                    BaseUrl = @"https://models.data.lutron.io/";
                    AccessKey = keys.Item1;
                    SecretKey = keys.Item2;
                    m_authenticator = new AwsAuthenticator(AccessKey, SecretKey, "us-east-1");
                    break;
                default:
                    BaseUrl = @"";
                    AccessKey = @"";
                    SecretKey = @"";
                    break;
            }
            m_restClient = new RestClient(BaseUrl);
            m_restClient.Authenticator = m_authenticator;
        }
        #endregion Constructors

        #region Public Methods
        public async Task<IRestResponse> ExecuteRestRequestAsync(Method method, Dictionary<string,string> header, Tuple<string, object, ParameterType>[] parameters, string resource, CancellationToken token)
        {
            var request = new RestRequest(method);

            foreach (var param in parameters)
            {
                request.AddParameter(param.Item1, param.Item2, param.Item3);
            }
            request.AddHeaders(header);
            request.Resource = resource;
            return await m_restClient.ExecuteAsync(request, token);
        }
        #endregion Public Methods
    }
}
