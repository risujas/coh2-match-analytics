using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchHistoryTracker
	{
		private static List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat> matches = new List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat>();
		public static ReadOnlyCollection<RelicApi.JsonRecentMatchHistory.MatchHistoryStat> Matches
		{
			get { return matches.AsReadOnly(); }
		}

		public static void LogMatch(RelicApi.JsonRecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.id) == null)
			{
				matches.Add(matchHistoryStat);
			}
		}

		public static RelicApi.JsonRecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			foreach (var m in matches)
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
			return matches.Count;
		}
	}
}
