using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Lutron.Gulliver.Infrastructure.LoggingFramework;
using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using RestSharp.Authenticators;
using System.Net;

namespace Lutron.CondorApiIntegration
{
    /// <summary>
    /// A generic class for a Condor Api integration
    /// Has to be implemented with implementation of generics
    /// D stands for datum class
    /// R stands for response
    /// </summary>
    public abstract class CondorApiClient<D, R>
    {

        #region Protected fields
        protected readonly int MAX_ATTEMPTS = 3;
        protected CondorApiVariablesBase apiVariables;
        /// <summary>
        /// List of Datums that are used to populate the request
        /// </summary>
        protected List<D> dataList;
        #endregion 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiVariables"></param>
        protected CondorApiClient(CondorApiVariablesBase apiVariables)
        {
            this.apiVariables = apiVariables;
        }

        #region Request Execution Methods
        /// <summary>
        /// Main call to the api after class is created
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Tuple<ResponseResult, List<R>>> ExecuteRequest(List<D> dataList)
        {
            this.dataList = dataList;
            var message = "Failure";
            ResponseResult responseResult = ResponseResult.Fail;
            LogExceptionType logExceptionType = LogExceptionType.Error;
            List<R> results = null;
            try
            {
                IRestResponse response = await ExecuteRestRequestAsync();
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        results = ParseResponse(response);
                        if (results.Count >= 1)
                        {
                            message = "Success";
                            logExceptionType = LogExceptionType.Information;
                            responseResult = ResponseResult.Success;
                        }
                        else
                        {
                            responseResult = ResponseResult.MalformedResponse;
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        responseResult = ResponseResult.BadRequest;
                        break;
                    case HttpStatusCode.Unauthorized:
                        responseResult = ResponseResult.Unauthorized;
                        break;
                    case HttpStatusCode.NotFound:
                        responseResult = ResponseResult.NotFound;
                        break;
                    case HttpStatusCode.ServiceUnavailable:
                        responseResult = ResponseResult.ServiceUnavailable;
                        break;
                    case HttpStatusCode.InternalServerError:
                        responseResult = ResponseResult.ServerError;
                        break;
                }
            } catch (Exception ex)
            {
                message = ex.Message;
            }
            OutputLog(message, logExceptionType, 0);
            return new Tuple<ResponseResult, List<R>>(responseResult, results);
        }

        /// <summary>
        /// Executes request up to MAX_ATTEMPTS times
        /// If fails, resets secrets and makes new secrets request
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IRestResponse> ExecuteRestRequestAsync()
        {
            int attempt = 1;
            IRestResponse response = null;
            while (attempt <= MAX_ATTEMPTS)
            {
                IRestClient restClient = new RestClient(apiVariables.BaseUrl);
                RestRequest request = new RestRequest(apiVariables.Method);
                // Reset keys if previous attempt failed
                AddAuthentificator(restClient, request, attempt > 1);
                AddRequestOptions(request);
                CancellationToken cancelToken = new CancellationToken();
                response = await restClient.ExecuteAsync(request, cancelToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LogRequestFailure(response, attempt);
                    attempt++;
                }
                else
                {
                    LogRequestSuccess(attempt);
                    break;
                }
            }
            return response;
        }

        /// <summary>
        /// Instantiates a client and a request 
        /// </summary>
        /// <param name="request"></param>
        protected virtual void AddRequestOptions(RestRequest request)
        {
            foreach (var param in GetParameters())
            {
                request.AddParameter(param.Item1, param.Item2, param.Item3);
            }
            request.AddHeaders(GetHeaders());
            request.Resource = GetResource();
        }


        /// <summary>
        /// Requests secrets using AwsSecretsManagerHelper
        /// Gives an option for child classes to override the Authentificator addition
        /// in case they use a different authentificator or don't need one
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="request"></param>
        /// <param name="ResetSecrets"></param>
        protected virtual void AddAuthentificator(IRestClient restClient, RestRequest request, bool ResetSecrets)
        {
            var secretsManagerHelper = new AwsSecretsManagerHelper();
            var response = secretsManagerHelper.GetSecretsAsync(apiVariables, apiVariables.CacheKey, ResetSecrets);
            response.Wait();
            var secrets = response.Result;
            if (secrets != null)
            {
                var accessKey = secrets["aws_access_key_id"];
                var secretKey = secrets["aws_secret_access_key"];
                request.AddHeader("x-api-key", secrets["aws_api_key"]);
                IAuthenticator authenticator = new AwsAuthenticator(accessKey, secretKey, apiVariables.Region);
                restClient.Authenticator = authenticator;
            }
        }
        #endregion

        #region Logging

        /// <summary>
        /// Logs failure
        /// </summary>
        /// <param name="response"></param>
        /// <param name="attempt"></param>
        protected virtual void LogRequestFailure(
            IRestResponse response,
            int attempt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rest Request Failure. ");
            sb.Append($"Status Code: {response.StatusCode}; ");
            if (!String.IsNullOrEmpty(response.ErrorMessage))
            {
                sb.Append($"Error Message: {response.ErrorMessage}");
            }
            else
            {
                sb.Append($"Error Message: None");
            }

            if (response.ErrorException != null)
            {
                sb.Append($"Error Exception Message: {response.ErrorException.Message}");
            }

            this.OutputLog(sb.ToString(), LogExceptionType.Error, attempt);
        }

        /// <summary>
        /// Logs success
        /// </summary>
        /// <param name="attempt"></param>
        protected virtual void LogRequestSuccess(
            int attempt)
        {
            this.OutputLog("Success", LogExceptionType.Information,  attempt);
        }

        /// <summary>
        /// Outputs the log using the Logging Framework
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        /// <param name="attempt"></param>
        protected virtual void OutputLog(
            string message,
            LogExceptionType logType,
            int attempt)
        {
            var finalMessage = String.Join(
                    "CondorApi: ",
                    attempt.ToString(), " ",
                    apiVariables.LogType, " Request ", message,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Log.WriteDebugEntry(finalMessage, logType);
        }
        #endregion 

        #region Private/Protected Methods
        protected abstract List<R> ParseResponse(IRestResponse response);

        protected abstract string GetBody();

        protected abstract Dictionary<string, string> GetHeaders();

        protected abstract Tuple<string, object, ParameterType>[] GetParameters();

        protected abstract string GetResource();

        #endregion Private/Protected Methods

    }
}
