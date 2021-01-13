using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class MatchHistoryTracker
	{
		private const string matchHistoryDataFile = "matches.txt";

		private static List<JsonRecentMatchHistory.MatchHistoryStat> matches = new List<JsonRecentMatchHistory.MatchHistoryStat>();
		public static ReadOnlyCollection<JsonRecentMatchHistory.MatchHistoryStat> Matches
		{
			get { return matches.AsReadOnly(); }
		}

		public static void LogMatch(JsonRecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetById(matchHistoryStat.Id) == null)
			{
				matches.Add(matchHistoryStat);
			}
		}

		public static JsonRecentMatchHistory.MatchHistoryStat GetById(int id)
		{
			foreach (var m in matches)
			{
				if (m.Id == id)
				{
					return m;
				}
			}

			return null;
		}

		public static int GetNumLoggedMatches()
		{
			return matches.Count;
		}

		public static bool LoadMatchData()
		{
			if (!File.Exists(matchHistoryDataFile))
			{
				Console.WriteLine("No match history data file exists");
				return false;
			}

			Console.WriteLine("Loading match history data file");
			string text = File.ReadAllText(matchHistoryDataFile);
			matches = JsonConvert.DeserializeObject<List<JsonRecentMatchHistory.MatchHistoryStat>>(text);

			return true;
		}

		public static void WriteMatchData()
		{
			var text = JsonConvert.SerializeObject(matches, Formatting.Indented);
			File.WriteAllText(matchHistoryDataFile, text);
		}
	}
}
