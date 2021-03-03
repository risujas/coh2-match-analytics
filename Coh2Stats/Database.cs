using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coh2Stats
{
	public class Database
	{
		// TODO package these 3 classes into one so Database doesn't need to be serialized
		public List<RelicAPI.PlayerIdentity> PlayerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> StatGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> LeaderboardStats = new List<RelicAPI.LeaderboardStat>();

		[JsonIgnore] public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> MatchHistoryStats = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		[JsonIgnore] private readonly string databaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";
		[JsonIgnore] private const string playerDatabaseFile = "playerData.txt";
		[JsonIgnore] private const string matchDatabaseFile = "matchData.txt";

		[JsonIgnore] private Dictionary<LeaderboardId, int> leaderboardSizes = new Dictionary<LeaderboardId, int>();

		public Database()
		{
			Directory.CreateDirectory(databaseFolder);
		}

		// BUILDER METHODS

		public void ProcessPlayers(MatchTypeId gameMode)
		{
			var newPlayers = GetNewPlayers(gameMode, 1, -1);
			var knownRankedPlayers = PlayerIdentities.Where(p => p.GetHighestRank(this, gameMode) != int.MaxValue).ToList();

			for (int i = 0; i < newPlayers.Count; i++)
			{
				var x = newPlayers[i];
				
				bool found = false;
				for (int j = 0; j < knownRankedPlayers.Count; j++)
				{
					if (x.ProfileId == knownRankedPlayers[j].ProfileId)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					knownRankedPlayers.Add(x);
				}
			}

			UpdatePlayerDetails(knownRankedPlayers);
			WritePlayerDatabase();
		}

		public void ProcessMatches(MatchTypeId gameMode, long startedAfterTimestamp)
		{
			var playersToBeProcessed = PlayerIdentities.Where(p => p.GetHighestRank(this, gameMode) != int.MaxValue).ToList(); // TODO something more efficient
			int oldMatchCount = MatchHistoryStats.Count;

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
					LogPlayer(x);
				}

				for (int i = 0; i < response.MatchHistoryStats.Count; i++)
				{
					var x = response.MatchHistoryStats[i];
					if (x.MatchTypeId == (int)gameMode && x.StartGameTime >= startedAfterTimestamp)
					{
						LogMatch(x);
					}
				}
			}

			int newMatchCount = MatchHistoryStats.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLogLine("{0} new matches found", difference);

			WritePlayerDatabase();
			WriteMatchDatabase(gameMode);
		}

		private List<RelicAPI.PlayerIdentity> GetNewPlayers(MatchTypeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			UserIO.WriteLogLine("Finding new players");

			int numPlayersBefore = PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, gameMode) == false)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				int batchStartingIndex = startingRank;

				if (leaderboardSizes.ContainsKey((LeaderboardId)leaderboardIndex))
				{
					leaderboardSizes[(LeaderboardId)leaderboardIndex] = leaderboardMaxRank;
				}
				else
				{
					leaderboardSizes.Add((LeaderboardId)leaderboardIndex, leaderboardMaxRank);
				}

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
							LogPlayer(x);
						}

						LogStatGroup(sg);
					}

					for (int i = 0; i < response.LeaderboardStats.Count; i++)
					{
						var lbs = response.LeaderboardStats[i];
						LogStat(lbs);
					}

					UserIO.WriteLogLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);
					batchStartingIndex += batchSize;
				}
			}

			int numPlayersAfter = PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;
			var newPlayers = PlayerIdentities.GetRange(numPlayersBefore, playerCountDiff);

			UserIO.WriteLogLine("{0} new players found", playerCountDiff);

			return newPlayers;
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
							LogPlayer(x);
						}

						LogStatGroup(sg);
					}

					for (int i = 0; i < response.LeaderboardStats.Count; i++)
					{
						var lbs = response.LeaderboardStats[i];
						LogStat(lbs);
					}
				}

				else
				{
					batchSize = players.Count;
				}
			}
		}

		// FILE HANDLING METHODS

		public bool LoadPlayerDatabase()
		{
			string fullPath = databaseFolder + "\\" + playerDatabaseFile;
			if (!File.Exists(fullPath))
			{
				UserIO.WriteLogLine("No player database found");
				return false;
			}

			UserIO.WriteLogLine("Player database found");

			string text = File.ReadAllText(fullPath);
			var json = JsonConvert.DeserializeObject<Database>(text);

			PlayerIdentities = json.PlayerIdentities;
			StatGroups = json.StatGroups;
			LeaderboardStats = json.LeaderboardStats;

			UserIO.WriteLogLine("{0} player identities", PlayerIdentities.Count);
			UserIO.WriteLogLine("{0} stat groups", StatGroups.Count);
			UserIO.WriteLogLine("{0} leaderboard stats", LeaderboardStats.Count);

			return true;
		}

		public void WritePlayerDatabase()
		{
			UserIO.WriteLogLine("Writing player database");

			Directory.CreateDirectory(databaseFolder);

			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			string fullPath = databaseFolder + "\\" + playerDatabaseFile;
			File.WriteAllText(fullPath, text);
		}

		public bool LoadMatchDatabase(MatchTypeId gameMode)
		{
			string fullPath = databaseFolder + "\\" + gameMode.ToString() + matchDatabaseFile;
			if (!File.Exists(fullPath))
			{
				UserIO.WriteLogLine("No match database found");
				return false;
			}

			UserIO.WriteLogLine("Match database found");

			string text = File.ReadAllText(fullPath);
			MatchHistoryStats = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			UserIO.WriteLogLine("{0} match history stats", MatchHistoryStats.Count);

			return true;
		}

		public void WriteMatchDatabase(MatchTypeId gameMode)
		{
			UserIO.WriteLogLine("Writing match database");

			Directory.CreateDirectory(databaseFolder);

			var text = JsonConvert.SerializeObject(MatchHistoryStats, Formatting.Indented);
			string fullPath = databaseFolder + "\\" + gameMode.ToString() + matchDatabaseFile;
			File.WriteAllText(fullPath, text);
		}

		// PLAYERIDENTITY ACCESS METHODS

		public RelicAPI.PlayerIdentity GetPlayerByProfileId(int profileId)
		{
			for (int i = 0; i < PlayerIdentities.Count; i++)
			{
				var x = PlayerIdentities[i];
				if (x.ProfileId == profileId)
				{
					return x;
				}
			}

			return null;
		}

		public void LogPlayer(RelicAPI.PlayerIdentity playerIdentity)
		{
			var oldPlayerIdentity = GetPlayerByProfileId(playerIdentity.ProfileId);
			if (oldPlayerIdentity != null)
			{
				PlayerIdentities.Remove(oldPlayerIdentity);
			}
			PlayerIdentities.Add(playerIdentity);
		}

		// STATGROUP ACCESS METHODS

		public RelicAPI.StatGroup GetStatGroupById(int id)
		{
			for (int i = 0; i < StatGroups.Count; i++)
			{
				var x = StatGroups[i];

				if (x.Id == id)
				{
					return x;
				}
			}

			return null;
		}

		public void LogStatGroup(RelicAPI.StatGroup statGroup)
		{
			var oldStatGroup = GetStatGroupById(statGroup.Id);
			if (oldStatGroup != null)
			{
				StatGroups.Remove(oldStatGroup);
			}
			StatGroups.Add(statGroup);
		}

		// LEADERBOARDSTAT ACCESS METHODS

		public RelicAPI.LeaderboardStat GetHighestStatByStatGroup(int statGroupId, MatchTypeId gameMode)
		{
			RelicAPI.LeaderboardStat highest = null;

			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId && LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)x.LeaderboardId, gameMode))
				{
					if (highest == null)
					{
						highest = x;
					}

					else if ((highest.Rank == -1) || x.Rank < highest.Rank && x.Rank >= 1)
					{
						highest = x;
					}
				}
			}

			return highest;
		}

		public RelicAPI.LeaderboardStat GetStat(int statGroupId, LeaderboardId leaderboardId)
		{
			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId && x.LeaderboardId == (int)leaderboardId)
				{
					return x;
				}
			}

			return null;
		}

		public void LogStat(RelicAPI.LeaderboardStat stat)
		{
			var oldStat = GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId);
			if (oldStat != null)
			{
				LeaderboardStats.Remove(oldStat);
			}
			LeaderboardStats.Add(stat);
		}

		// MATCHHISTORYSTAT ACCESS METHODS

		public void LogMatch(RelicAPI.RecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.Id) == null)
			{
				MatchHistoryStats.Add(matchHistoryStat);
			}
		}

		public RelicAPI.RecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			for (int i = 0; i < MatchHistoryStats.Count; i++)
			{
				var x = MatchHistoryStats[i];

				if (x.Id == id)
				{
					return x;
				}
			}

			return null;
		}

		// OTHER METHODS

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
