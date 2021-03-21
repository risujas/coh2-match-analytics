using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats.RelicAPI
{
	public class RecentMatchHistory
	{
		public class Matchhistoryreportresult
		{
			[JsonProperty("matchhistory_id")] public int MatchHistoryId { get; set; }
			[JsonProperty("profile_id")] public int ProfileId { get; set; }
			[JsonProperty("resulttype")] public int ResultType { get; set; }
			[JsonProperty("teamid")] public int TeamId { get; set; }
			[JsonProperty("race_id")] public int RaceId { get; set; }
			[JsonProperty("xpgained")] public int XpGained { get; set; }
			[JsonProperty("counters")] public string Counters { get; set; }
			[JsonProperty("matchstartdate")] public int MatchStartDate { get; set; }
			[JsonProperty("rank")] public int Rank { get; set; }
			[JsonProperty("ranktotal")] public int RankTotal { get; set; }
			[JsonProperty("alias")] public string Alias { get; set; }

		}

		public class Matchhistoryitem
		{
			[JsonProperty("profile_id")] public int ProfileId { get; set; }
			[JsonProperty("matchhistory_id")] public int MatchHistoryId { get; set; }
			[JsonProperty("iteminstance_id")] public int ItemInstanceId { get; set; }
			[JsonProperty("itemdefinition_id")] public int ItemDefinitionId { get; set; }
			[JsonProperty("durabilitytype")] public int DurabilityType { get; set; }
			[JsonProperty("durability")] public int Durability { get; set; }
			[JsonProperty("metadata")] public string MetaData { get; set; }
			[JsonProperty("itemlocation_id")] public int ItemLocationId { get; set; }
		}

		public class MatchHistoryStat
		{
			[JsonProperty("id")] public int Id { get; set; }
			[JsonProperty("creator_profile_id")] public int CreatorProfileId { get; set; }
			[JsonProperty("mapname")] public string MapName { get; set; }
			[JsonProperty("maxplayers")] public int MaxPlayers { get; set; }
			[JsonProperty("matchtype_id")] public int MatchTypeId { get; set; }
			[JsonProperty("options")] public string Options { get; set; }
			[JsonProperty("slotinfo")] public string SlotInfo { get; set; }
			[JsonProperty("description")] public string Description { get; set; }
			[JsonProperty("startgametime")] public int StartGameTime { get; set; }
			[JsonProperty("completiontime")] public int CompletionTime { get; set; }
			[JsonProperty("observertotal")] public int ObserverTotal { get; set; }
			[JsonProperty("matchhistoryreportresults")] public List<Matchhistoryreportresult> MatchHistoryReportResults { get; set; }
			[JsonProperty("matchhistoryitems")] public List<Matchhistoryitem> MatchHistoryItems { get; set; }
			[JsonProperty("matchurls")] public List<object> MatchUrls { get; set; }
		}

		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("matchHistoryStats")] public List<MatchHistoryStat> MatchHistoryStats { get; set; }
			[JsonProperty("profiles")] public List<PlayerIdentity> Profiles { get; set; }
		}

		public static Root RequestByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}
}
