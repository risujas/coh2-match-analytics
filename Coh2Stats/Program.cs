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
		private const long relevantTimeCutoff = 1614376800;

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

			UserIO.PrintUILine("{0} win rates:", raceFlag.ToString());
			UserIO.PrintUILine("{0:0.00}     {1} / {2,5}", totalWinRate, totalWins.Matches.Count, totalGames.Matches.Count);
			UserIO.PrintUILine("----------------------------------------------------------------");
			foreach (var m in mapsByWinRate)
			{
				int winCount = 0;
				if (mapsByWinCount.ContainsKey(m.Key))
				{
					winCount = mapsByWinCount[m.Key];
				}

				UserIO.PrintUILine("{0:0.00}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
			}
		}

		public static void PrintInformationPerRank(Database db, double percentile)
		{
			UserIO.PrintUILine("Match data for top {0}% of players", percentile);

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db).FilterByStartGameTime((int)relevantTimeCutoff, -1).FilterByDescription("AUTOMATCH").FilterByTopPercentile(db, percentile, true);

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			var playCounts = mab.GetOrderedMapPlayCount();
			foreach (var p in playCounts)
			{
				UserIO.PrintUILine(p.Value + " " + p.Key);
			}
		}

		public static int RunModeSelection()
		{
			UserIO.PrintUILine("1 - Match history logging");
			UserIO.PrintUILine("2 - Match history logging (repeat)");
			UserIO.PrintUILine("3 - Data printing (top players by percentile)");
			UserIO.PrintUILine("Please choose an operating mode.");

			return UserIO.RunIntegerSelection(1, 3);
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
							UserIO.WriteLogLine("Resuming operations in {0:0} seconds", difference);
							Thread.Sleep(1000);
						}
					}
				}
			}

			if (selectedMode == 3)
			{
				UserIO.PrintUILine("Top percentile: ");
				double percentile = UserIO.RunFloatingPointInput();

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

				UserIO.PrintUILine("{0} ranks: top {1}", german.ToString(), germanRanks);
				UserIO.PrintUILine("{0} ranks: top {1}", soviet.ToString(), sovietRanks);
				UserIO.PrintUILine("{0} ranks: top {1}", wgerman.ToString(), wgermanRanks);
				UserIO.PrintUILine("{0} ranks: top {1}", aef.ToString(), aefRanks);
				UserIO.PrintUILine("{0} ranks: top {1}", british.ToString(), britishRanks);

				PrintInformationPerRank(db, percentile);
			}
		}

		private static void Main()
		{
			Database db = new Database();
			db.LoadPlayerDatabase();
			db.LoadMatchDatabase();

			while (true)
			{
				int selectedMode = RunModeSelection();
				RunModeOperations(db, selectedMode);
			}
		}
	}
}