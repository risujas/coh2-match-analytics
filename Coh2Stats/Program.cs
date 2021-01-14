using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Coh2Stats
{
	class Program
	{
		static public void AnalyzeWinRatesByRace(MatchAnalyticsBundle mab, RaceFlag raceFlag)
		{
			FactionId factionId = FactionId.Allies;
			if (raceFlag == RaceFlag.German || raceFlag == RaceFlag.WGerman)
			{
				factionId = FactionId.Axis;
			}

			var totalGames = mab.FilterByRace(raceFlag);
			if (totalGames.Matches.Count == 0)
			{
				return;
			}

			var totalWins = totalGames.FilterByResult(true, factionId);
			double totalWinRate = (double)totalWins.Matches.Count / (double)totalGames.Matches.Count;

			var mapsByPlayCount = totalGames.GetOrderedMapPlayCount();
			var mapsByWinCount = totalWins.GetOrderedMapPlayCount();
			Dictionary<string, double> mapsByWinRate = new Dictionary<string, double>();

			foreach (var m in mapsByPlayCount)
			{
				string map = m.Key;
				int playCount = m.Value;

				int winCount = 0;
				if (mapsByWinCount.ContainsKey(map))
				{
					winCount = mapsByWinCount[map];
				}

				double winRate = (double)winCount / (double)playCount;
				mapsByWinRate.Add(map, winRate);
			}

			mapsByWinRate = mapsByWinRate.OrderByDescending(m => m.Value).ToDictionary(m => m.Key, m => m.Value);

			Console.WriteLine("{0} win rates:", raceFlag.ToString());
			Console.WriteLine("{0:0.00} ({1}/{2})\toverall", totalWinRate, totalWins.Matches.Count, totalGames.Matches.Count);
			Console.WriteLine("--------------------------------");
			foreach (var m in mapsByWinRate)
			{
				int winCount = 0;
				if (mapsByWinCount.ContainsKey(m.Key))
				{
					winCount = mapsByWinCount[m.Key];
				}

				Console.WriteLine("{0:0.00} ({2}/{3})\t{1}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
			}
			Console.WriteLine();
		}

		static void Main()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

			Database db = new Database();
			db.LoadPlayerDatabase();
			db.LoadMatchDatabase();

			db.FindNewPlayers(MatchTypeId._1v1_);
			
			while(true)
			{
				db.ProcessMatches();
			}

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db)
				.FilterByMatchType(MatchTypeId._1v1_).FilterByDescription("AUTOMATCH");

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			Console.ReadLine();
		}
	}
}