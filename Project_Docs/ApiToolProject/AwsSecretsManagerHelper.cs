using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SecretsManager.Extensions.Caching;
using Amazon.Runtime;

namespace com.ketra.APITools
{
    public class AwsSecretsManagerHelper : IDisposable
    {
        AmazonSecretsManagerClient m_secretsMngrClient;
        SecretsManagerCache m_cache;

        public AwsSecretsManagerHelper()
        {
            m_secretsMngrClient = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);
            m_cache = new SecretsManagerCache(m_secretsMngrClient);
        }

        public void Dispose()
        {
            m_secretsMngrClient.Dispose();
            m_cache.Dispose();
        }

        public Tuple<string, string> GetKeys(
            string secretID)
        {
            var request = new GetSecretValueRequest()
            {
                SecretId = secretID
            };
            var getSecretValueTask = m_secretsMngrClient.GetSecretValueAsync(request, new System.Threading.CancellationToken());
            getSecretValueTask.Wait();
            var value = getSecretValueTask.Result;
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(value.SecretString);
            return new Tuple<string, string>(
                jObject["aws_access_key_id"].ToString(),
                jObject["aws_secret_access_key"].ToString());
        }
    }
}
