using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class JsonAvailableLeaderboards
		{
			public class Leaderboardmap
			{
				[JsonProperty("matchtype_id")] public int MatchTypeId { get; set; }
				[JsonProperty("statgroup_type")] public int StatGroupType { get; set; }
				[JsonProperty("race_id")] public int RaceId { get; set; }
			}

			public class Leaderboard
			{
				[JsonProperty("id")] public int Id { get; set; }
				[JsonProperty("name")] public string Name { get; set; }
				[JsonProperty("isranked")] public int IsRanked { get; set; }
				[JsonProperty("leaderboardmap")] public List<Leaderboardmap> LeaderboardMap { get; set; }
			}

			public class MatchType
			{
				[JsonProperty("id")] public int Id { get; set; }
				[JsonProperty("name")] public string Name { get; set; }
				[JsonProperty("locstringid")] public int LocStringId { get; set; }
				[JsonProperty("localizedName")] public string LocalizedName { get; set; }
			}

			public class Race
			{
				[JsonProperty("id")] public int Id { get; set; }
				[JsonProperty("name")] public string Name { get; set; }
				[JsonProperty("faction_id")] public int FactionId { get; set; }
				[JsonProperty("locstringid")] public int LocStringId { get; set; }
				[JsonProperty("localizedName")] public string LocalizedName { get; set; }
			}

			public class Faction
			{
				[JsonProperty("id")] public int Id { get; set; }
				[JsonProperty("name")] public string Name { get; set; }
				[JsonProperty("locstringid")] public int LocStringId { get; set; }
				[JsonProperty("localizedName")] public string LocalizedName { get; set; }
			}

			public class LeaderboardRegion
			{
				[JsonProperty("id")] public int Id { get; set; }
				[JsonProperty("name")] public string Name { get; set; }
				[JsonProperty("locstringid")] public int LocStringId { get; set; }
			}

			public class Root
			{
				[JsonProperty("result")] public WebRequestResult Result { get; set; }
				[JsonProperty("leaderboards")] public List<Leaderboard> Leaderboards { get; set; }
				[JsonProperty("matchTypes")] public List<MatchType> MatchTypes { get; set; }
				[JsonProperty("races")] public List<Race> Races { get; set; }
				[JsonProperty("factions")] public List<Faction> Factions { get; set; }
				[JsonProperty("leaderboardRegions")] public List<LeaderboardRegion> LeaderboardRegions { get; set; }
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
