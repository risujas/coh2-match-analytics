using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coh2Stats
{
	public class Database
	{
		public List<RelicAPI.PlayerIdentity> PlayerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> StatGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> LeaderboardStats = new List<RelicAPI.LeaderboardStat>();

		[JsonIgnore] public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> MatchHistoryStats = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		[JsonIgnore] private readonly string databaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";
		[JsonIgnore] private const string playerDatabaseFile = "playerData.txt";
		[JsonIgnore] private const string matchDatabaseFile = "matchData.txt";
		[JsonIgnore] private List<RelicAPI.PlayerIdentity> matchHistoryProcessQueue = new List<RelicAPI.PlayerIdentity>();

		[JsonIgnore] private Dictionary<LeaderboardId, int> leaderboardSizes = new Dictionary<LeaderboardId, int>();

		public Database()
		{
			Directory.CreateDirectory(databaseFolder);
		}

		// BUILDER METHODS

		public void FindNewPlayers(MatchTypeId gameMode)
		{
			var players = GetNewPlayers(gameMode, 1, -1);
			FetchPlayerDetails(players);

			WritePlayerDatabase();
		}

		public bool ProcessMatches(MatchTypeId matchTypeId, int maxPlayers)
		{
			if (matchHistoryProcessQueue.Count == 0)
			{
				SortPlayersByHighestRank(matchTypeId);
				matchHistoryProcessQueue = PlayerIdentities.GetRange(0, maxPlayers).Where(p => p.GetHighestRank(this, matchTypeId) != int.MaxValue).ToList(); // TODO something more efficient
			}

			int batchSize = 200;
			if (matchHistoryProcessQueue.Count < batchSize)
			{
				batchSize = matchHistoryProcessQueue.Count;
			}

			Console.Write("Getting match history for {0} players...", matchHistoryProcessQueue.Count);

			var range = matchHistoryProcessQueue.GetRange(0, batchSize);
			matchHistoryProcessQueue.RemoveRange(0, batchSize);

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

			int oldMatchCount = MatchHistoryStats.Count;
			for (int i = 0; i < response.MatchHistoryStats.Count; i++)
			{
				var x = response.MatchHistoryStats[i];
				if (x.MatchTypeId == (int)matchTypeId)
				{
					LogMatch(x);
				}
			}
			int newMatchCount = MatchHistoryStats.Count;

			int difference = newMatchCount - oldMatchCount;
			Console.WriteLine(" +{0}", difference);

			if (newMatchCount > oldMatchCount)
			{
				WritePlayerDatabase();
				WriteMatchDatabase();
			}

			if (matchHistoryProcessQueue.Count == 0)
			{
				return false;
			}

			return true;
		}

		private List<RelicAPI.PlayerIdentity> GetNewPlayers(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
		{
			int numPlayersBefore = PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, matchTypeId) == false)
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

					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, PlayerIdentities.Count);
					batchStartingIndex += batchSize;
				}
			}

			int numPlayersAfter = PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;
			var newPlayers = PlayerIdentities.GetRange(numPlayersBefore, playerCountDiff);

			return newPlayers;
		}

		private void FetchPlayerDetails(List<RelicAPI.PlayerIdentity> players)
		{
			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					Console.WriteLine("Fetching player summaries, {0} remaining", players.Count);

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
				Console.WriteLine("No player database found");
				return false;
			}

			Console.WriteLine("Player database found...");

			string text = File.ReadAllText(fullPath);
			var json = JsonConvert.DeserializeObject<Database>(text);

			PlayerIdentities = json.PlayerIdentities;
			StatGroups = json.StatGroups;
			LeaderboardStats = json.LeaderboardStats;

			Console.WriteLine("{0} player identities", PlayerIdentities.Count);
			Console.WriteLine("{0} stat groups", StatGroups.Count);
			Console.WriteLine("{0} leaderboard stats", LeaderboardStats.Count);

			return true;
		}

		public void WritePlayerDatabase()
		{
			Directory.CreateDirectory(databaseFolder);

			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			string fullPath = databaseFolder + "\\" + playerDatabaseFile;
			File.WriteAllText(fullPath, text);
		}

		public bool LoadMatchDatabase()
		{
			string fullPath = databaseFolder + "\\" + matchDatabaseFile;
			if (!File.Exists(fullPath))
			{
				Console.WriteLine("No match database found");
				return false;
			}

			Console.WriteLine("Match database found...");

			string text = File.ReadAllText(fullPath);
			MatchHistoryStats = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			Console.WriteLine("{0} match history stats", MatchHistoryStats.Count);

			return true;
		}

		public void WriteMatchDatabase()
		{
			Directory.CreateDirectory(databaseFolder);

			var text = JsonConvert.SerializeObject(MatchHistoryStats, Formatting.Indented);
			string fullPath = databaseFolder + "\\" + matchDatabaseFile;
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

		public RelicAPI.PlayerIdentity GetPlayerBySteamId(string steamId)
		{
			for (int i = 0; i < PlayerIdentities.Count; i++)
			{
				var x = PlayerIdentities[i];

				if (x.Name == steamId)
				{
					return x;
				}
			}

			return null;
		}

		public RelicAPI.PlayerIdentity GetPlayerByPersonalStatGroupId(int personalStatGroupId)
		{
			for (int i = 0; i < PlayerIdentities.Count; i++)
			{
				var x = PlayerIdentities[i];

				if (x.PersonalStatGroupId == personalStatGroupId)
				{
					return x;
				}
			}

			return null;
		}

		public void LogPlayer(RelicAPI.PlayerIdentity playerIdentity)
		{
			if (GetPlayerByProfileId(playerIdentity.ProfileId) == null)
			{
				PlayerIdentities.Add(playerIdentity);
			}
		}

		public void SortPlayersByHighestRank(MatchTypeId matchTypeId)
		{
			Console.WriteLine("Sorting player list by best rank");

			List<RelicAPI.PlayerIdentity> rankedPlayers = new List<RelicAPI.PlayerIdentity>();
			List<RelicAPI.PlayerIdentity> unrankedPlayers = new List<RelicAPI.PlayerIdentity>();

			for (int i = 0; i < PlayerIdentities.Count; i++)
			{
				var p = PlayerIdentities[i];
				if (p.GetHighestRank(this, matchTypeId) == int.MaxValue)
				{
					unrankedPlayers.Add(p);
				}
				else
				{
					rankedPlayers.Add(p);
				}
			}

			rankedPlayers = rankedPlayers.OrderBy(p => p.GetHighestRank(this, matchTypeId)).ToList();

			var combinedPlayers = rankedPlayers;
			combinedPlayers.AddRange(unrankedPlayers);

			PlayerIdentities = combinedPlayers;
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

		public void LogStatGroup(RelicAPI.StatGroup psg)
		{
			if (GetStatGroupById(psg.Id) == null)
			{
				StatGroups.Add(psg);
			}
		}

		// LEADERBOARDSTAT ACCESS METHODS

		public RelicAPI.LeaderboardStat GetHighestStatByStatGroup(int statGroupId, MatchTypeId matchTypeId)
		{
			RelicAPI.LeaderboardStat highest = null;

			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId && LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)x.LeaderboardId, matchTypeId))
				{
					if (highest == null)
					{
						highest = x;
					}

					else if (x.Rank < highest.Rank && x.Rank >= 1)
					{
						highest = x;
					}
				}
			}

			return highest;
		}

		public RelicAPI.LeaderboardStat GetLowestStatByStatGroup(int statGroupId)
		{
			RelicAPI.LeaderboardStat lowest = null;

			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId)
				{
					if (lowest == null)
					{
						lowest = x;
					}

					else if (x.Rank > lowest.Rank)
					{
						lowest = x;
					}
				}
			}

			return lowest;
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

		public List<RelicAPI.LeaderboardStat> GetAllStatsByStatGroup(int statGroupId)
		{
			List<RelicAPI.LeaderboardStat> stats = new List<RelicAPI.LeaderboardStat>();

			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId)
				{
					stats.Add(x);
				}
			}

			return stats;
		}

		public void LogStat(RelicAPI.LeaderboardStat stat)
		{
			var oldStat = GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId);
			if (oldStat == null)
			{
				LeaderboardStats.Add(stat);
			}
			else
			{
				LeaderboardStats.Remove(oldStat);
				LeaderboardStats.Add(stat);
			}
		}

		// MATCHHISTORYSTAT ACCCESS METHODS

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
