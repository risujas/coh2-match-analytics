using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coh2Stats
{
	public class DatabaseHandler
	{
		public readonly PlayerDatabase PlayerDb = new PlayerDatabase();
		public readonly MatchDatabase MatchDb = new MatchDatabase();

		public static readonly string ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";
		public static readonly string DatabaseFolder = ApplicationDataFolder + "\\databases";

		private Dictionary<LeaderboardId, int> leaderboardSizes = new Dictionary<LeaderboardId, int>();

		public DatabaseHandler()
		{
			Directory.CreateDirectory(ApplicationDataFolder);
			Directory.CreateDirectory(DatabaseFolder);
		}

		public void Load(MatchTypeId gameMode)
		{
			FindLeaderboardSizes(gameMode);

			PlayerDb.Load(DatabaseFolder);
			MatchDb.Load(DatabaseFolder, gameMode);
		}

		public void ProcessPlayers(MatchTypeId gameMode)
		{
			FindNewPlayers(gameMode, 1, -1);
			var knownRankedPlayers = PlayerDb.GetRankedPlayersFromDatabase(gameMode);

			UpdatePlayerDetails(knownRankedPlayers);
			PlayerDb.Write(DatabaseFolder);
		}

		public void ProcessMatches(MatchTypeId gameMode, long startedAfterTimestamp)
		{
			var playersToBeProcessed = PlayerDb.GetRankedPlayersFromDatabase(gameMode);
			int oldMatchCount = MatchDb.MatchData.Count;

			while (playersToBeProcessed.Count > 0)
			{
				int batchSize = 200;
				if (playersToBeProcessed.Count < batchSize)
				{
					batchSize = playersToBeProcessed.Count;
				}

				UserIO.WriteLogLine("Retrieving match history for {0} players", playersToBeProcessed.Count);

				var range = playersToBeProcessed.GetRange(0, batchSize);
				playersToBeProcessed.RemoveRange(0, batchSize);

				List<int> profileIds = new List<int>();
				for (int i = 0; i < range.Count; i++)
				{
					var x = range[i];
					profileIds.Add(x.ProfileId);
				}

				var response = RelicAPI.RecentMatchHistory.GetByProfileId(profileIds);

				for (int i = 0; i < response.Profiles.Count; i++)
				{
					var x = response.Profiles[i];
					PlayerDb.LogPlayer(x);
				}

				for (int i = 0; i < response.MatchHistoryStats.Count; i++)
				{
					var x = response.MatchHistoryStats[i];
					if (x.MatchTypeId == (int)gameMode && x.StartGameTime >= startedAfterTimestamp)
					{
						MatchDb.LogMatch(x);
					}
				}

				UserIO.AllowPause();
			}

			int newMatchCount = MatchDb.MatchData.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLogLine("{0} new matches found", difference);

			PlayerDb.Write(DatabaseFolder);
			MatchDb.Write(DatabaseFolder, gameMode);
		}

		private void FindLeaderboardSizes(MatchTypeId gameMode)
		{
			UserIO.WriteLogLine("Finding leaderboard sizes");

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, gameMode) == false)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;

				if (leaderboardSizes.ContainsKey((LeaderboardId)leaderboardIndex))
				{
					leaderboardSizes[(LeaderboardId)leaderboardIndex] = leaderboardMaxRank;
				}
				else
				{
					leaderboardSizes.Add((LeaderboardId)leaderboardIndex, leaderboardMaxRank);
				}

				UserIO.WriteLogLine(((LeaderboardId)leaderboardIndex).ToString() + " " + leaderboardSizes[(LeaderboardId)leaderboardIndex]);
			}
		}

		private void FindNewPlayers(MatchTypeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			UserIO.WriteLogLine("Finding new players");

			int numPlayersBefore = PlayerDb.PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, gameMode) == false)
				{
					continue;
				}

				int leaderboardMaxRank = leaderboardSizes[(LeaderboardId)leaderboardIndex];
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

					var response = RelicAPI.Leaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);

					for (int i = 0; i < response.StatGroups.Count; i++)
					{
						var sg = response.StatGroups[i];
						for (int j = 0; j < sg.Members.Count; j++)
						{
							var x = sg.Members[j];
							PlayerDb.LogPlayer(x);
						}

						PlayerDb.LogStatGroup(sg);
					}

					for (int i = 0; i < response.LeaderboardStats.Count; i++)
					{
						var lbs = response.LeaderboardStats[i];
						PlayerDb.LogStat(lbs);
					}

					UserIO.WriteLogLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);
					batchStartingIndex += batchSize;

					UserIO.AllowPause();
				}
			}

			int numPlayersAfter = PlayerDb.PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;

			UserIO.WriteLogLine("{0} new players found", playerCountDiff);
		}

		private void UpdatePlayerDetails(List<RelicAPI.PlayerIdentity> players)
		{
			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					UserIO.WriteLogLine("Updating player details, {0} remaining", players.Count);

					var range = players.GetRange(0, batchSize);
					players.RemoveRange(0, batchSize);

					List<int> profileIds = range.Select(p => p.ProfileId).ToList();
					var response = RelicAPI.PersonalStat.GetByProfileId(profileIds);

					for (int i = 0; i < response.StatGroups.Count; i++)
					{
						var sg = response.StatGroups[i];
						for (int j = 0; j < sg.Members.Count; j++)
						{
							var x = sg.Members[j];
							PlayerDb.LogPlayer(x);
						}

						PlayerDb.LogStatGroup(sg);
					}

					for (int i = 0; i < response.LeaderboardStats.Count; i++)
					{
						var lbs = response.LeaderboardStats[i];
						PlayerDb.LogStat(lbs);
					}
				}

				else
				{
					batchSize = players.Count;
				}

				UserIO.AllowPause();
			}
		}

		public int GetLeaderboardRankByPercentile(LeaderboardId id, double percentile)
		{
			if (leaderboardSizes.ContainsKey(id) == false)
			{
				var probeResponse = RelicAPI.Leaderboard.GetById((int)id, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				leaderboardSizes.Add(id, leaderboardMaxRank);
			}

			int maxRank = leaderboardSizes[id];
			double cutoffRank = maxRank * (percentile / 100.0);

			return (int)cutoffRank;
		}
	}
}
