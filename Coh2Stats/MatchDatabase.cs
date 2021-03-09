﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class MatchDatabase
	{
		public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> MatchData = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		public const string DbFile = "matchData.txt";

		public bool Load(string dbFolder, MatchTypeId gameMode)
		{
			string fullPath = dbFolder + "\\" + gameMode.ToString() + DbFile;
			if (!File.Exists(fullPath))
			{
				UserIO.WriteLogLine("No match database found");
				return false;
			}

			UserIO.WriteLogLine("Match database found");

			string text = File.ReadAllText(fullPath);
			MatchData = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			UserIO.WriteLogLine("{0} match history stats", MatchData.Count);

			return true;
		}

		public void Write(string dbFolder, MatchTypeId gameMode)
		{
			UserIO.WriteLogLine("Writing match database");

			var text = JsonConvert.SerializeObject(MatchData, Formatting.Indented);
			string fullPath = dbFolder + "\\" + gameMode.ToString() + DbFile;
			File.WriteAllText(fullPath, text);
		}

		public void LogMatch(RelicAPI.RecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetMatchById(matchHistoryStat.Id) == null)
			{
				MatchData.Add(matchHistoryStat);
			}
		}

		public RelicAPI.RecentMatchHistory.MatchHistoryStat GetMatchById(int id)
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
	}
}