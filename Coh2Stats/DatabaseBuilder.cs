using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		private const string playerList = "players.txt";
		List<PlayerIdentity> playerMatchHistoryQueue = new List<PlayerIdentity>();

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
		}

		public void ProcessMatches()
		{
			if (playerMatchHistoryQueue.Count == 0)
			{
				playerMatchHistoryQueue = PlayerIdentityTracker.PlayerIdentities.ToList();
			}

			int batchSize = 200;
			if (playerMatchHistoryQueue.Count < batchSize)
			{
				batchSize = playerMatchHistoryQueue.Count;
			}

			var range = playerMatchHistoryQueue.GetRange(0, batchSize);
			playerMatchHistoryQueue.RemoveRange(0, batchSize);

			List<string> steamIds = new List<string>();
			foreach (var p in playerMatchHistoryQueue)
			{
				steamIds.Add(p.Name);
			}


			Console.WriteLine("Getting match history for {0} players", batchSize);
			JsonRecentMatchHistory.GetByProfileId(steamIds);
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
	}
}
