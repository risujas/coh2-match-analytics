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
			var totalWins = totalGames.FilterByResult(true, factionId);
			double totalWinRate = (double)totalWins.Matches.Count / (double)totalGames.Matches.Count;

			var mapsByPlayCount = totalGames.GetOrderedMapPlayCount();
			var mapsByWinCount = totalWins.GetOrderedMapPlayCount();
			Dictionary<string, double> mapsByWinRate = new Dictionary<string, double>();

			foreach (var m in mapsByPlayCount)
			{
				string map = m.Key;
				int playCount = m.Value;
				int winCount = mapsByWinCount[map];
				double winRate = (double)winCount / (double)playCount;
				mapsByWinRate.Add(map, winRate);
			}

			mapsByWinRate = mapsByWinRate.OrderByDescending(m => m.Value).ToDictionary(m => m.Key, m => m.Value);

			Console.WriteLine("{0} win rates:", raceFlag.ToString());
			Console.WriteLine("{0:0.000} overall ({1} / {2})", totalWinRate, totalWins.Matches.Count, totalGames.Matches.Count);
			Console.WriteLine("--------------------------------");
			foreach (var m in mapsByWinRate)
			{
				Console.WriteLine("{0:0.000} {1}", m.Value, m.Key);
			}
			Console.WriteLine();
		}

		static void Main()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

			Database db = new Database();
			db.LoadFromFile();

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db)
				.FilterByMatchType(MatchTypeId._1v1_).FilterByCompletionTime(1593561600, 1610599515).FilterByDescription("AUTOMATCH");

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			Console.ReadLine();
		}
	}
}