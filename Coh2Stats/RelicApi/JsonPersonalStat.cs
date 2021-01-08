using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class JsonPersonalStat
		{
			public class Root
			{
				[JsonProperty("result")] public WebRequestResult Result { get; set; }
				[JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
				[JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
			}

			public static Root GetBySteamId(string steamId)
			{
				List<string> list = new List<string> { steamId };
				return GetBySteamId(list);
			}

			public static Root GetBySteamId(List<string> steamIds)
			{
				string idString = "\"" + string.Join("\",\"", steamIds) + "\"";
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
				string requestParams = "?title=coh2&profile_names=[" + idString + "]";

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
