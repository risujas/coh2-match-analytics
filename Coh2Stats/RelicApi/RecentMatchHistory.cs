#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class RecentMatchHistory
		{
			public class Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public class Matchhistoryreportresult
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

			public class Matchhistoryitem
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

			public class MatchHistoryStat
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

			public class Profile: PlayerIdentity
			{
			}

			public class Root
			{
				public Result result { get; set; }
				public List<MatchHistoryStat> matchHistoryStats { get; set; }
				public List<Profile> profiles { get; set; }

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
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getRecentMatchHistory";
				string requestParams = "?title=coh2&profile_names=[" + idString + "]";

				var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				foreach (var x in response.profiles)
				{
					PlayerIdentityTracker.LogPlayer(new PlayerIdentity(x));
				}

				foreach (var mhs in response.matchHistoryStats)
				{
					MatchHistoryTracker.LogMatch(mhs);
				}

				return response;
			}
		}
	}
}
