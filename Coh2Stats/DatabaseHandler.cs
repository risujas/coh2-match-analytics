﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Coh2Stats
{
	public static class DatabaseHandler
	{
		public static readonly PlayerDatabase PlayerDb = new PlayerDatabase();
		public static readonly MatchDatabase MatchDb = new MatchDatabase();

		public static void Load()
		{
			PlayerDb.FindLeaderboardSizes();

			PlayerDb.Load();
			MatchDb.Load();
		}

		public static void ParseAndProcess()
		{
			ProcessPlayers();
			ProcessMatches();
		}

		private static void ProcessPlayers()
		{
			PlayerDb.FindNewPlayers(1, -1);
			PlayerDb.UpdatePlayerDetails();

			PlayerDb.Write();
		}

		private static void ProcessMatches()
		{
			var playersToBeProcessed = PlayerDb.GetRankedPlayersFromDatabase();
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
					if (x.MatchTypeId == 1 && x.StartGameTime >= Program.RelevantTimeCutoffSeconds)
					{
						MatchDb.LogMatch(x);
					}
				}

				UserIO.AllowPause();
			}

			int newMatchCount = MatchDb.MatchData.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLine("{0} new matches found", difference);

			PlayerDb.Write();
			MatchDb.Write();
		}
	}
}
