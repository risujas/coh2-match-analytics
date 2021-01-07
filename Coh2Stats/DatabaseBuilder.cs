using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		public void Build(RelicApi.GameModeId gameMode)
		{
			BuildPlayerList(gameMode);
			PlayerIdentityTracker.SortPlayersByHighestRank();
			BuildMatchList(gameMode);
		}

		private void BuildPlayerList(RelicApi.GameModeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (gameMode == RelicApi.GameModeId.OneVsOne)
				{
					if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.GameModeId.TwoVsTwo)
				{
					if (leaderboardIndex != 8 && leaderboardIndex != 9 && leaderboardIndex != 10 && leaderboardIndex != 11 && leaderboardIndex != 52)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.GameModeId.ThreeVsThree)
				{
					if (leaderboardIndex != 12 && leaderboardIndex != 13 && leaderboardIndex != 14 && leaderboardIndex != 15 && leaderboardIndex != 53)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.GameModeId.FourVsFour)
				{
					if (leaderboardIndex != 16 && leaderboardIndex != 17 && leaderboardIndex != 18 && leaderboardIndex != 19 && leaderboardIndex != 54)
					{
						continue;
					}
				}

				var probeResponse = RelicApi.JsonLeaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.rankTotal;
				int batchStartingIndex = startingRank;

				if (maxRank != -1)
				{
					leaderboardMaxRank = maxRank;
				}

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

		private void BuildMatchList(RelicApi.GameModeId gameMode)
		{
			for (int i = 0; i < PlayerIdentityTracker.GetNumLoggedPlayers(); i++)
			{
				int maxPlayers = 0;
				switch (gameMode)
				{
					case RelicApi.GameModeId.OneVsOne:
						maxPlayers = 2;
						break;
					case RelicApi.GameModeId.TwoVsTwo:
						maxPlayers = 4;
						break;
					case RelicApi.GameModeId.ThreeVsThree:
						maxPlayers = 6;
						break;
					case RelicApi.GameModeId.FourVsFour:
						maxPlayers = 8;
						break;
					default:
						throw new Exception("Invalid value for " + maxPlayers);
				}

				var p = PlayerIdentityTracker.PlayerIdentities[i];
				RelicApi.JsonRecentMatchHistory.GetBySteamId(p.Name, maxPlayers);
			}
		}
	}
}
