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

namespace DBox_CS.Core.APIClient
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseurl;
        private readonly string _getAllLocationsurl;
        private readonly string _addUpdateEmployeeurl;
        private readonly string _getEmployeeurl;

        public ApiClient(HttpClient httpClient, string apiKey, string apiKeyHeader)
        {
            //Force Set security protocol to use TLS 1.2(by default tls 1.2 if below line not mentioned)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add(apiKeyHeader, apiKey);
            

            _baseurl = ConfigurationManager.AppSettings.Get("UFApiSettings.BaseUrl");
            _getAllLocationsurl = _baseurl + ConfigurationManager.AppSettings.Get("UFApiSettings.Endpoints.GetAllLocations");
            _addUpdateEmployeeurl = _baseurl + ConfigurationManager.AppSettings.Get("UFApiSettings.Endpoints.AddUpdateEmployee");
            _getEmployeeurl = _baseurl + ConfigurationManager.AppSettings.Get("UFApiSettings.Endpoints.GetEmployee");
        }
        public string GetToken(string username, string password)
        {
            //var loginData = new { Username = username, Password = password };
            //var json = JsonConvert.SerializeObject(loginData);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            ////var response = _httpClient.PostAsync("https://localhost:44382/login", content).GetAwaiter().GetResult();
            ////var response = _httpClient.PostAsync("https://cpalsalamjed.civilsoft.org/TAReadingAPI/login", content).GetAwaiter().GetResult();
            //var response = _httpClient.PostAsync(_authurl, content).GetAwaiter().GetResult();
            //response.EnsureSuccessStatusCode();
            //var tokenResponse =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //var tokenresp = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);
            //return tokenresp.Token;

            return "";

        }
        public HttpResponseMessage PostEmployeeData(UFEmployeeDTO data, string paraLocationCode, string paraEmployeeid)
        {

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //robin testing:uncomment for testing
            //AppClass.Common.LogAction(json);
            //AppClass.Common.LogAction("{location-code}:" + paraLocationCode);
            //AppClass.Common.LogAction("{location-code}:" + paraEmployeeid);

            string addUpdateEmployeeurl_WithPara = _addUpdateEmployeeurl;
            addUpdateEmployeeurl_WithPara = addUpdateEmployeeurl_WithPara.Replace("{location-code}", paraLocationCode);
            addUpdateEmployeeurl_WithPara = addUpdateEmployeeurl_WithPara.Replace("{employee-id}", paraEmployeeid);

            //robin testing:commented for testing
            var result = _httpClient.PutAsync(addUpdateEmployeeurl_WithPara, content).GetAwaiter().GetResult();

            //robin testing:uncomment for testing
            //HttpResponseMessage result = null;

            return result;
            
        }
    }
}
