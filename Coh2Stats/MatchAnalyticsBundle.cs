using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class MatchAnalyticsBundle
	{
		public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> Matches = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		public static MatchAnalyticsBundle GetAllLoggedMatches(Database db)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();
			matchAnalyticsBundle.Matches = db.matchHistoryStats;
			return matchAnalyticsBundle;
		}

		public MatchAnalyticsBundle FilterByRace(RaceFlag raceFlags)
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

		public MatchAnalyticsBundle FilterByPartialPlayerNickname(Database db, string nickname)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				foreach (var rr in m.MatchHistoryReportResults)
				{
					string currentNick = db.GetPlayerByProfileId(rr.ProfileId).Alias;
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

		public MatchAnalyticsBundle FilterByMinimumHighRank(Database db, int minHighRank, bool requireOnAll)
		{
			MatchAnalyticsBundle matchAnalyticsBundle = new MatchAnalyticsBundle();

			foreach (var m in Matches)
			{
				int numGoodPlayers = 0;

				foreach (var rr in m.MatchHistoryReportResults)
				{
					var identity = db.GetPlayerByProfileId(rr.ProfileId);
					LeaderboardId lbid = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode((RaceId)rr.RaceId, (MatchTypeId)m.MatchTypeId);
					var lbs = db.GetStat(identity.PersonalStatGroupId, lbid);

					if (lbs == null)
					{
						continue;
					}

					if (lbs.Rank <= minHighRank && lbs.Rank > 0)
					{
						numGoodPlayers++;
					}
				}

				if (numGoodPlayers > 0)
				{
					if (!requireOnAll)
					{
						matchAnalyticsBundle.Matches.Add(m);
					}

					else if (requireOnAll && numGoodPlayers == m.MaxPlayers)
					{
						matchAnalyticsBundle.Matches.Add(m);
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
