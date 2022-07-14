using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lutron.CondorApiIntegration.LoginClient;
using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using System.Runtime.Caching;
using Newtonsoft.Json;

namespace Lutron.CondorApiIntegration
{
    /// <summary>
    /// Makes Login request to retrieve secrets or gets them from cache
    /// Stores secrets in cache
    /// </summary>
    public class AwsSecretsManagerHelper
    {

        /// <summary>
        /// Cache timeout hours
        /// </summary>
        private static readonly double CACHE_TIMEOUT = 120;

        /// <summary>
        /// 
        /// </summary>
        public AwsSecretsManagerHelper()
        {
        }

        /// <summary>
        /// Gets secrets from cache or makes a login request
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ResetSecrets">Allows to reset secrets if the expire</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetSecretsAsync(CondorApiVariablesBase apiVariables, string cacheKey, bool resetSecrets)
        {
            var secrets = GetSecretsFromCache(cacheKey);
            if (secrets == null || resetSecrets)
            {
                var loginCondorApi = CondorApiClientProvider.GetLoginClient(apiVariables.UserManagementToken, apiVariables.BaseUrl);
                var response = await loginCondorApi.ExecuteRequest(null);
                var result = response;
                if (result.Item1 == ResponseResult.Success)
                {
                    var loginResult = result.Item2;
                    secrets = loginResult[0].Secrets;
                    SetSecretsToCache(cacheKey, secrets);
                }
            }
            return secrets;
        }

        /// <summary>
        /// Gets secrets from cache
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSecretsFromCache(string cacheKey)
        {
            Dictionary<string, string> secretsDict = null;
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(cacheKey)) { 
                var responseDict = ToDictionary<string>(cache.Get(cacheKey));
                var allConstants = (responseDict.ContainsKey("aws_access_key_id"))
                                && (responseDict.ContainsKey("aws_secret_access_key"))
                                && (responseDict.ContainsKey("aws_api_key"));
                if (allConstants)
                {
                    secretsDict = responseDict;
                }
            } 
            return secretsDict;
        }

        /// <summary>
        /// Sets secrets to cache
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secrets"></param>
        public void SetSecretsToCache(string cacheKey, Dictionary<string, string> secrets)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(CACHE_TIMEOUT);
            cache.Add(cacheKey, secrets, cacheItemPolicy);
        }


        /// <summary>
        /// Converts object to dictionary
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }
    }
}
