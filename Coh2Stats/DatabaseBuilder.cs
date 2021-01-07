using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		public void Build(RelicApi.MatchTypeId gameMode)
		{
			BuildPlayerList(gameMode, 1, 200);
			PlayerIdentityTracker.SortPlayersByHighestRank();
			BuildMatchList(5);
		}

		private void BuildPlayerList(RelicApi.MatchTypeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (gameMode == RelicApi.MatchTypeId.Auto1v1)
				{
					if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.MatchTypeId.Auto2v2)
				{
					if (leaderboardIndex != 8 && leaderboardIndex != 9 && leaderboardIndex != 10 && leaderboardIndex != 11 && leaderboardIndex != 52)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.MatchTypeId.Auto3v3)
				{
					if (leaderboardIndex != 12 && leaderboardIndex != 13 && leaderboardIndex != 14 && leaderboardIndex != 15 && leaderboardIndex != 53)
					{
						continue;
					}
				}

				if (gameMode == RelicApi.MatchTypeId.Auto4v4)
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

		private void BuildMatchList(int maxPlayersProcessed = -1)
		{
			int max = PlayerIdentityTracker.GetNumLoggedPlayers();
			if (maxPlayersProcessed != -1)
			{
				max = maxPlayersProcessed;
			}

			for (int i = 0; i < max; i++)
			{
				var p = PlayerIdentityTracker.PlayerIdentities[i];
				RelicApi.JsonRecentMatchHistory.GetBySteamId(p.Name);

				Console.WriteLine("Fetched recent match history for {0} ({1})", p.Name, p.Alias);
			}
		}
	}
}
