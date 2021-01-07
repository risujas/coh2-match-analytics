using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchAnalyticsBundle
	{
		public List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat> Matches = new List<RelicApi.JsonRecentMatchHistory.MatchHistoryStat>();

		public static MatchAnalyticsBundle GetAllLoggedMatches()
		{
			MatchAnalyticsBundle matchBundle = new MatchAnalyticsBundle();
			matchBundle.Matches = MatchHistoryTracker.Matches.ToList();
			return matchBundle;
		}

		public MatchAnalyticsBundle FilterByRace(RelicApi.RaceFlag raceFlags)
		{
			MatchAnalyticsBundle matchBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.HasGivenRaces(raceFlags))
				{
					matchBundle.Matches.Add(m);
				}
			}

			return matchBundle;
		}

		public MatchAnalyticsBundle FilterByMap(string mapName)
		{
			MatchAnalyticsBundle matchBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.mapname == mapName)
				{
					matchBundle.Matches.Add(m);
				}
			}

			return matchBundle;
		}

		// OrderMapsByRaceWinRate

		public Dictionary<string, int> GetOrderedPlayCount()
		{
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

			foreach (var m in Matches)
			{
				if (!keyValuePairs.ContainsKey(m.mapname))
				{
					keyValuePairs.Add(m.mapname, 0);
				}

				keyValuePairs[m.mapname] += 1;
			}

			keyValuePairs = keyValuePairs.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
			keyValuePairs = keyValuePairs.Reverse().ToDictionary(p => p.Key, p => p.Value);

			return keyValuePairs;
		}
	}
}
