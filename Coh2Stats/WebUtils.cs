using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Coh2Stats
{
    class WebUtils
    {
        public static string GetJsonResponseString(string requestUrl, string requestParams)
        {
            string responseString;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(requestUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseObject = client.GetAsync(requestParams).Result;
            if (responseObject.IsSuccessStatusCode)
            {
                responseString = responseObject.Content.ReadAsStringAsync().Result;

            }
            else
            {
                throw new Exception((int)responseObject.StatusCode + " (" + responseObject.ReasonPhrase + ")");
            }

            client.Dispose();

            return responseString;
        }

        public static T GetStructuredJsonResponse<T>(string requestUrl, string requestParams)
        {
            string responseString = GetJsonResponseString(requestUrl, requestParams);
            T structuredResponse = JsonConvert.DeserializeObject<T>(responseString);
            return structuredResponse;
        }
    }
}
