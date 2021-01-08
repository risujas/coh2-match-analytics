using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class JsonLeaderboard
		{
			public class Root
			{
				[JsonProperty("result")] public WebRequestResult Result { get; set; }
				[JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
				[JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
				[JsonProperty("rankTotal")] public int RankTotal { get; set; }
			}

			public static Root GetById(int leaderboardId, int startRank = -1, int numRanks = -1)
			{
				if (startRank == -1)
				{
					startRank = 1;
				}

				if (numRanks == -1)
				{
					numRanks = 200;
				}

				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getLeaderBoard2";
				string requestParams = "?title=coh2&leaderboard_id=" + leaderboardId.ToString() + "&start=" + startRank.ToString() + "&count=" + numRanks.ToString();

				var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				foreach (var sg in response.StatGroups)
				{
					foreach (var x in sg.Members)
					{
						PlayerIdentityTracker.LogPlayer(x);
					}

					StatGroupTracker.LogStatGroup(sg);
				}

				foreach (var lbs in response.LeaderboardStats)
				{
					LeaderboardStatTracker.LogStat(lbs);
				}

				return response;
			}
		}
	}
}
