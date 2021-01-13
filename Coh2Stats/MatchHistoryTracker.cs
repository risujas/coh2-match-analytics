using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Coh2Stats
{
	class MatchHistoryTracker
	{
		private static List<JsonRecentMatchHistory.MatchHistoryStat> matches = new List<JsonRecentMatchHistory.MatchHistoryStat>();
		public static ReadOnlyCollection<JsonRecentMatchHistory.MatchHistoryStat> Matches
		{
			get { return matches.AsReadOnly(); }
		}

		public static void LogMatch(JsonRecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.Id) == null)
			{
				matches.Add(matchHistoryStat);
			}
		}

		public static JsonRecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			foreach (var m in matches)
			{
				if (m.Id == id)
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
