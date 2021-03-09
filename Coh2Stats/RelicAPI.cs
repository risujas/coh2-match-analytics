using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Coh2Stats.RelicAPI
{
	public class PlayerIdentity
	{
		[JsonProperty("profile_id")] public int ProfileId { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("alias")] public string Alias { get; set; }
		[JsonProperty("personal_statgroup_id")] public int PersonalStatGroupId { get; set; }
		[JsonProperty("xp")] public int Xp { get; set; }
		[JsonProperty("level")] public int Level { get; set; }
		[JsonProperty("leaderboardregion_id")] public int LeaderboardRegionId { get; set; }
		[JsonProperty("country")] public string Country { get; set; }

		public int GetHighestRank(PlayerDatabase pdb, MatchTypeId matchTypeId)
		{
			var stat = pdb.GetHighestStatByStatGroup(PersonalStatGroupId, matchTypeId);

			if (stat == null)
			{
				return int.MaxValue;
			}

			if (stat.Rank < 1)
			{
				return int.MaxValue;
			}

			return stat.Rank;
		}
	}

	public class StatGroup
	{
		[JsonProperty("id")] public int Id { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("type")] public int Type { get; set; }
		[JsonProperty("members")] public List<PlayerIdentity> Members { get; set; }
	}

	public class LeaderboardStat
	{
		[JsonProperty("statGroup_id")] public int StatGroupId { get; set; }
		[JsonProperty("leaderboard_id")] public int LeaderboardId { get; set; }
		[JsonProperty("wins")] public int Wins { get; set; }
		[JsonProperty("losses")] public int Losses { get; set; }
		[JsonProperty("streak")] public int Streak { get; set; }
		[JsonProperty("disputes")] public int Disputes { get; set; }
		[JsonProperty("drops")] public int Drops { get; set; }
		[JsonProperty("rank")] public int Rank { get; set; }
		[JsonProperty("rankTotal")] public int RankTotal { get; set; }
		[JsonProperty("regionRank")] public int RegionRank { get; set; }
		[JsonProperty("regionRankTotal")] public int RegionRankTotal { get; set; }
		[JsonProperty("rankLevel")] public int RankLevel { get; set; }
		[JsonProperty("lastMatchDate")] public int LastMatchDate { get; set; }
	}

	internal class Leaderboard
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

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}

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

			public bool HasMatchHistoryItem(int id)
			{
				for (int i = 0; i < MatchHistoryItems.Count; i++)
				{
					var x = MatchHistoryItems[i];
					if (x.ItemDefinitionId == id)
					{
						return true;
					}
				}
				return false;
			}

			public bool HasAxisVictory()
			{
				for (int i = 0; i < MatchHistoryReportResults.Count; i++)
				{
					var x = MatchHistoryReportResults[i];
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

			public bool HasRequiredRaces(RaceFlag raceFlags)
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

				for (int i = 0; i < MatchHistoryReportResults.Count; i++)
				{
					var x = MatchHistoryReportResults[i];
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

			public bool HasAllowedRaces(RaceFlag raceFlags)
			{
				bool allowGerman = raceFlags.HasFlag(RaceFlag.German);
				bool allowSoviet = raceFlags.HasFlag(RaceFlag.Soviet);
				bool allowWestGerman = raceFlags.HasFlag(RaceFlag.WGerman);
				bool allowAef = raceFlags.HasFlag(RaceFlag.AEF);
				bool allowBritish = raceFlags.HasFlag(RaceFlag.British);

				bool hasGerman = false;
				bool hasSoviet = false;
				bool hasWestGerman = false;
				bool hasAef = false;
				bool hasBritish = false;

				for (int i = 0; i < MatchHistoryReportResults.Count; i++)
				{
					var x = MatchHistoryReportResults[i];
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

				if (!allowGerman && hasGerman)
				{
					return false;
				}
				if (!allowSoviet && hasSoviet)
				{
					return false;
				}
				if (!allowWestGerman && hasWestGerman)
				{
					return false;
				}
				if (!allowAef && hasAef)
				{
					return false;
				}
				if (!allowBritish && hasBritish)
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

		public static Root GetByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}

	internal class PersonalStat
	{
		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
			[JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
		}

		public static Root GetByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}
}
