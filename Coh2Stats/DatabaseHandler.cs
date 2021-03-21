using System;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	static class DatabaseHandler
	{
		public static readonly PlayerDatabase PlayerDb = new PlayerDatabase();
		public static readonly MatchDatabase MatchDb = new MatchDatabase();
		public static Dictionary<int, int> LeaderboardSizes = new Dictionary<int, int>();

		public static string DatabaseFolder
		{
			get;
		} = Program.ApplicationDataFolder + "\\databases";

		public static void Unload()
		{
			PlayerDb.PlayerIdentities.Clear();
			PlayerDb.StatGroups.Clear();
			PlayerDb.LeaderboardStats.Clear();

			MatchDb.MatchData.Clear();

			LeaderboardSizes.Clear();
		}

		public static void Load()
		{
			GetLeaderboardSizes();

			MatchDb.Load();
		}

		public static void ProcessPlayers()
		{
			PlayerDb.FindPlayerNames();
			PlayerDb.FindPlayerStats();
		}

		public static void ProcessMatches()
		{
			var playersToBeProcessed = PlayerDb.PlayerIdentities.ToList();
			int oldMatchCount = MatchDb.MatchData.Count;

			while (playersToBeProcessed.Count > 0)
			{
				int batchSize = 200;
				if (playersToBeProcessed.Count < batchSize)
				{
					batchSize = playersToBeProcessed.Count;
				}

				UserIO.WriteLine("Retrieving match history for {0} players", playersToBeProcessed.Count);

				var range = playersToBeProcessed.GetRange(0, batchSize);
				playersToBeProcessed.RemoveRange(0, batchSize);

				List<int> profileIds = new List<int>();
				for (int i = 0; i < range.Count; i++)
				{
					var x = range[i];
					profileIds.Add(x.ProfileId);
				}

				var response = RelicAPI.RecentMatchHistory.RequestByProfileId(profileIds);

				for (int i = 0; i < response.Profiles.Count; i++)
				{
					var x = response.Profiles[i];
					PlayerDb.LogPlayer(x);
				}

				for (int i = 0; i < response.MatchHistoryStats.Count; i++)
				{
					var x = response.MatchHistoryStats[i];
					
					if (x.MatchTypeId != 1)
					{
						continue;
					}

					DateTime dt = DateTime.UtcNow.AddDays(-1);
					DateTimeOffset dto = new DateTimeOffset(dt);
					long unixTimeCutoff = dto.ToUnixTimeSeconds();

					if (x.StartGameTime < unixTimeCutoff)
					{
						continue;
					}

					MatchDb.LogMatch(x);
				}

				UserIO.AllowPause();
			}

			int newMatchCount = MatchDb.MatchData.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLine("{0} new matches found", difference);

			MatchDb.Write();
		}

		private static void GetLeaderboardSizes()
		{
			UserIO.WriteLine("Finding leaderboard sizes");

			for (int i = 0; i < 100; i++)
			{
				if (i != 4 && i != 5 && i != 6 && i != 7 && i != 51)
				{
					continue;
				}

				var response = RelicAPI.Leaderboard.RequestById(i, 1, 1);
				LeaderboardSizes.Add(i, response.RankTotal);

				UserIO.WriteLine("#{0}: {1}", i, response.RankTotal);
			}
		}
	}
}
