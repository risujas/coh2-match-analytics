﻿using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

using static Coh2Stats_Net5.RelicAPI.RecentMatchHistory;

namespace Coh2Stats_Net5
{
	public class MatchDatabase
	{
		public List<MatchHistoryStat> MatchData = new List<MatchHistoryStat>();

		public const string DbFile = "matchData.json";

		public bool Load()
		{
			string fullPath = Program.DatabaseFolder + "\\" + DbFile;
			if (!File.Exists(fullPath))
			{
				return false;
			}

			UserIO.WriteLine("Loading match data");

			string text = File.ReadAllText(fullPath);
			var loadedMatches = JsonConvert.DeserializeObject<List<MatchHistoryStat>>(text);

			UserIO.WriteLine("Validating match data");
			foreach (var lm in loadedMatches)
			{
				if (ValidateMatch(lm, Program.MatchDiscardCutoff))
				{
					MatchData.Add(lm);
				}
			}

			Write();

			UserIO.WriteLine("{0} match history stats", MatchData.Count);

			return true;
		}

		public void Write()
		{
			UserIO.WriteLine("Writing match data");

			var text = JsonConvert.SerializeObject(MatchData, Formatting.Indented);
			string fullPath = Program.DatabaseFolder + "\\" + DbFile;
			File.WriteAllText(fullPath, text);
		}

		public void LogMatch(MatchHistoryStat matchHistoryStat)
		{
			if (GetMatchById(matchHistoryStat.Id) == null && ValidateMatch(matchHistoryStat, Program.MatchLoggingCutoff))
			{
				for (int i = 0; i < matchHistoryStat.MatchHistoryReportResults.Count; i++)
				{
					var r = matchHistoryStat.MatchHistoryReportResults[i];

					var player = DatabaseHandler.PlayerDb.GetPlayerByProfileId(r.ProfileId);
					var playerLbd = LeaderboardCompatibility.GetLeaderboardByRace((RaceId)r.RaceId);
					var playerStat = DatabaseHandler.PlayerDb.GetLeaderboardStat(player.PersonalStatGroupId, playerLbd);

					r.RankTotal = DatabaseHandler.PlayerDb.LeaderboardSizes[playerLbd];

					if (playerStat == null)
					{
						r.Rank = -1;
					}

					else
					{
						r.Rank = playerStat.Rank;
					}
				}

				MatchData.Add(matchHistoryStat);
			}
		}

		public MatchHistoryStat GetMatchById(int id)
		{
			for (int i = 0; i < MatchData.Count; i++)
			{
				var x = MatchData[i];

				if (x.Id == id)
				{
					return x;
				}
			}

			return null;
		}

		public int GetHighestId()
		{
			int highest = 0;

			foreach (var md in MatchData)
			{
				if (md.Id >= highest)
				{
					highest = md.Id;
				}
			}

			return highest;
		}

		public bool ValidateMatch(MatchHistoryStat matchHistoryStat, long startTimeCutoff)
		{
			if (matchHistoryStat.MatchTypeId != 1)
			{
				return false;
			}

			if (matchHistoryStat.StartGameTime < startTimeCutoff)
			{
				return false;
			}

			if (matchHistoryStat.Description != "AUTOMATCH")
			{
				return false;
			}

			for (int i = 0; i < matchHistoryStat.MatchHistoryReportResults.Count; i++)
			{
				var r = matchHistoryStat.MatchHistoryReportResults[i];
				if (r.ResultType != 0 && r.ResultType != 1)
				{
					return false;
				}
			}

			return true;
		}
	}
}