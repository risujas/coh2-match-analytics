#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class JsonPersonalStat
		{
			public class Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public class Member: PlayerIdentity
			{
			}

			public class StatGroup
			{
				public int id { get; set; }
				public string name { get; set; }
				public int type { get; set; }
				public List<Member> members { get; set; }
			}

			public class LeaderboardStat
			{
				public int statGroup_id { get; set; }
				public int leaderboard_id { get; set; }
				public int wins { get; set; }
				public int losses { get; set; }
				public int streak { get; set; }
				public int disputes { get; set; }
				public int drops { get; set; }
				public int rank { get; set; }
				public int rankTotal { get; set; }
				public int regionRank { get; set; }
				public int regionRankTotal { get; set; }
				public int rankLevel { get; set; }
				public int lastMatchDate { get; set; }
			}

			public class Root
			{
				public Result result { get; set; }
				public List<StatGroup> statGroups { get; set; }
				public List<LeaderboardStat> leaderboardStats { get; set; }

				private Root()
				{
				}
			}

			public static Root GetBySteamId(string steamId)
			{
				List<string> list = new List<string>();
				list.Add(steamId);
				return GetBySteamId(list);
			}

			public static Root GetBySteamId(List<string> steamIds)
			{
				string idString = "\"" + string.Join("\",\"", steamIds) + "\"";
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
				string requestParams = "?title=coh2&profile_names=[" + idString + "]";

				var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				foreach (var sg in response.statGroups)
				{
					foreach (var x in sg.members)
					{
						PlayerIdentityTracker.LogPlayer(new PlayerIdentity(x));
					}
				}

				return response;
			}
		}
	}
}
