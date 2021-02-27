using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Coh2Stats
{
	internal class Program
	{
		const long relevantTimeCutoff = 1614376800;

		public static void AnalyzeWinRatesByRace(MatchAnalyticsBundle mab, RaceFlag raceFlag)
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
			double totalWinRate = totalWins.Matches.Count / (double)totalGames.Matches.Count;

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

				double winRate = winCount / (double)playCount;
				mapsByWinRate.Add(map, winRate);
			}

			mapsByWinRate = mapsByWinRate.OrderByDescending(m => m.Value).ToDictionary(m => m.Key, m => m.Value);

			Console.WriteLine("{0} win rates:", raceFlag.ToString());
			Console.WriteLine("{0:0.00}     {1} / {2,5}", totalWinRate, totalWins.Matches.Count, totalGames.Matches.Count);
			Console.WriteLine("----------------------------------------------------------------");
			foreach (var m in mapsByWinRate)
			{
				int winCount = 0;
				if (mapsByWinCount.ContainsKey(m.Key))
				{
					winCount = mapsByWinCount[m.Key];
				}

				Console.WriteLine("{0:0.00}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
			}
			Console.WriteLine();
		}

		public static void PrintInformationPerRank(Database db, double percentile)
		{
			Console.WriteLine("Match data for top {0}% of players", percentile);

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db).FilterByStartGameTime((int)relevantTimeCutoff, -1).FilterByDescription("AUTOMATCH").FilterByTopPercentile(db, percentile, true);

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			var playCounts = mab.GetOrderedMapPlayCount();
			foreach (var p in playCounts)
			{
				Console.WriteLine(p.Value + " " + p.Key);
			}
			Console.WriteLine();
		}

		public static int RunModeSelection()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

			int selection;
			do
			{
				Console.Clear();
				Console.WriteLine("1 - Match history logging");
				Console.WriteLine("2 - Match history logging (repeat)");
				Console.WriteLine("3 - Data printing (top players by percentile)");
				Console.Write("Please choose your desired mode: ");

				int.TryParse(Console.ReadLine(), out selection);

			} while (selection != 1 && selection != 2 && selection != 3);

			return selection;
		}

		public static void RunModeOperations(Database db, int selectedMode)
		{
			MatchTypeId matchType = MatchTypeId._1v1_; // TODO

			if (selectedMode == 1)
			{
				db.ProcessPlayers(matchType);
				while (db.ProcessMatches(matchType, relevantTimeCutoff) == true) ;
			}

			if (selectedMode == 2)
			{
				while (true)
				{
					db.ProcessPlayers(matchType);
					while (db.ProcessMatches(matchType, relevantTimeCutoff) == true) ;

					Stopwatch sw = Stopwatch.StartNew();
					double sessionInterval = 1200;

					while (sw.Elapsed.TotalSeconds < sessionInterval)
					{
						double difference = sessionInterval - sw.Elapsed.TotalSeconds;
						int intDiff = (int)difference;

						if (intDiff % 60 == 0)
						{
							Console.WriteLine("Resuming operations in {0:0} seconds", difference);
							Thread.Sleep(1000);
						}
					}
				}
			}

			if (selectedMode == 3)
			{
				Console.Write("Top percentile: ");
				bool goodParse = double.TryParse(Console.ReadLine(), out double percentile);
				Console.Clear();

				var german = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode(RaceId.German, matchType);
				var soviet = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode(RaceId.Soviet, matchType);
				var wgerman = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode(RaceId.WGerman, matchType);
				var aef = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode(RaceId.AEF, matchType);
				var british = LeaderboardCompatibility.GetLeaderboardFromRaceAndMode(RaceId.British, matchType);

				int germanRanks = db.GetLeaderboardRankByPercentile(german, percentile);
				int sovietRanks = db.GetLeaderboardRankByPercentile(soviet, percentile);
				int wgermanRanks = db.GetLeaderboardRankByPercentile(wgerman, percentile);
				int aefRanks = db.GetLeaderboardRankByPercentile(aef, percentile);
				int britishRanks = db.GetLeaderboardRankByPercentile(british, percentile);

				Console.WriteLine("{0} ranks: top {1}", german.ToString(), germanRanks);
				Console.WriteLine("{0} ranks: top {1}", soviet.ToString(), sovietRanks);
				Console.WriteLine("{0} ranks: top {1}", wgerman.ToString(), wgermanRanks);
				Console.WriteLine("{0} ranks: top {1}", aef.ToString(), aefRanks);
				Console.WriteLine("{0} ranks: top {1}", british.ToString(), britishRanks);

				if (goodParse)
				{
					PrintInformationPerRank(db, percentile);
					Console.ReadLine();
				}
			}
		}

		private static void Main()
		{
			Database db = new Database();
			db.LoadPlayerDatabase();
			db.LoadMatchDatabase();

			while (true)
			{
				try
				{
					int selectedMode = RunModeSelection();
					RunModeOperations(db, selectedMode);
				}

				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.ReadLine();
				}
			}
		}
	}
}