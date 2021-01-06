using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchHistoryTracker
	{
		private static List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat> Matches = new List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat>();

		public static void LogMatch(RelicApi.JsonRecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.id) == null)
			{
				Matches.Add(matchHistoryStat);
			}
		}

		public static RelicApi.JsonRecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			foreach (var m in Matches)
			{
				if (m.id == id)
				{
					return m;
				}
			}

			return null;
		}

		public static int GetNumLoggedMatches()
		{
			return Matches.Count;
		}

		public static void BuildDatabase()
		{
			Build1v1PlayerList();
			PlayerIdentityTracker.SortPlayersByHighestRank();
			Build1v1MatchList();
		}

		private static void Build1v1PlayerList()
		{
			for (int i = 4; i < 52; i++)
			{
				if (i != 4 && i != 5 && i != 6 && i != 7 && i != 51)
				{
					continue;
				}

				var probe = RelicApi.JsonLeaderboard.GetById(i, 1, 1);
				int rankedTotal = probe.rankTotal;
				int index = 1;

				while (index < rankedTotal)
				{
					int difference = rankedTotal - index;
					int batchSize = 200;

					if (difference < 200)
					{
						batchSize = difference;
					}

					RelicApi.JsonLeaderboard.GetById(i, index, batchSize);
					index += batchSize;
				}
			}
		}

		private static void Build1v1MatchList()
		{

		}
	}
}
