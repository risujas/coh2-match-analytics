using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coh2Stats
{
	public class PlayerDatabase
	{
		public List<RelicAPI.PlayerIdentity> PlayerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> StatGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> LeaderboardStats = new List<RelicAPI.LeaderboardStat>();

		public const string PlayerIdentityFile = "playerIdentities.json";
		public const string StatGroupFile = "statGroups.json";
		public const string LeaderboardStatFile = "leaderboardStats.json";

		private Dictionary<LeaderboardId, int> leaderboardSizes = new Dictionary<LeaderboardId, int>();

		public bool Load(string dbFolder)
		{
			if (File.Exists(dbFolder + "\\" + PlayerIdentityFile) || File.Exists(dbFolder + "\\" + StatGroupFile) || File.Exists(dbFolder + "\\" + LeaderboardStatFile))
			{
				UserIO.WriteLine("Loading player data");
			}

			string fullPath = dbFolder + "\\" + PlayerIdentityFile;
			if (File.Exists(fullPath))
			{
				string text = File.ReadAllText(fullPath);
				PlayerIdentities = JsonConvert.DeserializeObject<List<RelicAPI.PlayerIdentity>>(text);
				UserIO.WriteLine("{0} player identities", PlayerIdentities.Count);
			}

			fullPath = dbFolder + "\\" + StatGroupFile;
			if (File.Exists(fullPath))
			{
				string text = File.ReadAllText(fullPath);
				StatGroups = JsonConvert.DeserializeObject<List<RelicAPI.StatGroup>>(text);
				UserIO.WriteLine("{0} stat groups", StatGroups.Count);
			}

			fullPath = dbFolder + "\\" + LeaderboardStatFile;
			if (File.Exists(fullPath))
			{
				string text = File.ReadAllText(fullPath);
				LeaderboardStats = JsonConvert.DeserializeObject<List<RelicAPI.LeaderboardStat>>(text);
				UserIO.WriteLine("{0} leaderboard stats", LeaderboardStats.Count);
			}

			return true;
		}

		public void Write(string dbFolder)
		{
			UserIO.WriteLine("Writing player data");

			var text = JsonConvert.SerializeObject(PlayerIdentities, Formatting.Indented);
			var fullPath = dbFolder + "\\" + PlayerIdentityFile;
			File.WriteAllText(fullPath, text);

			text = JsonConvert.SerializeObject(StatGroups, Formatting.Indented);
			fullPath = dbFolder + "\\" + StatGroupFile;
			File.WriteAllText(fullPath, text);

			text = JsonConvert.SerializeObject(LeaderboardStats, Formatting.Indented);
			fullPath = dbFolder + "\\" + LeaderboardStatFile;
			File.WriteAllText(fullPath, text);
		}

		public void FindNewPlayers(MatchTypeId gameMode, int startingRank = 1, int maxRank = -1)
		{
			UserIO.WriteLine("Finding new players");

			int numPlayersBefore = PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardIsCompatibleWithGameMode((LeaderboardId)leaderboardIndex, gameMode) == false)
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

					var response = RelicAPI.Leaderboard.RequestById(leaderboardIndex, batchStartingIndex, batchSize);

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

					UserIO.WriteLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);
					batchStartingIndex += batchSize;

					UserIO.AllowPause();
				}
			}

			int numPlayersAfter = PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;

			UserIO.WriteLine("{0} new players found", playerCountDiff);
		}

		public void UpdatePlayerDetails(MatchTypeId gameMode)
		{
			var players = GetRankedPlayersFromDatabase(gameMode);

			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					UserIO.WriteLine("Updating player details, {0} remaining", players.Count);

					var range = players.GetRange(0, batchSize);
					players.RemoveRange(0, batchSize);

					List<int> profileIds = range.Select(p => p.ProfileId).ToList();
					var response = RelicAPI.PersonalStat.RequestByProfileId(profileIds);

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

			UserIO.WriteLine("Missing player data; making an additional request to fill the gaps");

			List<int> list = new List<int>();
			list.Add(profileId);

			var ps = RelicAPI.PersonalStat.RequestByProfileId(list);
			RelicAPI.PlayerIdentity player = null;

			foreach (var sg in ps.StatGroups)
			{
				if (sg.Type == 1)
				{
					player = sg.Members[0];
					LogPlayer(player);
				}

				LogStatGroup(sg);
			}

			foreach (var lbs in ps.LeaderboardStats)
			{
				LogStat(lbs);
			}

			Write(DatabaseHandler.DatabaseFolder);

			return player;
		}

		public void LogPlayer(RelicAPI.PlayerIdentity playerIdentity)
		{
			for (int i = 0; i < PlayerIdentities.Count; i++)
			{
				var p = PlayerIdentities[i];

				if (playerIdentity.ProfileId == p.ProfileId)
				{
					PlayerIdentities.RemoveAt(i);
					break;
				}
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
			UserIO.WriteLine("Getting ranked players from the database. This is a long operation.");
			var rankedPlayers = PlayerIdentities.Where(p => p.GetHighestRank(this, gameMode) != int.MaxValue).ToList();
			UserIO.WriteLine("Finished.");

			return rankedPlayers;

		}

		public RelicAPI.LeaderboardStat GetHighestStatByStatGroup(int statGroupId, MatchTypeId gameMode)
		{
			RelicAPI.LeaderboardStat highest = null;

			for (int i = 0; i < LeaderboardStats.Count; i++)
			{
				var x = LeaderboardStats[i];

				if (x.StatGroupId == statGroupId && LeaderboardCompatibility.LeaderboardIsCompatibleWithGameMode((LeaderboardId)x.LeaderboardId, gameMode))
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
			UserIO.WriteLine("Finding leaderboard sizes");

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardIsCompatibleWithGameMode((LeaderboardId)leaderboardIndex, gameMode) == false)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.RequestById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;

				if (leaderboardSizes.ContainsKey((LeaderboardId)leaderboardIndex))
				{
					leaderboardSizes[(LeaderboardId)leaderboardIndex] = leaderboardMaxRank;
				}
				else
				{
					leaderboardSizes.Add((LeaderboardId)leaderboardIndex, leaderboardMaxRank);
				}

				UserIO.WriteLine(((LeaderboardId)leaderboardIndex).ToString() + " " + leaderboardSizes[(LeaderboardId)leaderboardIndex]);
			}
		}

		public int GetLeaderboardRankByPercentile(LeaderboardId id, double percentile)
		{
			if (leaderboardSizes.ContainsKey(id) == false)
			{
				var probeResponse = RelicAPI.Leaderboard.RequestById((int)id, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				leaderboardSizes.Add(id, leaderboardMaxRank);
			}

			int maxRank = leaderboardSizes[id];
			double cutoffRank = maxRank * (percentile / 100.0);

			return (int)cutoffRank;
		}
	}
}
