﻿using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class PlayerDatabase
	{
		public List<RelicAPI.PlayerIdentity> PlayerIdentities = new List<RelicAPI.PlayerIdentity>();
		public List<RelicAPI.StatGroup> StatGroups = new List<RelicAPI.StatGroup>();
		public List<RelicAPI.LeaderboardStat> LeaderboardStats = new List<RelicAPI.LeaderboardStat>();

		public void FindPlayerNames()
		{
			UserIO.WriteLine("Finding player names");

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.RequestById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				int batchStartingIndex = 1;

				while (batchStartingIndex < leaderboardMaxRank)
				{
					int difference = leaderboardMaxRank - batchStartingIndex;
					int batchSize = 200;

					if (difference < 200)
					{
						batchSize = difference + 1;
					}

					UserIO.WriteLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);

					var response = RelicAPI.Leaderboard.RequestById(leaderboardIndex, batchStartingIndex, batchSize);

					for (int i = 0; i < response.StatGroups.Count; i++)
					{
						var sg = response.StatGroups[i];
						for (int j = 0; j < sg.Members.Count; j++)
						{
							var x = sg.Members[j];
							LogPlayer(x);
						}
					}

					batchStartingIndex += batchSize;

					UserIO.AllowPause();
				}
			}

			UserIO.WriteLine("{0} players names found", PlayerIdentities.Count);
		}

		public void FindPlayerStats()
		{
			var players = PlayerIdentities.ToList();

			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					UserIO.WriteLine("Finding player stats, {0} remaining", players.Count);

					var range = players.GetRange(0, batchSize);
					players.RemoveRange(0, batchSize);

					List<int> profileIds = range.Select(p => p.ProfileId).ToList();
					var response = RelicAPI.PersonalStat.RequestByProfileId(profileIds);

					for (int i = 0; i < response.StatGroups.Count; i++)
					{
						var sg = response.StatGroups[i];
						LogStatGroup(sg);
					}

					for (int i = 0; i < response.LeaderboardStats.Count; i++)
					{
						var lbs = response.LeaderboardStats[i];
						LogLeaderboardStat(lbs);
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

		public RelicAPI.LeaderboardStat GetLeaderboardStatByStatGroupId(int statGroupId, LeaderboardId leaderboardId)
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

		public void LogLeaderboardStat(RelicAPI.LeaderboardStat stat)
		{
			var oldStat = GetLeaderboardStatByStatGroupId(stat.StatGroupId, (LeaderboardId)stat.LeaderboardId);
			if (oldStat != null)
			{
				LeaderboardStats.Remove(oldStat);
			}
			LeaderboardStats.Add(stat);
		}
	}
}
