using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lutron.CondorApiIntegration.Test
{
    [TestClass]
    public class AwsSecretsManagerHelperTest
    {
        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCacheStoreAndRetrieve()
        {
            var token = "test_token";
            var secrets = new Dictionary<string, string>()
            {
               { "aws_api_key", "somevalue"},
               { "aws_access_key_id", "somevalue"},
               { "aws_secret_access_key", "somevalue"}
            };
            var secretsManagerHelper = new AwsSecretsManagerHelper();
            secretsManagerHelper.SetSecretsToCache(token, secrets);
            var newSecrets = secretsManagerHelper.GetSecretsFromCache(token);
            Assert.AreEqual(secrets["aws_access_key_id"],       newSecrets["aws_access_key_id"]);
            Assert.AreEqual(secrets["aws_secret_access_key"],   newSecrets["aws_secret_access_key"]);
            Assert.AreEqual(secrets["aws_api_key"],             newSecrets["aws_api_key"]);
        }
    }
}
