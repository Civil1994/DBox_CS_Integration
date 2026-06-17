using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Models;
using System.Data.SqlClient;
using DBox_CS.Core.BL;

namespace DBox_CS.Core.APIClient
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseurl;
        private readonly string _pushEmployee;
        private readonly string _apikeyheader;
        private readonly string _apikey;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _pushDocument;
        private readonly string _authCode;
        public ApiClient(HttpClient httpClient, string apiKey, string apiKeyHeader)
        {
            //Force Set security protocol to use TLS 1.2(by default tls 1.2 if below line not mentioned)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add(apiKeyHeader, apiKey);
            

            _baseurl = ConfigurationManager.AppSettings.Get("DBOXApiSettings.BaseUrl");
            _pushEmployee = _baseurl + ConfigurationManager.AppSettings.Get("DBOXApiSettings.Endpoints.PushEmployee");
            _pushDocument = _baseurl + ConfigurationManager.AppSettings.Get("DBOXApiSettings.Endpoints.PushDocuments");

            _apikeyheader = ConfigurationManager.AppSettings["DBOXApiSettings.APIKeyHeader"].ToString();
            _apikey = ConfigurationManager.AppSettings["DBOXApiSettings.APIKey"].ToString();
            _authCode = ConfigurationManager.AppSettings["DBOXApiSettings.AuthCode"].ToString();
            _clientId = ConfigurationManager.AppSettings["DBOXApiSettings.ClientId"].ToString();
            _clientSecret = ConfigurationManager.AppSettings["DBOXApiSettings.ClientSecret"].ToString();
        }
        public string GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                var url = _baseurl + "oauth/v2token";

                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientId),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                };

                var content = new FormUrlEncodedContent(formData);

                var response = client.PostAsync(url, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                dynamic json = JsonConvert.DeserializeObject(result);

                return json.access_token;
            }
        }

        public string GetAccessToken1()
        {
            using (var client = new HttpClient())
            {
                var url = _baseurl + "oauth/v1token";

                var formData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", _authCode)
        };

                var content = new FormUrlEncodedContent(formData);

                var response = client.PostAsync(url, content).Result;

                var result = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Token API Error: " + result);

                dynamic json = JsonConvert.DeserializeObject(result);

                return json.access_token;
            }
        }
        public HttpResponseMessage PostEmployeeData(EmployeePushModel data)
        {

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string addUpdateEmployeeurl_WithPara = _pushEmployee;
            string token = GetAccessToken(); // Get JWT token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //robin testing:commented for testing
            var result = _httpClient.PutAsync(addUpdateEmployeeurl_WithPara, content).GetAwaiter().GetResult();

            return result;

        }

        public HttpResponseMessage PostEmployeeDocument(DocumentPushDTO data)
        {
            try
            {

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _pushDocument);

                // API KEY must be in HEADER NAME, not Authorization
                request.Headers.Clear();
                request.Headers.Add("TOKEN", GetAccessToken1()); // 👈 IMPORTANT FIX

                request.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = content;

                var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

                return response;
            }
            catch (Exception ex)
            {
                string e = ex.Message;
           

                throw; // rethrow so caller knows the request failed
            }
        }
    }
}
