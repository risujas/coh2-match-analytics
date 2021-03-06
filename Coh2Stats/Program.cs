using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Coh2Stats
{
	internal class Program
	{
		private const long relevantTimeCutoff = 1614376800;

		public static RaceFlag GenerateRaceFlag(bool includeGerman, bool includeSoviet, bool includeAEF, bool includeWGerman, bool includeBritish)
		{
			RaceFlag germans = RaceFlag.None;
			RaceFlag soviets = RaceFlag.None;
			RaceFlag aef = RaceFlag.None;
			RaceFlag wgermans = RaceFlag.None;
			RaceFlag british = RaceFlag.None;

			if (includeGerman)
			{
				germans = RaceFlag.German;
			}

			if (includeSoviet)
			{
				soviets = RaceFlag.Soviet;
			}

			if (includeAEF)
			{
				aef = RaceFlag.AEF;
			}

			if (includeWGerman)
			{
				wgermans = RaceFlag.WGerman;
			}

			if (includeBritish)
			{
				british = RaceFlag.British;
			}

			RaceFlag combinedFlags = germans | soviets | aef | wgermans | british;
			return combinedFlags;
		}
		public static void AnalyzeWinRatesByRace(MatchAnalyticsBundle mab, RaceFlag raceFlag, string destination = "")
		{
			FactionId factionId = FactionId.Allies;
			if (raceFlag == RaceFlag.German || raceFlag == RaceFlag.WGerman)
			{
				factionId = FactionId.Axis;
			}

			var allGames = mab.FilterByRequiredRaces(raceFlag);
			if (allGames.Matches.Count == 0)
			{
				return;
			}

			var wonGames = allGames.FilterByResult(true, factionId);
			double totalWinRate = wonGames.Matches.Count / (double)allGames.Matches.Count;

			var mapsByPlayCount = allGames.GetOrderedMapPlayCount();
			var mapsByWinCount = wonGames.GetOrderedMapPlayCount();
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

			if (destination == "")
			{
				UserIO.WriteLogLine("");
				UserIO.WriteLogLine("{0} win rates:", raceFlag.ToString());
				UserIO.WriteLogLine("{0:0.00}     {1} / {2,5}", totalWinRate, wonGames.Matches.Count, allGames.Matches.Count);
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

			else
			{
				using (StreamWriter file = File.AppendText(destination))
				{
					file.WriteLine("{0} win rates:", raceFlag.ToString());
					file.WriteLine("{0:0.00}     {1} / {2,5}", totalWinRate, wonGames.Matches.Count, allGames.Matches.Count);
					file.WriteLine("----------------------------------------------------------------");

					foreach (var m in mapsByWinRate)
					{
						int winCount = 0;
						if (mapsByWinCount.ContainsKey(m.Key))
						{
							winCount = mapsByWinCount[m.Key];
						}

						file.WriteLine("{0:0.00}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
					}

					file.WriteLine();
				}
			}
		}

		public static void SaveResultsToFile(MatchAnalyticsBundle mab, string fileName)
		{
			string finalFolder = Database.ApplicationDataFolder + "\\exports";

			Directory.CreateDirectory(Database.ApplicationDataFolder);
			Directory.CreateDirectory(finalFolder);

			string filePath = finalFolder + "\\" + fileName + ".txt";
			File.Delete(filePath);

			AnalyzeWinRatesByRace(mab, RaceFlag.German, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.British, filePath);
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
				SaveResultsToFile(mab, fileName);

				RunInteractiveAnalysis(db, mab, filterHistory);
			}

			if (operation == '1')
			{
				UserIO.PrintUIPrompt("Please select the percentile of top players to be included in the results.");
				double percentile = UserIO.RunFloatingPointInput();

				mab = mab.FilterByTopPercentile(db, percentile, true);
				RunInteractiveAnalysis(db, mab, filterHistory + ",tp-" + percentile);
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

				RaceFlag flags = GenerateRaceFlag(partsList.Contains("w"), partsList.Contains("s"), partsList.Contains("u"), partsList.Contains("o"), partsList.Contains("b"));
				mab = mab.FilterByAllowedRaces(flags);

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