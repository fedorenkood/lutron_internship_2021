using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using Lutron.CondorApiIntegration.LoginClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockHttpServer;
using System;
using System.Collections.Generic;
using System.Net;

namespace Lutron.CondorApiIntegration.Test
{
    [TestClass]
    public class LoginCondorApiClientTest
    {
        private static int TestPort = 9900;
        private LoginCondorApiClient LoginClient;

        [TestInitialize]
        public void Initialize()
        {
            LoginClient = CondorApiClientProvider.GetLoginClient("", $"http://localhost:{TestPort}/");
        }

        private MockServer GetLoginMockServer(int responseStatus, string responseString)
        {
            var requestHandlers = new List<MockHttpHandler>()
            {
                new MockHttpHandler("/v1/login", "POST", (req, rsp, prm) => {
                    rsp.StatusCode = responseStatus;
                    return responseString;
                }),
            };
            return new MockServer(TestPort, requestHandlers);
        }

        #region LoginClient

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiLogin_Success()
        {
            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"aws_api_key\": \"somevalue\", \"aws_access_key_id\": \"somevalue\",\"aws_secret_access_key\": \"somevalue\"}";

            using (GetLoginMockServer(responseStatus, responseString))
            {
                var response = LoginClient.ExecuteRequest(null);
                response.Wait();
                var result = response.Result;
                Assert.AreEqual(ResponseResult.Success, result.Item1, "LoginCondorApiHelper failed to return Sucessful result");
                if (result.Item1 == ResponseResult.Success)
                {
                    var loginResult = result.Item2;
                    var secrets = loginResult[0].Secrets;
                    Assert.AreEqual("somevalue", secrets["aws_access_key_id"]);
                    Assert.AreEqual("somevalue", secrets["aws_secret_access_key"]);
                    Assert.AreEqual("somevalue", secrets["aws_api_key"]);
                }
            }
        }

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiLogin_Malformated()
        {
            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{}";
            using (GetLoginMockServer(responseStatus, responseString))
            {
                var response = LoginClient.ExecuteRequest(null);
                response.Wait();
                var result = response.Result;
                Assert.AreEqual(ResponseResult.MalformedResponse, result.Item1, "LoginCondorApiHelper failed to return MalformedResponse result");
            }
        }

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiAreaTypePrediction_OtherErrors()
        {
            List<Tuple<HttpStatusCode, ResponseResult>> responseResultList = new List<Tuple<HttpStatusCode, ResponseResult>>
            {
                new Tuple<HttpStatusCode, ResponseResult> ( HttpStatusCode.BadRequest , ResponseResult.BadRequest ),
                new Tuple<HttpStatusCode, ResponseResult> ( HttpStatusCode.Unauthorized , ResponseResult.Unauthorized ),
                new Tuple<HttpStatusCode, ResponseResult> ( HttpStatusCode.NotFound , ResponseResult.NotFound ),
                new Tuple<HttpStatusCode, ResponseResult> ( HttpStatusCode.ServiceUnavailable , ResponseResult.ServiceUnavailable ),
                new Tuple<HttpStatusCode, ResponseResult> ( HttpStatusCode.InternalServerError , ResponseResult.ServerError )

            };
            foreach (var tuple in responseResultList)
            {
                var responseStatus = (int)tuple.Item1;
                var responseString = "{}";
                using (GetLoginMockServer(responseStatus, responseString))
                {
                    var response = LoginClient.ExecuteRequest(null);
                    response.Wait();
                    var result = response.Result;
                    Assert.AreEqual(tuple.Item2, result.Item1, "LoginCondorApiHelper failed to return " + tuple.Item2.ToString() + " result");
                }
            }
        }

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiAreaTypePrediction_Offline()
        {
            var response = LoginClient.ExecuteRequest(null);
            response.Wait();
            var result = response.Result;
            Assert.AreEqual(ResponseResult.Fail, result.Item1, "LoginCondorApiHelper failed to return Fail result");

        }

        #endregion


    }
}
