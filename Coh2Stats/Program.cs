using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class Program
	{
        public static LeaderboardResponse GetLeaderboardById(int leaderboardId, int startRank, int numRanks)
        {
            LeaderboardResponse leaderboardResponse;

            string url = "https://coh2-api.reliclink.com/community/leaderboard/getLeaderBoard2";
            string urlParams = "?title=coh2&leaderboard_id=5&start=" + startRank.ToString() + "&count=" + numRanks.ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(urlParams).Result;
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                leaderboardResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(jsonResponse);

            }
            else
            {
                throw new Exception((int)response.StatusCode + " (" + response.ReasonPhrase + ")");
            }

            client.Dispose();

            return leaderboardResponse;
        }

        static void Main(string[] args)
		{
            GetLeaderboardById(5, 1, 10);
            Console.ReadLine();
		}
	}
}