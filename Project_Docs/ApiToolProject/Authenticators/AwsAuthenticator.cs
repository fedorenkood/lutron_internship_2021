using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace com.ketra.APITools
{
    public class AwsAuthenticator : IAuthenticator
    {
        public string AccessKey { get; }
        public string SecretKey { get; }
        public string Region { get; }

        public AwsAuthenticator(string accessKeyId, string accessKeySecret, string region)
        {
            AccessKey = accessKeyId;
            SecretKey = accessKeySecret;
            Region = region;
        }

        private static string[] unsignedHeaders = new string[] {
            "authorization",
            "content-length",
            "content-type",
            "user-agent"
        };

        public void Authenticate(RestSharp.IRestClient client, RestSharp.IRestRequest request)
        {
            DateTime signingDate = DateTime.UtcNow;
            SetContentSha256(request);
            SetHostHeader(request, client);
            SetDateHeader(request, signingDate);
            SortedDictionary<string, string> headersToSign = GetHeadersToSign(request);
            string signedHeadersString = string.Join(";", headersToSign.Keys);
            string canonicalRequestHash = GetCanonicalRequestHash(client, request, headersToSign);
            string signature = GetSignature(Region, signingDate, canonicalRequestHash);
            string authorization = GetAuthorizationHeader(signedHeadersString, signature, signingDate, Region);
            request.AddHeader("Authorization", authorization);

        }

        public string GetCredentialString(DateTime signingDate, string region)
        {
            return AccessKey + "/" + GetScope(region, signingDate);
        }

        private string GetAuthorizationHeader(string signedHeaders, string signature, DateTime signingDate, string region)
        {
            string credentials = GetCredentialString(signingDate, region);
            return $"AWS4-HMAC-SHA256 Credential={credentials}, SignedHeaders={signedHeaders}, Signature={signature}";
        }

        private byte[] GenerateSigningKey(string region, DateTime signingDate)
        {
            byte[] formattedDateBytes = System.Text.Encoding.UTF8.GetBytes(signingDate.ToString("yyyyMMdd"));
            byte[] formattedKeyBytes = System.Text.Encoding.UTF8.GetBytes("AWS4" + this.SecretKey);
            byte[] dateKey = SignHmac(formattedKeyBytes, formattedDateBytes);

            byte[] regionBytes = System.Text.Encoding.UTF8.GetBytes(region);
            byte[] dateRegionKey = SignHmac(dateKey, regionBytes);

            byte[] serviceBytes = System.Text.Encoding.UTF8.GetBytes("execute-api");
            byte[] dateRegionServiceKey = SignHmac(dateRegionKey, serviceBytes);

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes("aws4_request");
            return SignHmac(dateRegionServiceKey, requestBytes);
        }

        private byte[] SignHmac(byte[] key, byte[] content)
        {
            HMACSHA256 hmac = new HMACSHA256(key);
            hmac.Initialize();
            return hmac.ComputeHash(content);
        }

        private string GetSignature(
            string region,
            DateTime signingDate,
            string canonicalRequestHash)
        {
            string stringToSign = GetStringToSign(Region, signingDate, canonicalRequestHash);
            byte[] stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
            byte[] signingKey = GenerateSigningKey(Region, signingDate);
            byte[] signatureBytes = SignHmac(signingKey, stringToSignBytes);
            return BytesToHex(signatureBytes);
        }

        private string GetStringToSign(
            string region,
            DateTime signingDate,
            string canonicalRequestHash)
        {
            return "AWS4-HMAC-SHA256\n" +
                signingDate.ToString("yyyyMMddTHHmmssZ") + "\n" +
                GetScope(region, signingDate) + "\n" +
                canonicalRequestHash;
        }

        private string GetScope(string region, DateTime signingDate)
        {
            string formattedDate = signingDate.ToString("yyyyMMdd");
            return formattedDate + "/" + region + "/execute-api/aws4_request";
        }

        private byte[] ComputeSha256(byte[] body)
        {

            SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(body);
        }

        private string BytesToHex(byte[] checkSum)
        {
            return BitConverter.ToString(checkSum).Replace("-", string.Empty).ToLower();
        }

        private string GetCanonicalRequestHash(
            IRestClient client,
            IRestRequest request,
            SortedDictionary<string, string> headersToSign)
        {
            string canonicalRequest = GetCanonicalRequest(client, request, headersToSign);
            byte[] canonicalRequestBytes = System.Text.Encoding.UTF8.GetBytes(canonicalRequest);
            return BytesToHex(ComputeSha256(canonicalRequestBytes));
        }

        private string GetCanonicalRequest(
            IRestClient client,
            IRestRequest request,
            SortedDictionary<string, string> headersToSign)
        {
            List<string> canonicalStringList = new List<string>();
            canonicalStringList.Add(request.Method.ToString());

            string[] path = request.Resource.Split(new char[] { '?' }, 2);
            if (!path[0].StartsWith("/"))
            {
                path[0] = "/" + path[0];
            }
            canonicalStringList.Add(path[0]);

            string query = "";
            if (path.Length == 2)
            {
                var parameterString = path[1];
                var parameterList = parameterString.Split('&');
                SortedSet<string> sortedQueries = new SortedSet<string>();
                foreach (string individualParameterString in parameterList)
                {
                    if (individualParameterString.Contains("="))
                    {
                        string[] splitQuery = individualParameterString.Split(new char[] { '=' }, 2);
                        sortedQueries.Add(splitQuery[0] + "=" + splitQuery[1]);
                    }
                    else
                    {
                        sortedQueries.Add(individualParameterString + "=");
                    }
                }
                query = string.Join("&", sortedQueries);
            }
            canonicalStringList.Add(query);

            foreach (string header in headersToSign.Keys)
            {
                canonicalStringList.Add(header + ":" + headersToSign[header]);
            }
            canonicalStringList.Add("");

            canonicalStringList.Add(string.Join(";", headersToSign.Keys));
            if (headersToSign.Keys.Contains("x-amz-content-sha256"))
            {
                canonicalStringList.Add(headersToSign["x-amz-content-sha256"]);
            }
            else
            {
                canonicalStringList.Add("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            }

            return string.Join("\n", canonicalStringList);
        }

        private SortedDictionary<string, string> GetHeadersToSign(RestSharp.IRestRequest request)
        {
            var headers = request.Parameters.Where(p => p.Type.Equals(RestSharp.ParameterType.HttpHeader)).ToList();

            SortedDictionary<string, string> sortedHeaders = new SortedDictionary<string, string>();
            foreach (var header in headers)
            {
                string headerName = header.Name.ToLower();
                string headerValue = header.Value.ToString();
                if (!unsignedHeaders.Contains(headerName))
                {
                    sortedHeaders.Add(headerName, headerValue);
                }
            }
            return sortedHeaders;
        }

        private void SetDateHeader(RestSharp.IRestRequest request, DateTime signingDate)
        {
            request.AddHeader("X-Amz-Date", signingDate.ToString("yyyyMMddTHHmmssZ"));
        }

        private void SetHostHeader(RestSharp.IRestRequest request, RestSharp.IRestClient client)
        {
            request.AddHeader("Host", client.BaseUrl.Host);
            //request.AddHeader("Host", client.BaseUrl.Host + (client.BaseUrl.Port != 80 ? ":" + client.BaseUrl.Port : string.Empty));
        }

        private void SetContentSha256(RestSharp.IRestRequest request)
        {
            if (request.Method == RestSharp.Method.PUT || request.Method.Equals(RestSharp.Method.POST))
            {
                var bodyParameter = request.Parameters.Where(p => p.Type.Equals(RestSharp.ParameterType.RequestBody)).FirstOrDefault();
                if (bodyParameter == null)
                {
                    request.AddHeader("X-Amz-Content-Sha256", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
                    return;
                }
                byte[] body = null;
                if (bodyParameter.Value is string)
                {
                    body = System.Text.Encoding.UTF8.GetBytes(bodyParameter.Value as string);
                }
                if (bodyParameter.Value is byte[])
                {
                    body = bodyParameter.Value as byte[];
                }
                if (body == null)
                {
                    body = new byte[0];
                }
                SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
                byte[] hash = sha256.ComputeHash(body);
                string hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                request.AddHeader("X-Amz-Content-Sha256", hex);
            }
            else
            {
                request.AddHeader("X-Amz-Content-Sha256", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            }
        }
    }
}
