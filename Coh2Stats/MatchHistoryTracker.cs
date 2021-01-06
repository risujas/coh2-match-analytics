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
	}
}
