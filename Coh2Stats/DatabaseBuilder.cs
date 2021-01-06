using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	enum GameMode
	{
		OneVsOne = 1,
		TwoVsTwo = 2,
		ThreeVsThree = 3,
		FourVsFour = 4
	}

	class DatabaseBuilder
	{
		public static void Build(GameMode gameMode)
		{
			BuildPlayerList(gameMode);
			PlayerIdentityTracker.SortPlayersByHighestRank();
			BuildMatchList(gameMode);
		}

		private static void BuildPlayerList(GameMode gameMode)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (gameMode == GameMode.OneVsOne)
				{
					if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
					{
						continue;
					}
				}

				if (gameMode == GameMode.TwoVsTwo)
				{
					if (leaderboardIndex != 8 && leaderboardIndex != 9 && leaderboardIndex != 10 && leaderboardIndex != 11 && leaderboardIndex != 52)
					{
						continue;
					}
				}

				if (gameMode == GameMode.ThreeVsThree)
				{
					if (leaderboardIndex != 12 && leaderboardIndex != 13 && leaderboardIndex != 14 && leaderboardIndex != 15 && leaderboardIndex != 53)
					{
						continue;
					}
				}

				if (gameMode == GameMode.FourVsFour)
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

		private static void BuildMatchList(GameMode gameMode)
		{
			for (int i = 0; i < PlayerIdentityTracker.GetNumLoggedPlayers(); i++)
			{
				var p = PlayerIdentityTracker.PlayerIdentities[i];
				Console.WriteLine("Fetching recent match history for {0} ({1})...", p.Name, p.Alias);
				RelicApi.JsonRecentMatchHistory.GetBySteamId(p.Name, (int)gameMode * 2);
			}
		}
	}
}
