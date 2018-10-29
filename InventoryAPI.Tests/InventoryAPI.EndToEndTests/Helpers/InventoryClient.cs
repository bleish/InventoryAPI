using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InventoryAPI.EndToEndTests.Helpers
{
    public class InventoryClient
    {
        private const string DefaultBaseUrl = "https://localhost:5001";

        private HttpClient Client { get; }

        public InventoryClient(string baseUrl = DefaultBaseUrl)
        {
            var handler = new HttpClientHandler
            {
                // Ignore SSL certificate validation
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        #region HTTP Methods

        public async Task<Result> Get(string endpoint, string resourceId = null, Dictionary<string, string> parameters = null)
        {
            var url = string.IsNullOrWhiteSpace(resourceId) ? endpoint : $"{endpoint}/{HttpUtility.UrlEncode(resourceId)}";
            url = await AddParametersToUrl(url, parameters);
            var response = await Client.GetAsync(url);

            return await GetResult(response);
        }

        public async Task<Result> Post(string endpoint, object obj, Dictionary<string, string> parameters = null, string contentType = "application/json")
        {
            var url = await AddParametersToUrl(endpoint, parameters);
            var response = await Client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, contentType));

            return await GetResult(response);
        }

        public async Task<Result> Put(string endpoint, object obj, string resourceId = null, string contentType = "application/json")
        {
            var url = string.IsNullOrWhiteSpace(resourceId) ? endpoint : $"{endpoint}/{HttpUtility.UrlEncode(resourceId)}";
            var response = await Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, contentType));

            return await GetResult(response);
        }

        public async Task<Result> Delete(string endpoint, string resourceId)
        {
            var response = await Client.DeleteAsync($"{endpoint}/{HttpUtility.UrlEncode(resourceId)}");

            return await GetResult(response);
        }

        #endregion

        #region Helper Methods

        private static async Task<string> AddParametersToUrl(string url, Dictionary<string, string> parameters)
        {
            if (parameters == null) return url;

            var query = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();
            return $"{url}?{query}";
        }

        private async Task<Result> GetResult(HttpResponseMessage response)
        {
            return new Result
            {
                Response = response,
                Content = await response.Content.ReadAsStringAsync()
            };
        }

        #endregion
    }
}