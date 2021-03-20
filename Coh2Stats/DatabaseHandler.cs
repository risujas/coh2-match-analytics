using System;
using System.Collections.Generic;
using System.IO;

namespace Coh2Stats
{
	public static class DatabaseHandler
	{
		public static readonly PlayerDatabase PlayerDb = new PlayerDatabase();
		public static readonly MatchDatabase MatchDb = new MatchDatabase();
		public static MatchTypeId LoadedDataSet;

		public static string ApplicationDataFolder
		{
			get;
		} = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";

		public static string DatabaseFolder
		{
			get;
		} = ApplicationDataFolder + "\\databases";

		static DatabaseHandler()
		{
			Directory.CreateDirectory(ApplicationDataFolder);
			Directory.CreateDirectory(DatabaseFolder);
		}

		public static void Load(MatchTypeId gameMode)
		{
			PlayerDb.FindLeaderboardSizes(gameMode);

			PlayerDb.Load(DatabaseFolder);
			MatchDb.Load(DatabaseFolder, gameMode);

			LoadedDataSet = gameMode;
		}

		public static void ParseAndProcess(MatchTypeId gameMode)
		{
			ProcessPlayers(gameMode);
			ProcessMatches(gameMode, Program.RelevantTimeCutoffSeconds);
		}

		private static void ProcessPlayers(MatchTypeId gameMode)
		{
			PlayerDb.FindNewPlayers(gameMode, 1, -1);
			PlayerDb.UpdatePlayerDetails(gameMode);

			PlayerDb.Write(DatabaseFolder);
		}

		private static void ProcessMatches(MatchTypeId gameMode, long startedAfterTimestamp)
		{
			var playersToBeProcessed = PlayerDb.GetRankedPlayersFromDatabase(gameMode);
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
					if (x.MatchTypeId == (int)gameMode && x.StartGameTime >= startedAfterTimestamp)
					{
						MatchDb.LogMatch(x);
					}
				}

				UserIO.AllowPause();
			}

			int newMatchCount = MatchDb.MatchData.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLine("{0} new matches found", difference);

			PlayerDb.Write(DatabaseFolder);
			MatchDb.Write(DatabaseFolder, gameMode);
		}
	}
}
