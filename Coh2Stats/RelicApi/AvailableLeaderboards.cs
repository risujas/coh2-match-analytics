#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class AvailableLeaderboards
		{
			public struct Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public struct Leaderboardmap
			{
				public int matchtype_id { get; set; }
				public int statgroup_type { get; set; }
				public int race_id { get; set; }
			}

			public struct Leaderboard
			{
				public int id { get; set; }
				public string name { get; set; }
				public int isranked { get; set; }
				public List<Leaderboardmap> leaderboardmap { get; set; }
			}

			public struct MatchType
			{
				public int id { get; set; }
				public string name { get; set; }
				public int locstringid { get; set; }
				public string localizedName { get; set; }
			}

			public struct Race
			{
				public int id { get; set; }
				public string name { get; set; }
				public int faction_id { get; set; }
				public int locstringid { get; set; }
				public string localizedName { get; set; }
			}

			public struct Faction
			{
				public int id { get; set; }
				public string name { get; set; }
				public int locstringid { get; set; }
				public string localizedName { get; set; }
			}

			public struct LeaderboardRegion
			{
				public int id { get; set; }
				public string name { get; set; }
				public int locstringid { get; set; }
			}

			public class Root
			{
				public Result result { get; set; }
				public List<Leaderboard> leaderboards { get; set; }
				public List<MatchType> matchTypes { get; set; }
				public List<Race> races { get; set; }
				public List<Faction> factions { get; set; }
				public List<LeaderboardRegion> leaderboardRegions { get; set; }

				private Root()
				{
				}
			}

			public static Root Get()
			{
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetAvailableLeaderboards";
				string requestParams = "?title=coh2";

				return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
			}
		}
	}
}
