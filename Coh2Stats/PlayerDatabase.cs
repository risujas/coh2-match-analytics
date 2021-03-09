using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Coh2Stats
{
	public class PlayerDatabase
	{
		public List<RelicAPI.PlayerIdentity> PlayerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> StatGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> LeaderboardStats = new List<RelicAPI.LeaderboardStat>();

		[JsonIgnore] public const string DbFile = "playerData.txt";
		[JsonIgnore] public Dictionary<LeaderboardId, int> LeaderboardSizes = new Dictionary<LeaderboardId, int>();

		public bool Load(string dbFolder)
		{
			string fullPath = dbFolder + "\\" + DbFile;
			if (!File.Exists(fullPath))
			{
				UserIO.WriteLogLine("No player database found");
				return false;
			}

			UserIO.WriteLogLine("Player database found");

			string text = File.ReadAllText(fullPath);
			var json = JsonConvert.DeserializeObject<PlayerDatabase>(text);

			PlayerIdentities = json.PlayerIdentities;
			StatGroups = json.StatGroups;
			LeaderboardStats = json.LeaderboardStats;

			UserIO.WriteLogLine("{0} player identities", PlayerIdentities.Count);
			UserIO.WriteLogLine("{0} stat groups", StatGroups.Count);
			UserIO.WriteLogLine("{0} leaderboard stats", LeaderboardStats.Count);

			return true;
		}

		public void Write(string dbFolder)
		{
			UserIO.WriteLogLine("Writing player database");

			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			string fullPath = dbFolder + "\\" + DbFile;
			File.WriteAllText(fullPath, text);
		}

		public void FindNewPlayers(MatchTypeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			UserIO.WriteLogLine("Finding new players");

			int numPlayersBefore = PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, gameMode) == false)
				{
					continue;
				}

				int leaderboardMaxRank = LeaderboardSizes[(LeaderboardId)leaderboardIndex];
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

					UserIO.WriteLogLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);
					batchStartingIndex += batchSize;

					UserIO.AllowPause();
				}
			}

			int numPlayersAfter = PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;

			UserIO.WriteLogLine("{0} new players found", playerCountDiff);
		}

		public void UpdatePlayerDetails(MatchTypeId gameMode)
		{
			var players = GetRankedPlayersFromDatabase(gameMode);

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

				UserIO.AllowPause();
			}
		}

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

		public List<RelicAPI.PlayerIdentity> GetRankedPlayersFromDatabase(MatchTypeId gameMode)
		{
			UserIO.WriteLogLine("Getting ranked players from the database. This is a long operation.");
			var rankedPlayers = PlayerIdentities.Where(p => p.GetHighestRank(this, gameMode) != int.MaxValue).ToList();
			UserIO.WriteLogLine("Finished.");

			return rankedPlayers;

		}

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

		public void FindLeaderboardSizes(MatchTypeId gameMode)
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

				if (LeaderboardSizes.ContainsKey((LeaderboardId)leaderboardIndex))
				{
					LeaderboardSizes[(LeaderboardId)leaderboardIndex] = leaderboardMaxRank;
				}
				else
				{
					LeaderboardSizes.Add((LeaderboardId)leaderboardIndex, leaderboardMaxRank);
				}

				UserIO.WriteLogLine(((LeaderboardId)leaderboardIndex).ToString() + " " + LeaderboardSizes[(LeaderboardId)leaderboardIndex]);
			}
		}

		public int GetLeaderboardRankByPercentile(LeaderboardId id, double percentile)
		{
			if (LeaderboardSizes.ContainsKey(id) == false)
			{
				var probeResponse = RelicAPI.Leaderboard.GetById((int)id, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				LeaderboardSizes.Add(id, leaderboardMaxRank);
			}

			int maxRank = LeaderboardSizes[id];
			double cutoffRank = maxRank * (percentile / 100.0);

			return (int)cutoffRank;
		}
	}
}
