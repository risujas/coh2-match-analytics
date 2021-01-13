using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		public void Build(MatchTypeId gameMode)
		{
			BuildPlayerList(gameMode, 1, -1);
			PlayerIdentityTracker.SortPlayersByHighestRank();

			BuildMatchList(20);
		}

		private void BuildPlayerList(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, matchTypeId) == false)
				{
					continue;
				}

				var probeResponse = JsonLeaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
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

					JsonLeaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);
					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, PlayerIdentityTracker.GetNumLoggedPlayers());
					batchStartingIndex += batchSize;
				}
			}

			var players = PlayerIdentityTracker.PlayerIdentities.ToList();
			int magic = 200;
			while (players.Count > 0)
			{
				Console.WriteLine(players.Count);

				if (players.Count >= magic)
				{
					var range = players.GetRange(0, magic);
					players.RemoveRange(0, magic);

					List<int> profileIds = new List<int>();
					foreach (var p in range)
					{
						profileIds.Add(p.ProfileId);
					}

					JsonPersonalStat.GetByProfileId(profileIds);
				}

				else
				{
					magic = players.Count;
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
				JsonRecentMatchHistory.GetBySteamId(p.Name);

				Console.WriteLine("Fetched recent match history for {0} ({1})", p.Name, p.Alias);
			}
		}
	}
}
