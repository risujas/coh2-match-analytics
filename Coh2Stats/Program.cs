using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.IO;

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

			UserIO.WriteLogLine("");
			UserIO.WriteLogLine("{0} win rates:", raceFlag.ToString());
			UserIO.WriteLogLine("{0:0.00}     {1} / {2,5}", totalWinRate, totalWins.Matches.Count, totalGames.Matches.Count);
			UserIO.WriteLogLine("----------------------------------------------------------------");
			foreach (var m in mapsByWinRate)
			{
				int winCount = 0;
				if (mapsByWinCount.ContainsKey(m.Key))
				{
					winCount = mapsByWinCount[m.Key];
				}

				UserIO.WriteLogLine("{0:0.00}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
			}
		}

		public static void SaveResultsToFile(string fileName)
		{
			string finalFolder = Database.DatabaseFolder + "\\exports";

			Directory.CreateDirectory(Database.DatabaseFolder);
			Directory.CreateDirectory(finalFolder);

			File.Create(finalFolder + "\\" + fileName + ".txt");

			// TODO implement this
		}

		public static void RunInteractiveAnalysis(Database db, MatchAnalyticsBundle mab, string filterHistory = "")
		{
			UserIO.WriteLogLine("Filter history: " + filterHistory);

			AnalyzeWinRatesByRace(mab, RaceFlag.German);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
			AnalyzeWinRatesByRace(mab, RaceFlag.British);

			UserIO.WriteLogLine("");

			var playCounts = mab.GetOrderedMapPlayCount();
			foreach (var p in playCounts)
			{
				UserIO.WriteLogLine(p.Value + " " + p.Key);
			}

			UserIO.WriteLogLine("");

			UserIO.PrintUIPrompt("Q - Finish running the interactive analysis");
			UserIO.PrintUIPrompt("S - Export the current results into a file");
			UserIO.PrintUIPrompt("1 - Filter by top percentile of players");
			UserIO.PrintUIPrompt("2 - Filter by faction");
			UserIO.PrintUIPrompt("Please select an operation.");

			char operation = UserIO.RunCharSelection('Q', 'S', '1', '2', '3');
			operation = char.ToLower(operation);

			if (operation == 'q')
			{
				return;
			}

			if (operation == 's')
			{
				DateTime dt = DateTime.UtcNow;
				DateTimeOffset dto = new DateTimeOffset(dt);
				long unixTime = dto.ToUnixTimeSeconds();

				string fileName = unixTime.ToString() + filterHistory;
				SaveResultsToFile(fileName);

				RunInteractiveAnalysis(db, mab, filterHistory);
			}

			if (operation == '1')
			{
				UserIO.PrintUIPrompt("Please select the percentile of top players to be included in the results.");
				double percentile = UserIO.RunFloatingPointInput();

				mab = mab.FilterByTopPercentile(db, percentile, true);
				RunInteractiveAnalysis(db, mab, filterHistory + ",t%-" + percentile);
			}

			if (operation == '2')
			{
				UserIO.PrintUIPrompt("W - Wehrmacht Ostheer");
				UserIO.PrintUIPrompt("S - Soviet Union");
				UserIO.PrintUIPrompt("U - United States Forces");
				UserIO.PrintUIPrompt("O - Oberkommando West");
				UserIO.PrintUIPrompt("B - British Forces");
				UserIO.PrintUIPrompt("Input the factions you want to be included in the results. You can input multiple factions by separating the characters with commas or spaces.");

				bool goodParse = false;
				List<string> partsList = null;

				while (!goodParse)
				{
					string input = UserIO.RunStringInput();
					var parts = input.Split(',', ' ');

					for (int i = 0; i < parts.Length; i++)
					{
						parts[i] = parts[i].ToLower().Trim();
					}

					partsList = parts.ToList();

					if (partsList.Contains("w") || partsList.Contains("s") || partsList.Contains("u") || partsList.Contains("o") || partsList.Contains("b"))
					{
						goodParse = true;
					}
				}

				if (partsList.Contains("w"))
				{
					mab = mab.FilterByRace(RaceFlag.German);
				}

				if (partsList.Contains("s"))
				{
					mab = mab.FilterByRace(RaceFlag.Soviet);
				}

				if (partsList.Contains("u"))
				{
					mab = mab.FilterByRace(RaceFlag.AEF);
				}

				if (partsList.Contains("o"))
				{
					mab = mab.FilterByRace(RaceFlag.WGerman);
				}

				if (partsList.Contains("b"))
				{
					mab = mab.FilterByRace(RaceFlag.British);
				}

				RunInteractiveAnalysis(db, mab, filterHistory + ",r-" + string.Join("", partsList));
			}
		}

		public static MatchTypeId RunGameModeSelection()
		{
			UserIO.PrintUIPrompt("1 - 1v1 automatch");
			UserIO.PrintUIPrompt("2 - 2v2 automatch");
			UserIO.PrintUIPrompt("3 - 3v3 automatch");
			UserIO.PrintUIPrompt("4 - 4v4 automatch");
			UserIO.PrintUIPrompt("Please select a game mode.");

			return (MatchTypeId)UserIO.RunIntegerSelection(1, 4);
		}

		public static int RunOperatingModeSelection()
		{
			UserIO.PrintUIPrompt("1 - Match logging");
			UserIO.PrintUIPrompt("2 - Match logging (repeating)");
			UserIO.PrintUIPrompt("3 - Match analysis");
			UserIO.PrintUIPrompt("Please select an operating mode.");

			return UserIO.RunIntegerSelection(1, 4);
		}

		public static void RunModeOperations(Database db, int operatingMode, MatchTypeId gameMode)
		{
			if (operatingMode == 1)
			{
				db.ProcessPlayers(gameMode);
				db.ProcessMatches(gameMode, relevantTimeCutoff);
			}

			if (operatingMode == 2)
			{
				while (true)
				{
					db.ProcessPlayers(gameMode);
					db.ProcessMatches(gameMode, relevantTimeCutoff);

					Stopwatch sw = Stopwatch.StartNew();
					double sessionInterval = 1200;

					while (sw.Elapsed.TotalSeconds < sessionInterval)
					{
						double difference = sessionInterval - sw.Elapsed.TotalSeconds;
						int intDiff = (int)difference;

						if (intDiff % 60 == 0)
						{
							UserIO.WriteLogLine("Resuming operations in {0:0} seconds. You can press CTRL + C to exit this program.", difference);
							Thread.Sleep(1000);
						}
					}
				}
			}

			if (operatingMode == 3)
			{
				MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches(db);
				RunInteractiveAnalysis(db, mab);
			}
		}

		private static void Main()
		{
			var culture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = culture;

			DateTime dt = DateTime.Now;
			UserIO.WriteLogLine(dt.ToShortDateString() + " " + dt.ToShortTimeString());

			MatchTypeId gameMode = RunGameModeSelection();

			Database db = new Database();
			db.LoadPlayerDatabase();
			db.LoadMatchDatabase(gameMode);

			while (true)
			{
				int operatingMode = RunOperatingModeSelection();
				RunModeOperations(db, operatingMode, gameMode);
			}
		}
	}
}