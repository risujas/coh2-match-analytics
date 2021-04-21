﻿using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats_Net5
{
	public static class DatabaseHandler
	{
		public static readonly PlayerDatabase PlayerDb = new PlayerDatabase();
		public static readonly MatchDatabase MatchDb = new MatchDatabase();
		public const int MaxBatchSize = 100;

		public static void Load()
		{
			PlayerDb.FindLeaderboardSizes();

			MatchDb.Load();
			UserIO.WriteLine("Highest post-load match ID: " + MatchDb.GetHighestId());
		}

		public static void Process()
		{
			PlayerDb.FindPlayerNames();
			PlayerDb.FindPlayerDetails();

			ProcessMatches();
			UserIO.WriteLine("Highest post-process match ID: " + MatchDb.GetHighestId());
		}

		private static void ProcessMatches()
		{
			var playersToBeProcessed = PlayerDb.PlayerIdentities.ToList();
			int oldMatchCount = MatchDb.MatchData.Count;

			while (playersToBeProcessed.Count > 0)
			{
				int batchSize = MaxBatchSize;
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
					MatchDb.LogMatch(x);
				}

				UserIO.AllowPause();
			}

			int newMatchCount = MatchDb.MatchData.Count;
			int difference = newMatchCount - oldMatchCount;
			UserIO.WriteLine("{0} new matches found", difference);

			MatchDb.Write();
		}
	}
}