using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchHistoryTracker
	{
		private static List<RelicApi.RecentMatchHistory.MatchHistoryStat> Matches = new List<RelicApi.RecentMatchHistory.MatchHistoryStat>();

		public static void LogMatch(RelicApi.RecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.id) == null)
			{
				Matches.Add(matchHistoryStat);
			}
		}

		public static RelicApi.RecentMatchHistory.MatchHistoryStat GetById(int id)
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
	}
}
