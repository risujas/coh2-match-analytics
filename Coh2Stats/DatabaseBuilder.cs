using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		private const string playerList = "players.txt";

		public void Build(MatchTypeId gameMode)
		{
			MatchHistoryTracker.LoadMatchData();

			if (!PlayerIdentityTracker.LoadPlayerList(60))
			{
				ParseLeaderboards(gameMode, 1, -1);
				PlayerIdentityTracker.WritePlayerList();
			}

			FetchPlayerDetails();
			PlayerIdentityTracker.SortPlayersByHighestRank();
			BuildMatchList(20);
		}

		private void ParseLeaderboards(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
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
		}


		// Side effect: this function will discover new players from arranged teams and add them to PlayerIdentityTracker. Detailed information isn't retrieved for these players because:
		// 1. these players didn't match the original query and are thus not relevant
		// 2. retrieving detailed information would in turn cause more players to be discovered, creating an ever growing database of irrelevant players
		// Because of all this, PlayerIdentityTracker will contain some players who don't have any corresponding data in LeaderboardStatTracker. It is therefore necessary to prepare for
		// certain methods to return null references when "irrelevant" players are queried.
		private void FetchPlayerDetails()
		{
			var players = PlayerIdentityTracker.PlayerIdentities.ToList();
			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					Console.WriteLine("Fetching player summaries, {0} remaining", players.Count);

					var range = players.GetRange(0, batchSize);
					players.RemoveRange(0, batchSize);

					List<int> profileIds = new List<int>();
					foreach (var p in range)
					{
						profileIds.Add(p.ProfileId);
					}

					JsonPersonalStat.GetByProfileId(profileIds);
				}

				else
				{
					batchSize = players.Count;
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

				Console.WriteLine("{0}/{1} Fetched recent match history for {2} ({3})", i, max, p.Name, p.Alias);
			}
		}
	}
}
