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

		public List<RelicAPI.PlayerIdentity> GetRankedPlayersFromDatabase(MatchTypeId gameMode)
		{
			UserIO.WriteLogLine("Getting ranked players from the database. This is a long operation.");
			var rankedPlayers = PlayerIdentities.Where(p => p.GetHighestRank(this, gameMode) != int.MaxValue).ToList();
			UserIO.WriteLogLine("Finished.");

			return rankedPlayers;

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
	}
}
