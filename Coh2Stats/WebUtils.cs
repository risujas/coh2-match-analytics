using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Coh2Stats
{
	class WebUtils
	{
        public static string GetJsonResponse(string requestUrl, string requestParams)
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
    }
}
