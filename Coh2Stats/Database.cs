﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class Database
	{
		[JsonIgnore] private const string databaseFile = "data.txt";
		[JsonIgnore] List<PlayerIdentity> matchHistoryProcessQueue = new List<PlayerIdentity>();

		public List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();
		public List<StatGroup> statGroups = new List<StatGroup>();
		public List<LeaderboardStat> leaderboardStats = new List<LeaderboardStat>();
		public List<JsonRecentMatchHistory.MatchHistoryStat> matchHistoryStats = new List<JsonRecentMatchHistory.MatchHistoryStat>();

		// BUILDER METHODS

		public void FindNewPlayers(MatchTypeId gameMode)
		{
			ParseLeaderboards(gameMode, 1, -1);
			FetchPlayerDetails();
			SortPlayersByHighestRank();
		}

		public void ProcessMatches()
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
			var response = JsonRecentMatchHistory.GetByProfileId(profileIds);

			foreach (var x in response.Profiles)
			{
				LogPlayer(x);
			}

			int oldMatchCount = matchHistoryStats.Count;
			foreach (var mhs in response.MatchHistoryStats)
			{
				LogMatch(mhs);
			}
			int newMatchCount = matchHistoryStats.Count;

			if (newMatchCount > oldMatchCount)
			{
				WriteToFile();
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

					var response = JsonLeaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);

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

					var response = JsonPersonalStat.GetByProfileId(profileIds);

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

		public bool LoadFromFile()
		{
			if (!File.Exists(databaseFile))
			{
				Console.WriteLine("No player database found");
				return false;
			}

			Console.WriteLine("Player database found...");

			string text = File.ReadAllText(databaseFile);
			var json = JsonConvert.DeserializeObject<Database>(text);

			playerIdentities = json.playerIdentities;
			statGroups = json.statGroups;
			leaderboardStats = json.leaderboardStats;
			matchHistoryStats = json.matchHistoryStats;

			Console.WriteLine("{0} player identities", playerIdentities.Count);
			Console.WriteLine("{0} stat groups", statGroups.Count);
			Console.WriteLine("{0} leaderboard stats", leaderboardStats.Count);
			Console.WriteLine("{0} match history stats", matchHistoryStats.Count);

			return true;
		}

		public void WriteToFile()
		{
			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(databaseFile, text);
		}

		// PLAYERIDENTITY ACCESS METHODS

		public PlayerIdentity GetPlayerByProfileId(int profileId)
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

		public PlayerIdentity GetPlayerBySteamId(string steamId)
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

		public PlayerIdentity GetPlayerByPersonalStatGroupId(int personalStatGroupId)
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

		public void LogPlayer(PlayerIdentity playerIdentity)
		{
			if (GetPlayerByProfileId(playerIdentity.ProfileId) == null)
			{
				playerIdentities.Add(playerIdentity);
			}
		}

		public void SortPlayersByHighestRank()
		{
			Console.WriteLine("Sorting player list by best rank");

			playerIdentities = playerIdentities.OrderBy(p => p.GetHighestRank(this)).ToList();
		}

		// STATGROUP ACCESS METHODS

		public StatGroup GetStatGroupById(int id)
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

		public void LogStatGroup(StatGroup psg)
		{
			if (GetStatGroupById(psg.Id) == null)
			{
				statGroups.Add(psg);
			}
		}

		// LEADERBOARDSTAT ACCESS METHODS

		public LeaderboardStat GetHighestStatByStatGroup(int statGroupId)
		{
			LeaderboardStat highest = null;

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					if (highest == null)
					{
						highest = x;
					}

					else if (x.Rank < highest.Rank)
					{
						highest = x;
					}
				}
			}

			return highest;
		}

		public LeaderboardStat GetLowestStatByStatGroup(int statGroupId)
		{
			LeaderboardStat lowest = null;

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

		public LeaderboardStat GetStat(int statGroupId, LeaderboardId leaderboardId)
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

		public List<LeaderboardStat> GetAllStatsByStatGroup(int statGroupId)
		{
			List<LeaderboardStat> stats = new List<LeaderboardStat>();

			foreach (var x in leaderboardStats)
			{
				if (x.StatGroupId == statGroupId)
				{
					stats.Add(x);
				}
			}

			return stats;
		}

		public void LogStat(LeaderboardStat stat)
		{
			if (GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId) == null)
			{
				leaderboardStats.Add(stat);
			}
		}

		// MATCHHISTORYSTAT ACCCESS METHODS

		public void LogMatch(JsonRecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.Id) == null)
			{
				matchHistoryStats.Add(matchHistoryStat);
			}
		}

		public JsonRecentMatchHistory.MatchHistoryStat GetById(int id)
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
