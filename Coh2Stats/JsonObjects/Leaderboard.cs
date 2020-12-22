﻿#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class Leaderboard
		{
			public class Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public struct Member
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
				public int rankTotal { get; set; }
			}

			public static Root GetById(int leaderboardId, int startRank, int numRanks)
			{
				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/getLeaderBoard2";
				string requestParams = "?title=coh2&leaderboard_id=" + leaderboardId.ToString() + "&start=" + startRank.ToString() + "&count=" + numRanks.ToString();

				return WebUtils.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
			}
		}
	}
}
