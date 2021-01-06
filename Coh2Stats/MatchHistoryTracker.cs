using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	[Flags] enum RaceId
	{
		German = 0,
		Soviet = 1,
		WGerman = 2,
		AEF = 3,
		British = 4
	}

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

		public static Dictionary<string, int> GetMapPopularityDictionary(RaceId raceFlags)
		{
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

			foreach (var m in matches)
			{
				if (m.HasGivenRaces(raceFlags))
				{
					if (!keyValuePairs.ContainsKey(m.mapname))
					{
						keyValuePairs.Add(m.mapname, 0);
					}

					keyValuePairs[m.mapname] += 1;
				}
			}

			keyValuePairs = keyValuePairs.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
			keyValuePairs = keyValuePairs.Reverse().ToDictionary(p => p.Key, p => p.Value);

			return keyValuePairs;
		}
	}
}
