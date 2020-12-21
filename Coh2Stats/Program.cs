using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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

        public static LeaderboardResponse GetLeaderboardById(int leaderboardId, int startRank, int numRanks)
        {
            string url = "https://coh2-api.reliclink.com/community/leaderboard/getLeaderBoard2";
            string urlParams = "?title=coh2&leaderboard_id=5&start=" + startRank.ToString() + "&count=" + numRanks.ToString();

            string jsonResponse = GetJsonResponse(url, urlParams);
            LeaderboardResponse leaderboardResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(jsonResponse);

            return leaderboardResponse;
        }

        static void Main(string[] args)
		{
            var lb = GetLeaderboardById(5, 1, 10);
            for (int i = 0; i < lb.statGroups.Count; i++)
			{
                Console.WriteLine(lb.statGroups[i].members[0].alias + " " + lb.statGroups[i].members[0].country + " " + lb.statGroups[i].members[0].profile_id);
			}

            Console.ReadLine();
		}
	}
}