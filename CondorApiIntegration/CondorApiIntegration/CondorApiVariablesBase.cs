using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lutron.CondorApiIntegration
{
    /// <summary>
    /// Holds variables that are used in cosntructing the request in the CondorApiClient
    /// </summary>
    public abstract class CondorApiVariablesBase
    {
        protected Dictionary<string, object> customProperties;
        protected string baseUrl;
        protected string modelExtension;
        protected string region;
        protected string logType;
        protected Method method;
        protected string cacheKey;
        protected readonly string userManagementToken;

        /// <summary>
        /// Holds Custom properties read from the json file
        /// </summary>
        virtual public Dictionary<string, object> CustomProperties { get { return customProperties; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string BaseUrl { get { return baseUrl; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string ModelExtension { get { return modelExtension; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string Region { get { return region; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string LogType { get { return logType; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public RestSharp.Method Method { get { return method; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string CacheKey { get { return cacheKey; } }
        /// <summary>
        /// 
        /// </summary>
        virtual public string UserManagementToken { get { return userManagementToken; } }


        protected CondorApiVariablesBase(string userManagementToken)
        {
            this.userManagementToken = userManagementToken;
        }

        abstract protected void InstantiateConstants();

        protected Dictionary<string, object> ParseConfigJson(string ManifestResourceName)
        {
            try
            {
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(ManifestResourceName))
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());
                }

            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }

        protected void SetBaseProperties(Dictionary<string, object> dictionary)
        {
            var newDictionary = new Dictionary<string, object>(dictionary);
            foreach (var entry in newDictionary)
            {
                switch (entry.Key)
                {
                    case nameof(baseUrl):
                        baseUrl = (string)entry.Value;
                        dictionary.Remove(entry.Key);
                        break;
                    case nameof(modelExtension):
                        modelExtension = (string)entry.Value;
                        dictionary.Remove(entry.Key);
                        break;
                    case nameof(region):
                        region = (string)entry.Value;
                        dictionary.Remove(entry.Key);
                        break;
                    case nameof(logType):
                        logType = (string)entry.Value;
                        dictionary.Remove(entry.Key);
                        break;
                    case nameof(method):
                        method = (Method)Enum.Parse(typeof(Method), (string)entry.Value);
                        dictionary.Remove(entry.Key);
                        break;
                    case nameof(cacheKey):
                        cacheKey = (string)entry.Value;
                        dictionary.Remove(entry.Key);
                        break;
                }
            }
        }
    }
}
