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

		public static Dictionary<string, int> GetMapPopularityDictionary()
		{
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

			foreach (var m in MatchHistoryTracker.Matches)
			{
				if (!keyValuePairs.ContainsKey(m.mapname))
				{
					keyValuePairs.Add(m.mapname, 0);
				}

				keyValuePairs[m.mapname] += 1;
			}

			keyValuePairs = keyValuePairs.OrderBy(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
			keyValuePairs = keyValuePairs.Reverse().ToDictionary(k => k.Key, k => k.Value);

			return keyValuePairs;
		}
	}
}
