using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
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

		static public void PrintInformationPerRank(Database db, int rankFloor, int rankCap)
		{
			Console.WriteLine("Ranks {0} - {1}", rankFloor, rankCap);

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db).FilterByStartGameTime(1612389600, -1).FilterByDescription("AUTOMATCH").FilterByRank(db, rankFloor, rankCap, true);

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			Console.WriteLine();
			var playCounts = mab.GetOrderedMapPlayCount();
			foreach (var p in playCounts)
			{
				Console.WriteLine(p.Value + " " + p.Key);
			}
			Console.WriteLine();
		}

		static public int ModeSelection()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

			int selection;
			do
			{
				Console.Clear();
				Console.WriteLine("1 - Match history logging");
				Console.WriteLine("2 - Match history logging (repeat)");
				Console.WriteLine("3 - Data printing");
				Console.Write("Please choose your desired mode: ");

				int.TryParse(Console.ReadLine(), out selection);

			} while (selection != 1 && selection != 2 && selection != 3);

			return selection;
		}

		static void Main()
		{
			Database db = new Database();
			db.LoadPlayerDatabase();
			db.LoadMatchDatabase();

			while (true)
			{
				int selection = ModeSelection();

				if (selection == 1)
				{
					db.FindNewPlayers(MatchTypeId._1v1_);
					while (db.ProcessMatches(MatchTypeId._1v1_, 50000) == true);
				}

				if (selection == 2)
				{
					while (true)
					{
						db.FindNewPlayers(MatchTypeId._1v1_);
						while (db.ProcessMatches(MatchTypeId._1v1_, 50000) == true);

						Stopwatch sw = Stopwatch.StartNew();
						double operationInterval = 1200;

						while (sw.Elapsed.TotalSeconds < operationInterval)
						{
							double difference = operationInterval - sw.Elapsed.TotalSeconds;
							Console.WriteLine("Resuming operations in {0} seconds", difference);
						}
					}
				}

				if (selection == 3)
				{
					int rankFloor = 0;
					int rankCap = 0;

					Console.Write("Rank floor: ");
					bool goodParse1 = int.TryParse(Console.ReadLine(), out rankFloor);

					Console.Write("Rank cap: ");
					bool goodParse2 = int.TryParse(Console.ReadLine(), out rankCap);

					if (goodParse1 && goodParse2)
					{
						PrintInformationPerRank(db, rankFloor, rankCap);
						Console.ReadLine();
					}
				}
			}
		}
	}
}