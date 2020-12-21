using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Coh2Stats
{
	class Program
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

        static void Main(string[] args)
		{
            var lb = LeaderboardResponse.GetLeaderboardById(5, 1, 10);
            for (int i = 0; i < lb.statGroups.Count; i++)
			{
                Console.WriteLine(lb.statGroups[i].members[0].alias + " " + lb.statGroups[i].members[0].country + " " + lb.statGroups[i].members[0].profile_id);
			}

            Console.ReadLine();
		}
	}
}