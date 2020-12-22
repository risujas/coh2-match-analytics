#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class Leaderboard
		{
			public struct Result
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

			public struct StatGroup
			{
				public int id { get; set; }
				public string name { get; set; }
				public int type { get; set; }
				public List<Member> members { get; set; }
			}

			public struct LeaderboardStat
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

			public struct Root
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

			public static int FindEquivalentRankForLevel(int leaderboardId, int level)
			{
				int equivalentRank = 0;
				var response = GetById(leaderboardId, 1, 10);
				int maxRank = response.rankTotal;

				if (level == 20)
				{
					equivalentRank = 1;
				}

				if (level == 19)
				{
					equivalentRank = 3;
				}

				if (level == 18)
				{
					equivalentRank = 14;
				}

				if (level == 17)
				{
					equivalentRank = 37;
				}

				if (level == 16)
				{
					equivalentRank = 81;
				}

				if (level == 15) 
				{
					equivalentRank = 201; // depends
				}

				if (level == 14)
				{
					equivalentRank = 251; // depends
				}

				if (level == 13)
				{
					equivalentRank = (int)(maxRank * 0.1) + 1;
				}

				if (level == 12)
				{
					equivalentRank = (int)(maxRank * 0.15) + 1;
				}

				if (level == 11)
				{
					equivalentRank = (int)(maxRank * 0.2) + 1;
				}

				if (level == 10)
				{
					equivalentRank = (int)(maxRank * 0.25) + 1;
				}

				if (level == 9)
				{
					equivalentRank = (int)(maxRank * 0.31) + 1;
				}

				if (level == 8)
				{
					equivalentRank = (int)(maxRank * 0.38) + 1;
				}

				if (level == 7)
				{
					equivalentRank = (int)(maxRank * 0.45) + 1;
				}

				if (level == 6)
				{
					equivalentRank = (int)(maxRank * 0.55) + 1;
				}

				if (level == 5)
				{
					equivalentRank = (int)(maxRank * 0.65) + 1;
				}

				if (level == 4)
				{
					equivalentRank = (int)(maxRank * 0.75) + 1;
				}

				if (level == 3)
				{
					equivalentRank = (int)(maxRank * 0.8) + 1;
				}

				if (level == 2)
				{
					equivalentRank = (int)(maxRank * 0.86) + 1;
				}

				if (level == 1)
				{
					equivalentRank = (int)(maxRank * 0.94) + 1;
				}

				Console.WriteLine("Equivalent rank for level {0} on leaderboard {1} is roughly: {2}", level, leaderboardId, equivalentRank);

				return equivalentRank;
			}
		}
	}
}
