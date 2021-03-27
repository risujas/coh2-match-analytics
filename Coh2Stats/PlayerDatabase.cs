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

		public Dictionary<LeaderboardId, int> LeaderboardSizes = new Dictionary<LeaderboardId, int>();

		public void FindPlayerNames()
		{
			UserIO.WriteLine("Finding players");

			int numPlayersBefore = PlayerIdentities.Count;

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
				{
					continue;
				}

				int leaderboardMaxRank = LeaderboardSizes[(LeaderboardId)leaderboardIndex];
				int batchStartingIndex = 1;

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
					}

					UserIO.WriteLine("Parsing leaderboard #{0}: {1} - {2}", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1);
					batchStartingIndex += batchSize;

					UserIO.AllowPause();
				}
			}

			int numPlayersAfter = PlayerIdentities.Count;
			int playerCountDiff = numPlayersAfter - numPlayersBefore;

			UserIO.WriteLine("{0} players found", playerCountDiff);
		}

		public void FindPlayerDetails()
		{
			var players = PlayerIdentities.ToList();

			int batchSize = 200;
			while (players.Count > 0)
			{
				if (players.Count >= batchSize)
				{
					UserIO.WriteLine("Finding player details, {0} remaining", players.Count);

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

		// can return a stat with -1 rank
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

		public void FindLeaderboardSizes()
		{
			UserIO.WriteLine("Finding leaderboard sizes");

			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (leaderboardIndex != 4 && leaderboardIndex != 5 && leaderboardIndex != 6 && leaderboardIndex != 7 && leaderboardIndex != 51)
				{
					continue;
				}

				var probeResponse = RelicAPI.Leaderboard.RequestById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;

				if (LeaderboardSizes.ContainsKey((LeaderboardId)leaderboardIndex))
				{
					LeaderboardSizes[(LeaderboardId)leaderboardIndex] = leaderboardMaxRank;
				}
				else
				{
					LeaderboardSizes.Add((LeaderboardId)leaderboardIndex, leaderboardMaxRank);
				}

				UserIO.WriteLine(((LeaderboardId)leaderboardIndex).ToString() + " " + LeaderboardSizes[(LeaderboardId)leaderboardIndex]);
			}
		}

		public int GetLeaderboardRankByPercentile(LeaderboardId id, double percentile)
		{
			if (LeaderboardSizes.ContainsKey(id) == false)
			{
				var probeResponse = RelicAPI.Leaderboard.RequestById((int)id, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				LeaderboardSizes.Add(id, leaderboardMaxRank);
			}

			int maxRank = LeaderboardSizes[id];
			double cutoffRank = maxRank * (percentile / 100.0);

			return (int)cutoffRank;
		}
	}
}
