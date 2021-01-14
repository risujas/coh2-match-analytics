using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

		public int GetHighestRank(Database db)
		{
			var stat = db.GetHighestStatByStatGroup(PersonalStatGroupId);
			if (stat == null)
			{
				return int.MaxValue;
			}

			return stat.Rank;
		}

		public void PrintPlayer()
		{
			Console.WriteLine(ProfileId + " " + Name + " " + PersonalStatGroupId + " " + Xp + " " + Level + " " + Country + " " + LeaderboardRegionId + " " + Alias);
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

	class Leaderboard
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

	class AvailableLeaderboards
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

		public static Root GetByProfileId(int profileId)
		{
			List<int> list = new List<int> { profileId };
			return GetByProfileId(list);
		}

		public static Root GetByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}

	class PersonalStat
	{
		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
			[JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
		}

		public static Root GetByProfileId(int profileId)
		{
			List<int> list = new List<int> { profileId };
			return GetByProfileId(list);
		}

		public static Root GetByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}

	class PlayerSummaries
	{
		public class Player
		{
			[JsonProperty("steamid")] public string SteamId { get; set; }
			[JsonProperty("communityvisibilitystate")] public int CommunityVisibilityState { get; set; }
			[JsonProperty("profilestate")] public int ProfileState { get; set; }
			[JsonProperty("personaname")] public string PersonaName { get; set; }
			[JsonProperty("commentpermission")] public int CommentPermission { get; set; }
			[JsonProperty("profileurl")] public string ProfileUrl { get; set; }
			[JsonProperty("avatar")] public string Avatar { get; set; }
			[JsonProperty("avatarmedium")] public string AvatarMedium { get; set; }
			[JsonProperty("avatarfull")] public string AvatarFull { get; set; }
			[JsonProperty("avatarhash")] public string AvatarHash { get; set; }
			[JsonProperty("personastate")] public int PersonaState { get; set; }
			[JsonProperty("primaryclanid")] public string PrimaryClanId { get; set; }
			[JsonProperty("timecreated")] public int TimeCreated { get; set; }
			[JsonProperty("personastateflags")] public int PersonaStateFlags { get; set; }
			[JsonProperty("loccountrycode")] public string LocCountryCode { get; set; }
		}

		public class Response
		{
			[JsonProperty("players")] public List<Player> Players { get; set; }
		}

		public class SteamResults
		{
			[JsonProperty("response")] public Response Response { get; set; }
		}

		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("avatars")] public List<PlayerIdentity> Avatars { get; set; }
			[JsonProperty("steamResults")] public SteamResults SteamResults { get; set; }
		}

		public static Root GetBySteamId(string steamId)
		{
			List<string> list = new List<string> { steamId };
			return GetBySteamId(list);
		}

		public static Root GetBySteamId(List<string> steamIds)
		{
			string idString = "\"" + string.Join("\",\"", steamIds) + "\"";
			string requestUrl = "https://coh2-api.reliclink.com/community/external/proxysteamuserrequest";
			string requestParams = "?request=/ISteamUser/GetPlayerSummaries/v0002/&title=coh2&profileNames=[" + idString + "]";

			var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

			if (response.Result.Message != "SUCCESS")
			{
				throw new Exception(response.Result.Message + ": " + idString);
			}

			return response;
		}
	}
}
