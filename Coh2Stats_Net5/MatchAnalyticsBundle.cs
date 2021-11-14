using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	public class MatchAnalyticsBundle
	{
		public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> Matches = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		public static MatchAnalyticsBundle GetAllLoggedMatches()
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();
			matchAnalyticsBundle.Matches = DatabaseHandler.MatchDb.MatchData;
			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByRequiredRaces(RaceFlag raceFlags)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.HasRequiredRaces(raceFlags))
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByAllowedRaces(RaceFlag raceFlags)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.HasAllowedRaces(raceFlags))
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByResult(bool result, FactionId factionId)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (factionId == FactionId.Axis && m.HasAxisVictory() == result)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}

				else if (factionId == FactionId.Allies && m.HasAxisVictory() != result)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByStartGameTime(int startGameTimeBegin, int startGameTimeEnd)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			if (startGameTimeBegin == -1)
			{
				startGameTimeBegin = 0;
			}

			if (startGameTimeEnd == -1)
			{
				startGameTimeEnd = int.MaxValue;
			}

			foreach (var m in Matches)
			{
				if (m.StartGameTime >= startGameTimeBegin && m.StartGameTime <= startGameTimeEnd)
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
				long cutoffTimeSeconds = dto.ToUnixTimeSeconds();

				if (m.CompletionTime > cutoffTimeSeconds)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByPercentile(double low, double high, bool requireOnAll)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			for (int i = 0; i < Matches.Count; i++)
			{
				var match = Matches[i];
				int numValidPlayers = 0;

				for (int j = 0; j < match.MatchHistoryReportResults.Count; j++)
				{
					var report = match.MatchHistoryReportResults[j];
					LeaderboardId ldb = LeaderboardCompatibility.GetLeaderboardByRace((RaceId)report.RaceId);

					if (report.Rank < 1)
					{
						if (requireOnAll)
						{
							break;
						}

						continue;
					}

					int lowcutoffInclusive = DatabaseHandler.PlayerDb.GetLeaderboardRankByPercentile(ldb, low);
					int highCutoffExclusive = DatabaseHandler.PlayerDb.GetLeaderboardRankByPercentile(ldb, high);

					if (report.Rank >= lowcutoffInclusive && report.Rank < highCutoffExclusive)
					{
						numValidPlayers++;
					}
					else if (requireOnAll)
					{
						break;
					}
				}

				if (numValidPlayers > 0)
				{
					if (!requireOnAll)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}

					else if (requireOnAll && numValidPlayers == match.MaxPlayers)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByRank(int low, int high, bool requireOnAll)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			for (int i = 0; i < Matches.Count; i++)
			{
				var match = Matches[i];
				int numValidPlayers = 0;

				for (int j = 0; j < match.MatchHistoryReportResults.Count; j++)
				{
					var report = match.MatchHistoryReportResults[j];
					LeaderboardId ldb = LeaderboardCompatibility.GetLeaderboardByRace((RaceId)report.RaceId);

					if (report.Rank < 1)
					{
						if (requireOnAll)
						{
							break;
						}

						continue;
					}

					if (report.Rank >= low && report.Rank <= high)
					{
						numValidPlayers++;
					}
					else if (requireOnAll)
					{
						break;
					}
				}

				if (numValidPlayers > 0)
				{
					if (!requireOnAll)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}

					else if (requireOnAll && numValidPlayers == match.MaxPlayers)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}
				}
			}

			return matchAnalyticsBundle;
		}

		public Dictionary<string, int> GetOrderedMapPlayCount()
		{
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

			foreach (var m in Matches)
			{
				if (!keyValuePairs.ContainsKey(m.MapName))
				{
					keyValuePairs.Add(m.MapName, 0);
				}

				keyValuePairs[m.MapName] += 1;
			}

			keyValuePairs = keyValuePairs.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
			keyValuePairs = keyValuePairs.Reverse().ToDictionary(p => p.Key, p => p.Value);

			return keyValuePairs;
		}
	}
}
