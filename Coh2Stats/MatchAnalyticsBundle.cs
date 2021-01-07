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
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();
			matchAnalyticsBundle.Matches = MatchHistoryTracker.Matches.ToList();
			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByRace(RelicApi.RaceFlag raceFlags)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.HasGivenRaces(raceFlags))
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByMap(string mapName)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.mapname == mapName)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByResult(bool result, RelicApi.FactionId factionId)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (factionId == RelicApi.FactionId.Axis && m.HasAxisVictory() == result)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}

				else if (factionId == RelicApi.FactionId.Allies && m.HasAxisVictory() != result)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}	

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByCompletionTime(int completionTimeBegin, int completionTimeEnd)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.completiontime >= completionTimeBegin && m.completiontime <= completionTimeEnd)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByStartGameTime(int startGameTimeBegin, int startGameTimeEnd)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.startgametime >= startGameTimeBegin && m.startgametime <= startGameTimeEnd)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByMaxAgeInHours(int maxAgeInHours)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				DateTime dt = DateTime.UtcNow;
				DateTimeOffset dto = new DateTimeOffset(dt).AddHours(-maxAgeInHours);
				long cutoffTime = dto.ToUnixTimeSeconds();

				if (m.completiontime > cutoffTime)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public Dictionary<string, int> GetOrderedMapPlayCount()
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
