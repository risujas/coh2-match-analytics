using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class JsonRecentMatchHistory
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

			public bool HasAxisVictory()
			{
				foreach (var x in MatchHistoryReportResults)
				{
					if (x.RaceId == (int)RaceId.German || x.RaceId == (int)RaceId.WGerman)
					{
						if (x.ResultType == 1)
						{
							return true;
						}

						else
						{
							return false;
						}
					}
				}

				throw new Exception("Invalid factions");
			}

			public bool HasGivenRaces(RaceFlag raceFlags)
			{
				bool requireGerman = raceFlags.HasFlag(RaceFlag.German);
				bool requireSoviet = raceFlags.HasFlag(RaceFlag.Soviet);
				bool requireWestGerman = raceFlags.HasFlag(RaceFlag.WGerman);
				bool requireAef = raceFlags.HasFlag(RaceFlag.AEF);
				bool requireBritish = raceFlags.HasFlag(RaceFlag.British);

				bool hasGerman = false;
				bool hasSoviet = false;
				bool hasWestGerman = false;
				bool hasAef = false;
				bool hasBritish = false;

				foreach (var x in MatchHistoryReportResults)
				{
					if (x.RaceId == (int)RaceId.German)
					{
						hasGerman = true;
					}
					if (x.RaceId == (int)RaceId.Soviet)
					{
						hasSoviet = true;
					}
					if (x.RaceId == (int)RaceId.WGerman)
					{
						hasWestGerman = true;
					}
					if (x.RaceId == (int)RaceId.AEF)
					{
						hasAef = true;
					}
					if (x.RaceId == (int)RaceId.British)
					{
						hasBritish = true;
					}
				}

				if (requireGerman && !hasGerman)
				{
					return false;
				}
				if (requireSoviet && !hasSoviet)
				{
					return false;
				}
				if (requireWestGerman && !hasWestGerman)
				{
					return false;
				}
				if (requireAef && !hasAef)
				{
					return false;
				}
				if (requireBritish && !hasBritish)
				{
					return false;
				}

				return true;
			}
		}

		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("matchHistoryStats")] public List<MatchHistoryStat> MatchHistoryStats { get; set; }
			[JsonProperty("profiles")] public List<PlayerIdentity> Profiles { get; set; }
		}

		public static Root GetBySteamId(string steamId)
		{
			List<string> list = new List<string> { steamId };
			return GetBySteamId(list);
		}

		public static Root GetBySteamId(List<string> steamIds)
		{
			string idString = "\"" + string.Join("\",\"", steamIds) + "\"";
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
			string requestParams = "?title=coh2&profile_names=[" + idString + "]";

			var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

			foreach (var x in response.Profiles)
			{
				PlayerIdentityTracker.LogPlayer(x);
			}

			int oldMatchCount = MatchHistoryTracker.GetNumLoggedMatches();
			foreach (var mhs in response.MatchHistoryStats)
			{
				MatchHistoryTracker.LogMatch(mhs);
			}
			int newMatchCount = MatchHistoryTracker.GetNumLoggedMatches();

			if (newMatchCount > oldMatchCount)
			{
				MatchHistoryTracker.WriteMatchData();
			}

			return response;
		}
	}
}
