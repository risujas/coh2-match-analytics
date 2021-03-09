using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	internal class MatchAnalyticsBundle
	{
		public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> Matches = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		public static MatchAnalyticsBundle GetAllLoggedMatches(DatabaseHandler db)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();
			matchAnalyticsBundle.Matches = db.MatchDb.MatchData;
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

		public MatchAnalyticsBundle FilterByMap(string mapName)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.MapName == mapName)
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

		public MatchAnalyticsBundle FilterByCompletionTime(int completionTimeBegin, int completionTimeEnd)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.CompletionTime >= completionTimeBegin && m.CompletionTime <= completionTimeEnd)
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
				long cutoffTime = dto.ToUnixTimeSeconds();

				if (m.CompletionTime > cutoffTime)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByDescription(string description)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (description == m.Description)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByPartialPlayerNickname(DatabaseHandler db, string nickname)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				foreach (var rr in m.MatchHistoryReportResults)
				{
					string currentNick = db.PlayerDb.GetPlayerByProfileId(rr.ProfileId).Alias;
					if (currentNick.ToLower().Contains(nickname.ToLower()))
					{
						matchAnalyticsBundle.Matches.Add(m);
					}
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByMatchType(MatchTypeId matchTypeId)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.MatchTypeId == (int)matchTypeId)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByMinObserverCount(int minObserverCount)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				if (m.ObserverTotal >= minObserverCount)
				{
					matchAnalyticsBundle.Matches.Add(m);
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByTopPercentile(DatabaseHandler db, double topPercentile, bool requireOnAll)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			for (int i = 0; i < Matches.Count; i++)
			{
				var match = Matches[i];

				int numGoodPlayers = 0;

				for (int j = 0; j < match.MatchHistoryReportResults.Count; j++)
				{
					var report = match.MatchHistoryReportResults[j];

					var identity = db.PlayerDb.GetPlayerByProfileId(report.ProfileId);
					LeaderboardId lbid = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode((RaceId)report.RaceId, (MatchTypeId)match.MatchTypeId);
					var lbs = db.PlayerDb.GetStat(identity.PersonalStatGroupId, lbid);

					if (lbs == null)
					{
						if (requireOnAll)
						{
							break;
						}

						continue;
					}

					int cutoffRank = db.PlayerDb.GetLeaderboardRankByPercentile(lbid, topPercentile);
					if (lbs.Rank <= cutoffRank)
					{
						numGoodPlayers++;
					}

					else if (requireOnAll)
					{
						break;
					}
				}

				if (numGoodPlayers > 0)
				{
					if (!requireOnAll)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}

					else if (requireOnAll && numGoodPlayers == match.MaxPlayers)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByRank(DatabaseHandler db, int rankFloor, int rankCap, bool requireOnAll)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			for (int i = 0; i < Matches.Count; i++)
			{
				var match = Matches[i];

				int numGoodPlayers = 0;

				for (int j = 0; j < match.MatchHistoryReportResults.Count; j++)
				{
					var report = match.MatchHistoryReportResults[j];

					var identity = db.PlayerDb.GetPlayerByProfileId(report.ProfileId);
					LeaderboardId lbid = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode((RaceId)report.RaceId, (MatchTypeId)match.MatchTypeId);
					var lbs = db.PlayerDb.GetStat(identity.PersonalStatGroupId, lbid);

					if (lbs == null)
					{
						if (requireOnAll)
						{
							break;
						}

						continue;
					}

					if (lbs.Rank <= rankCap && lbs.Rank >= rankFloor)
					{
						numGoodPlayers++;
					}

					else if (requireOnAll)
					{
						break;
					}
				}

				if (numGoodPlayers > 0)
				{
					if (!requireOnAll)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}

					else if (requireOnAll && numGoodPlayers == match.MaxPlayers)
					{
						matchAnalyticsBundle.Matches.Add(match);
					}
				}
			}

			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByCommander(CommanderServerId commanderServerId)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				int id = (int)commanderServerId;
				if (m.HasMatchHistoryItem(id))
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
