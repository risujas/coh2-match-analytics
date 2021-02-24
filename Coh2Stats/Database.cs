﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			var players = ParseLeaderboards(gameMode, 1, -1);
			FetchPlayerDetails(players);
		}

		public bool ProcessMatches(MatchTypeId matchTypeId, int maxPlayers)
		{
			if (matchHistoryProcessQueue.Count == 0)
			{
				SortPlayersByHighestRank(matchTypeId);
				matchHistoryProcessQueue = playerIdentities.GetRange(0, maxPlayers).Where(p => p.GetHighestRank(this, matchTypeId) != int.MaxValue).ToList(); // TODO something more efficient
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

			int oldMatchCount = matchHistoryStats.Count;
			for (int i = 0; i < response.MatchHistoryStats.Count; i++)
			{
				var x = response.MatchHistoryStats[i];
				if (x.MatchTypeId == (int)matchTypeId)
				{
					LogMatch(x);
				}
			}
			int newMatchCount = matchHistoryStats.Count;

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

		private List<RelicAPI.PlayerIdentity> ParseLeaderboards(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
		{
			int numPlayersBefore = playerIdentities.Count;

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

					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, playerIdentities.Count);
					batchStartingIndex += batchSize;
				}
			}

			int numPlayersAfter = playerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;
			var newPlayers = playerIdentities.GetRange(numPlayersBefore, playerCountDiff);

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
			if (!File.Exists(playerDatabaseFile))
			{
				Console.WriteLine("No player database found");
				return false;
			}

			Console.WriteLine("Player database found...");

			const string temporaryCopiedFile = "temp_player_db";
			File.Copy(playerDatabaseFile, temporaryCopiedFile);
			string text = File.ReadAllText(temporaryCopiedFile);
			File.Delete(temporaryCopiedFile);

			var json = JsonConvert.DeserializeObject<Database>(text);

			playerIdentities = json.playerIdentities;
			statGroups = json.statGroups;
			leaderboardStats = json.leaderboardStats;

			Console.WriteLine("{0} player identities", playerIdentities.Count);
			Console.WriteLine("{0} stat groups", statGroups.Count);
			Console.WriteLine("{0} leaderboard stats", leaderboardStats.Count);

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

			const string temporaryCopiedFile = "temp_match_db";
			File.Copy(matchDatabaseFile, temporaryCopiedFile);
			string text = File.ReadAllText(temporaryCopiedFile);
			File.Delete(temporaryCopiedFile);

			matchHistoryStats = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			Console.WriteLine("{0} match history stats", matchHistoryStats.Count);

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
			for (int i = 0; i < playerIdentities.Count; i++)
			{
				var x = playerIdentities[i];
				if (x.ProfileId == profileId)
				{
					return x;
				}
			}

			return null;
		}

		public RelicAPI.PlayerIdentity GetPlayerBySteamId(string steamId)
		{
			for (int i = 0; i < playerIdentities.Count; i++)
			{
				var x = playerIdentities[i];

				if (x.Name == steamId)
				{
					return x;
				}
			}

			return null;
		}

		public RelicAPI.PlayerIdentity GetPlayerByPersonalStatGroupId(int personalStatGroupId)
		{
			for (int i = 0; i < playerIdentities.Count; i++)
			{
				var x = playerIdentities[i];

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
				playerIdentities.Add(playerIdentity);
			}
		}

		public void SortPlayersByHighestRank(MatchTypeId matchTypeId)
		{
			Console.WriteLine("Sorting player list by best rank");

			List<RelicAPI.PlayerIdentity> rankedPlayers = new List<RelicAPI.PlayerIdentity>();
			List<RelicAPI.PlayerIdentity> unrankedPlayers = new List<RelicAPI.PlayerIdentity>();

			for (int i = 0; i < playerIdentities.Count; i++)
			{
				var p = playerIdentities[i];
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

			playerIdentities = combinedPlayers;
		}

		// STATGROUP ACCESS METHODS

		public RelicAPI.StatGroup GetStatGroupById(int id)
		{
			for (int i = 0; i < statGroups.Count; i++)
			{
				var x = statGroups[i];

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

			for (int i = 0; i < leaderboardStats.Count; i++)
			{
				var x = leaderboardStats[i];

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

			for (int i = 0; i < leaderboardStats.Count; i++)
			{
				var x = leaderboardStats[i];

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
			for (int i = 0; i < leaderboardStats.Count; i++)
			{
				var x = leaderboardStats[i];

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

			for (int i = 0; i < leaderboardStats.Count; i++)
			{
				var x = leaderboardStats[i];

				if (x.StatGroupId == statGroupId)
				{
					stats.Add(x);
				}
			}

			return stats;
		}

		public void LogStat(RelicAPI.LeaderboardStat stat)
		{
			if (GetStat(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId) == null) // TODO check if this prevents stats from being updated
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
			for (int i = 0; i < matchHistoryStats.Count; i++)
			{
				var x = matchHistoryStats[i];

				if (x.Id == id)
				{
					return x;
				}
			}

			return null;
		}
	}
}
