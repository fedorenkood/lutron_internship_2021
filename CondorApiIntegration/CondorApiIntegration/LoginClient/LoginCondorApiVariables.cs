namespace Lutron.CondorApiIntegration.LoginClient
{
    public class LoginCondorApiVariables : CondorApiVariablesBase
    {
        public LoginCondorApiVariables(string userManagementToken) : base(userManagementToken)
        {
            InstantiateConstants();
        }

        // For testing purposes only
        public LoginCondorApiVariables(string userManagementToken, string baseUrl) : this(userManagementToken)
        {
            this.baseUrl = baseUrl;
        }

        protected override void InstantiateConstants()
        {
            baseUrl = "https://models.dev.data.lutron.io/";
            modelExtension = "v1/login";
            region = "us-east-1";
            logType = "Login";
            method = RestSharp.Method.POST;
            cacheKey = "login_client_cache";
        }
    }
}
