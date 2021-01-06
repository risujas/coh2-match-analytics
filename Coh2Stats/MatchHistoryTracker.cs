﻿using System;
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

		public static void BuildDatabase()
		{
			BuildPlayerList(2);
			PlayerIdentityTracker.SortPlayersByHighestRank();
			Build1v1MatchList();
		}

		private static void BuildPlayerList(int maxPlayers)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (maxPlayers == 2)
				{
					if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
					{
						continue;
					}
				}

				if (maxPlayers == 4)
				{
					if (leaderboardIndex != 8 && leaderboardIndex != 9 && leaderboardIndex != 10 && leaderboardIndex != 11 && leaderboardIndex != 52)
					{
						continue;
					}
				}

				if (maxPlayers == 6)
				{
					if (leaderboardIndex != 12 && leaderboardIndex != 13 && leaderboardIndex != 14 && leaderboardIndex != 15 && leaderboardIndex != 53)
					{
						continue;
					}
				}

				if (maxPlayers == 8)
				{
					if (leaderboardIndex != 16 && leaderboardIndex != 17 && leaderboardIndex != 18 && leaderboardIndex != 19 && leaderboardIndex != 54)
					{
						continue;
					}
				}

				var probeResponse = RelicApi.JsonLeaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.rankTotal;
				int batchStartingIndex = 1;

				while (batchStartingIndex < leaderboardMaxRank)
				{
					int difference = leaderboardMaxRank - batchStartingIndex;
					int batchSize = 200;

					if (difference < 200)
					{
						batchSize = difference + 1;
					}

					RelicApi.JsonLeaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);
					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, PlayerIdentityTracker.GetNumLoggedPlayers());
					batchStartingIndex += batchSize;
				}
			}
		}

		private static void Build1v1MatchList()
		{
			for (int i = 0; i < PlayerIdentityTracker.GetNumLoggedPlayers(); i++)
			{
				var p = PlayerIdentityTracker.PlayerIdentities[i];
				Console.WriteLine("Fetching recent match history for {0} ({1})...", p.Name, p.Alias);
				RelicApi.JsonRecentMatchHistory.GetBySteamId(p.Name, 2);
			}
		}
	}
}
