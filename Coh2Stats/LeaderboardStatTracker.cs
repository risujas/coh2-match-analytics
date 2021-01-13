using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Coh2Stats
{
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

	class LeaderboardStatTracker
	{
		private static List<LeaderboardStat> leaderboardStats = new List<LeaderboardStat>();
		public static ReadOnlyCollection<LeaderboardStat> LeaderBoardStats
		{
			get { return leaderboardStats.AsReadOnly(); }
		}

		public static LeaderboardStat GetHighestStatByStatGroup(int statGroupId)
		{
			LeaderboardStat highest = null;

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					if (highest == null)
					{
						highest = x;
					}

					else if (x.Rank < highest.Rank)
					{
						highest = x;
					}
				}
			}

			return highest;
		}

		public static LeaderboardStat GetLowestStatByStatGroup(int statGroupId)
		{
			LeaderboardStat lowest = null;

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					if (lowest == null)
					{
						lowest = x;
					}

					else if (x.Rank > lowest.Rank)
					{
						lowest = x;
					}
				}
			}

			return lowest;
		}

		public static LeaderboardStat GetStat(int statGroupId, LeaderboardId leaderboardId)
		{
			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId && x.LeaderboardId == (int)leaderboardId)
				{
					return x;
				}
			}

			return null;
		}

		public static List<LeaderboardStat> GetAllStatsByStatGroup(int statGroupId)
		{
			List<LeaderboardStat> stats = new List<LeaderboardStat>();

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					stats.Add(x);
				}
			}

			return stats;
		}

		public static void LogStat(LeaderboardStat stat)
		{
			if (GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId) == null)
			{
				leaderboardStats.Add(stat);
			}
		}
	}
}
