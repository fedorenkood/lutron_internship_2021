namespace Lutron.CondorApiIntegration.AreaTypePredictionClient
{
    public class AreaTypePredictionCondorApiVariables : CondorApiVariablesBase
    {
        public AreaTypePredictionCondorApiVariables(string userManagementToken) : base(userManagementToken)
        {
            InstantiateConstants();
        }

        // For testing purposes only
        public AreaTypePredictionCondorApiVariables(string userManagementToken, string baseUrl) : this(userManagementToken)
        {
            this.baseUrl = baseUrl;
        }
        protected override void InstantiateConstants()
        {
            baseUrl = "https://models.dev.data.lutron.io/";
            modelExtension = "v1/run/roomtype-prediction-residential/1.0.1";
            region = "us-east-1";
            logType = "AreaType Prediction";
            method = RestSharp.Method.POST;
            cacheKey = "area_type_client_cache";
        }
    }
}
