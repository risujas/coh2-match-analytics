using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Coh2Stats
{
    class LeaderboardEntry
	{
        public string race = "";
        public int rank = 0;
        public string steamID = "";
        public int level = 0;
	}

    class Leaderboard
	{
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        public static Leaderboard GetDataById(int leaderboardId)
		{
            Leaderboard lb = new Leaderboard();

            string url = "https://coh2-api.reliclink.com/community/leaderboard/getLeaderBoard2";
            string urlParams = "?title=coh2&leaderboard_id=5&start=1&count=125";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(urlParams).Result;
            if (response.IsSuccessStatusCode)
			{
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonResponse);

                for (int i = 0; i < myDeserializedClass.statGroups.Count; i++)
				{
                   // Console.WriteLine(myDeserializedClass.statGroups[i].members[0].alias);
                }
                
			}
            else
			{
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
			}

            client.Dispose();
            Console.ReadLine();

            return lb;
        }
	}

	class Program
	{
		static void Main(string[] args)
		{
            Leaderboard.GetDataById(5);
		}
	}
}