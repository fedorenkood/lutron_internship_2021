using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockHttpServer;
using System;
using System.Collections.Generic;
using System.Net;

namespace Lutron.CondorApiIntegration.Test
{
    [TestClass]
    public class AreaTypePredictionCondorApiClientTest
    {
        private static int TestPort = 9900;
        private AreaTypePredictionCondorApiClient AreaTypePredictionClient;

        [TestInitialize]
        public void Initialize()
        {
            AreaTypePredictionClient = CondorApiClientProvider.GetAreaTypePredictionClient("", $"http://localhost:{TestPort}/");
        }

        private MockServer GetAreaTypePredictionMockServer(int responseStatus, string responseString)
        {
            var requestHandlers = new List<MockHttpHandler>()
            {
                new MockHttpHandler("/v1/login", "POST", (req, rsp, prm) => {
                    rsp.StatusCode = (int)HttpStatusCode.OK;
                    return "{\"aws_api_key\": \"somevalue\", \"aws_access_key_id\": \"somevalue\",\"aws_secret_access_key\": \"somevalue\"}";
                }),
                new MockHttpHandler("v1/run/roomtype-prediction-residential/1.0.1", "POST", (req, rsp, prm) => {
                    rsp.StatusCode = responseStatus;
                    return responseString;
                })
            };
            return new MockServer(TestPort, requestHandlers);
        }

        private List<AreaTypePredictionData> GetAreaTypePredictionDatas(string area)
        {
            return new List<AreaTypePredictionData> {
                new AreaTypePredictionData(area)
            };
        }

        #region AreaTypePredictionClient

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiAreaTypePrediction_Success()
        {
            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                var response = AreaTypePredictionClient.ExecuteRequest(GetAreaTypePredictionDatas("Something"));
                response.Wait();
                var result = response.Result;
                Assert.AreEqual(ResponseResult.Success, result.Item1, "AreaTypePredictionCondorApiHelper failed to return Sucessful result");
                if (result.Item1 == ResponseResult.Success)
                {
                    Assert.AreEqual("value_one", result.Item2[0].AreaTypePredictedStringArray[0]);
                    Assert.AreEqual("value_two", result.Item2[0].AreaTypePredictedStringArray[1]);
                    Assert.AreEqual("value_one", result.Item2[1].AreaTypePredictedStringArray[0]);
                    Assert.AreEqual("value_two", result.Item2[1].AreaTypePredictedStringArray[1]);
                }
            }
        }

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiAreaTypePrediction_Malformated()
        {
            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                var response = AreaTypePredictionClient.ExecuteRequest(GetAreaTypePredictionDatas("Something"));
                response.Wait();
                var result = response.Result;
                Assert.AreEqual(ResponseResult.MalformedResponse, result.Item1, "AreaTypePredictionCondorApiHelper failed to return Fail result");
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
                var responseStatus = (int) tuple.Item1;
                var responseString = "{}";
                using (GetAreaTypePredictionMockServer(responseStatus, responseString))
                {
                    var response = AreaTypePredictionClient.ExecuteRequest(GetAreaTypePredictionDatas("Something"));
                    response.Wait();
                    var result = response.Result;
                    Assert.AreEqual(tuple.Item2, result.Item1, "AreaTypePredictionCondorApiHelper failed to return " + tuple.Item2.ToString() + " result");
                }
            }
        }

        [TestMethod]
        [Owner("ofedorenko")]
        public void TestCondorApiAreaTypePrediction_Offline()
        {
            var response = AreaTypePredictionClient.ExecuteRequest(GetAreaTypePredictionDatas("Something"));
            response.Wait();
            var result = response.Result;
            Assert.AreEqual(ResponseResult.Fail, result.Item1, "AreaTypePredictionCondorApiHelper failed to return Fail result");
            
        }
        #endregion

    }
}
