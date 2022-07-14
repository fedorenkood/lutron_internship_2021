using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using Lutron.CondorApiIntegration.LoginClient;
using System.Collections.Generic;

namespace Lutron.CondorApiIntegration
{
    public static class CondorApiClientProvider
    {

        public static AreaTypePredictionCondorApiClient GetAreaTypePredictionClient(string token, string baseUrl)
        {
            AreaTypePredictionCondorApiVariables apiConstants = new AreaTypePredictionCondorApiVariables(token, baseUrl);
            return new AreaTypePredictionCondorApiClient(apiConstants);
        }

        public static AreaTypePredictionCondorApiClient GetAreaTypePredictionClient(string token)
        {
            AreaTypePredictionCondorApiVariables apiConstants = new AreaTypePredictionCondorApiVariables(token);
            return new AreaTypePredictionCondorApiClient(apiConstants);
        }


        public static LoginCondorApiClient GetLoginClient(string token, string baseUrl)
        {
            LoginCondorApiVariables apiConstants = new LoginCondorApiVariables(token, baseUrl);
            return new LoginCondorApiClient(apiConstants);
        }

        public static LoginCondorApiClient GetLoginClient(string token)
        {
            LoginCondorApiVariables apiConstants = new LoginCondorApiVariables(token);
            return new LoginCondorApiClient(apiConstants);
        }
    }
}
