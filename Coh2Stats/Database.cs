using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class Database
	{
		[JsonIgnore] private const string playerDatabaseFile = "playerData.txt";
		[JsonIgnore] private const string matchDatabaseFile = "matchData.txt";
		[JsonIgnore] List<RelicAPI.PlayerIdentity> matchHistoryProcessQueue = new List<RelicAPI.PlayerIdentity>();

		public List<RelicAPI.PlayerIdentity> playerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> statGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> leaderboardStats = new List<RelicAPI.LeaderboardStat>();

		[JsonIgnore] public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> matchHistoryStats = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		// BUILDER METHODS

		public void FindNewPlayers(MatchTypeId gameMode)
		{
			ParseLeaderboards(gameMode, 1, -1);
			FetchPlayerDetails();
			SortPlayersByHighestRank(gameMode);
		}

		public void ProcessMatches(MatchTypeId matchTypeId)
		{
			if (matchHistoryProcessQueue.Count == 0)
			{
				matchHistoryProcessQueue = playerIdentities;
			}

			int batchSize = 200;
			if (matchHistoryProcessQueue.Count < batchSize)
			{
				batchSize = matchHistoryProcessQueue.Count;
			}

			var range = matchHistoryProcessQueue.GetRange(0, batchSize);
			matchHistoryProcessQueue.RemoveRange(0, batchSize);

			List<int> profileIds = new List<int>();
			foreach (var p in range)
			{
				profileIds.Add(p.ProfileId);
			}

			Console.WriteLine("Getting match history for {0} players", batchSize);
			var response = RelicAPI.RecentMatchHistory.GetByProfileId(profileIds);

			foreach (var x in response.Profiles)
			{
				LogPlayer(x);
			}

			int oldMatchCount = matchHistoryStats.Count;
			foreach (var mhs in response.MatchHistoryStats)
			{
				if (mhs.MatchTypeId == (int)matchTypeId)
				{
					LogMatch(mhs);
				}
			}
			int newMatchCount = matchHistoryStats.Count;

			if (newMatchCount > oldMatchCount)
			{
				WritePlayerDatabase();
				WriteMatchDatabase();
			}
		}

		private void ParseLeaderboards(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, matchTypeId) == false)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.GetById(leaderboardIndex, 1, 1);
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

					var response = RelicAPI.Leaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);

					foreach (var sg in response.StatGroups)
					{
						foreach (var x in sg.Members)
						{
							LogPlayer(x);
						}

						LogStatGroup(sg);
					}

					foreach (var lbs in response.LeaderboardStats)
					{
						LogStat(lbs);
					}

					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, playerIdentities.Count);
					batchStartingIndex += batchSize;
				}
			}
		}

		private void FetchPlayerDetails()
		{
			var players = playerIdentities.ToList();
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

					var response = RelicAPI.PersonalStat.GetByProfileId(profileIds);

					foreach (var sg in response.StatGroups)
					{
						foreach (var x in sg.Members)
						{
							LogPlayer(x);
						}

						LogStatGroup(sg);
					}

					foreach (var lbs in response.LeaderboardStats)
					{
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
			if (!File.Exists(playerDatabaseFile))
			{
				Console.WriteLine("No player database found");
				return false;
			}

			Console.WriteLine("Player database found...");

			string text = File.ReadAllText(playerDatabaseFile);
			var json = JsonConvert.DeserializeObject<Database>(text);

			playerIdentities = json.playerIdentities;
			statGroups = json.statGroups;
			leaderboardStats = json.leaderboardStats;

			Console.WriteLine("{0} player identities", playerIdentities.Count);
			Console.WriteLine("{0} stat groups", statGroups.Count);
			Console.WriteLine("{0} leaderboard stats", leaderboardStats.Count);
			Console.WriteLine();

			return true;
		}

		public void WritePlayerDatabase()
		{
			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(playerDatabaseFile, text);
		}

		public bool LoadMatchDatabase()
		{
			if (!File.Exists(matchDatabaseFile))
			{
				Console.WriteLine("No match database found");
				return false;
			}

			Console.WriteLine("Match database found...");

			string text = File.ReadAllText(matchDatabaseFile);
			matchHistoryStats = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			Console.WriteLine("{0} match history stats", matchHistoryStats.Count);
			Console.WriteLine();

			return true;
		}

		public void WriteMatchDatabase()
		{
			var text = JsonConvert.SerializeObject(matchHistoryStats, Formatting.Indented);
			File.WriteAllText(matchDatabaseFile, text);
		}

		// PLAYERIDENTITY ACCESS METHODS

		public RelicAPI.PlayerIdentity GetPlayerByProfileId(int profileId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.ProfileId == profileId)
				{
					return pi;
				}
			}

			return null;
		}

		public RelicAPI.PlayerIdentity GetPlayerBySteamId(string steamId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.Name == steamId)
				{
					return pi;
				}
			}

			return null;
		}

		public RelicAPI.PlayerIdentity GetPlayerByPersonalStatGroupId(int personalStatGroupId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.PersonalStatGroupId == personalStatGroupId)
				{
					return pi;
				}
			}

			return null;
		}

		public void LogPlayer(RelicAPI.PlayerIdentity playerIdentity)
		{
			if (GetPlayerByProfileId(playerIdentity.ProfileId) == null)
			{
				playerIdentities.Add(playerIdentity);
			}
		}

		public void SortPlayersByHighestRank(MatchTypeId matchTypeId)
		{
			Console.WriteLine("Sorting player list by best rank");

			playerIdentities = playerIdentities.OrderBy(p => p.GetHighestRank(this, matchTypeId)).ToList();
		}

		// STATGROUP ACCESS METHODS

		public RelicAPI.StatGroup GetStatGroupById(int id)
		{
			foreach (var x in statGroups)
			{
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
				statGroups.Add(psg);
			}
		}

		// LEADERBOARDSTAT ACCESS METHODS

		public RelicAPI.LeaderboardStat GetHighestStatByStatGroup(int statGroupId, MatchTypeId matchTypeId)
		{
			RelicAPI.LeaderboardStat highest = null;

			foreach (var x in leaderboardStats)
			{
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

			foreach (var x in leaderboardStats)
			{
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
			foreach (var x in leaderboardStats)
			{
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

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					stats.Add(x);
				}
			}

			return stats;
		}

		public void LogStat(RelicAPI.LeaderboardStat stat)
		{
			if (GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId) == null)
			{
				leaderboardStats.Add(stat);
			}
		}

		// MATCHHISTORYSTAT ACCCESS METHODS

		public void LogMatch(RelicAPI.RecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.Id) == null)
			{
				matchHistoryStats.Add(matchHistoryStat);
			}
		}

		public RelicAPI.RecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			foreach (var m in matchHistoryStats)
			{
				if (m.Id == id)
				{
					return m;
				}
			}

			return null;
		}
	}
}
