using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Coh2Stats
{
	class MatchDatabase
	{
		public List<RelicAPI.RecentMatchHistory.MatchHistoryStat> MatchData = new List<RelicAPI.RecentMatchHistory.MatchHistoryStat>();

		public const string DbFile = "matchData.json";

		public bool Load()
		{
			string fullPath = DatabaseHandler.DatabaseFolder + "\\" + DbFile;
			if (!File.Exists(fullPath))
			{
				return false;
			}

			UserIO.WriteLine("Loading match data");

			string text = File.ReadAllText(fullPath);
			MatchData = JsonConvert.DeserializeObject<List<RelicAPI.RecentMatchHistory.MatchHistoryStat>>(text);

			UserIO.WriteLine("{0} match history stats", MatchData.Count);

			return true;
		}

		public void Write()
		{
			UserIO.WriteLine("Writing match data");

			var text = JsonConvert.SerializeObject(MatchData, Formatting.Indented);
			string fullPath = DatabaseHandler.DatabaseFolder + "\\" + DbFile;
			File.WriteAllText(fullPath, text);
		}

		public void LogMatch(RelicAPI.RecentMatchHistory.MatchHistoryStat matchHistoryStat)
		{
			if (GetMatchById(matchHistoryStat.Id) == null)
			{
				foreach (var p in matchHistoryStat.MatchHistoryReportResults)
				{
					var player = DatabaseHandler.PlayerDb.GetPlayerByProfileId(p.ProfileId);
					var lbd = LeaderboardCompatibility.GetLeaderboardByRaceId((RaceId)p.RaceId);
					var stat = DatabaseHandler.PlayerDb.GetLeaderboardStatByStatGroupId(player.PersonalStatGroupId, lbd);
					
					int rank = -1;
					if (stat != null)
					{
						rank = stat.Rank;
					}

					p.Rank = rank;
					p.Alias = player.Alias;
					p.RankTotal = DatabaseHandler.LeaderboardSizes[(int)lbd];
				}

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
