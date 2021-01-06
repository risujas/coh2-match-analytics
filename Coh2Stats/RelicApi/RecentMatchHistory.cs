#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class RecentMatchHistory
		{
			public struct Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public struct Matchhistoryreportresult
			{
				public int matchhistory_id { get; set; }
				public int profile_id { get; set; }
				public int resulttype { get; set; }
				public int teamid { get; set; }
				public int race_id { get; set; }
				public int xpgained { get; set; }
				public string counters { get; set; }
				public int matchstartdate { get; set; }
			}

			public struct Matchhistoryitem
			{
				public int profile_id { get; set; }
				public int matchhistory_id { get; set; }
				public int iteminstance_id { get; set; }
				public int itemdefinition_id { get; set; }
				public int durabilitytype { get; set; }
				public int durability { get; set; }
				public string metadata { get; set; }
				public int itemlocation_id { get; set; }
			}

			public struct MatchHistoryStat
			{
				public int id { get; set; }
				public int creator_profile_id { get; set; }
				public string mapname { get; set; }
				public int maxplayers { get; set; }
				public int matchtype_id { get; set; }
				public string options { get; set; }
				public string slotinfo { get; set; }
				public string description { get; set; }
				public int startgametime { get; set; }
				public int completiontime { get; set; }
				public int observertotal { get; set; }
				public List<Matchhistoryreportresult> matchhistoryreportresults { get; set; }
				public List<Matchhistoryitem> matchhistoryitems { get; set; }
				public List<object> matchurls { get; set; }
			}

			public struct Profile
			{
				public int profile_id { get; set; }
				public string name { get; set; }
				public string alias { get; set; }
				public int personal_statgroup_id { get; set; }
				public int xp { get; set; }
				public int level { get; set; }
				public int leaderboardregion_id { get; set; }
				public string country { get; set; }
			}

			public struct Root
			{
				public Result result { get; set; }
				public List<MatchHistoryStat> matchHistoryStats { get; set; }
				public List<Profile> profiles { get; set; }
			}

			public static Root GetBySteamId(string steamId)
			{
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
				string requestParams = "?title=coh2&profile_names=[\"/steam/" + steamId + "\"]";

				var response = Utilities.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				foreach (var p in response.profiles)
				{
					PlayerIdentity pi = new PlayerIdentity();
					pi.SteamId = p.name.Substring(p.name.LastIndexOf('/') + 1);
					pi.Alias = p.alias;
					pi.ProfileId = p.profile_id.ToString();
					PlayerIdentityTracker.LogPlayer(pi);
				}

				return response;
			}
		}
	}
}
