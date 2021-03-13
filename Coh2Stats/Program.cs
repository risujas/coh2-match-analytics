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
		private const string factionFilterTag = "fac";
		private const string inclusivePercentileFilterTag = "%in";
		private const string exclusivePercentileFilterTag = "%ex";
		private const string ageFilterTag = "age";

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
			string finalFolder = DatabaseHandler.ApplicationDataFolder + "\\exports";

			Directory.CreateDirectory(DatabaseHandler.ApplicationDataFolder);
			Directory.CreateDirectory(finalFolder);

			string filePath = finalFolder + "\\" + fileName + ".txt";
			File.Delete(filePath);

			AnalyzeWinRatesByRace(mab, RaceFlag.German, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman, filePath);
			AnalyzeWinRatesByRace(mab, RaceFlag.British, filePath);
		}

		public static void RunInteractiveAnalysis(DatabaseHandler db, MatchAnalyticsBundle mab, string filterHistory = "")
		{
			while (true)
			{
				mab = MatchAnalyticsBundle.GetAllLoggedMatches(db);

				UserIO.WriteLogLine("Filter history: " + filterHistory);

				var filters = filterHistory.Split(',').ToList();
				if (filters.Count > 0)
				{
					filters.RemoveAt(0);

					foreach (var f in filters)
					{
						var parts = f.Split('-');
						var first = parts[0];
						var second = parts[1];

						if (first == factionFilterTag)
						{
							RaceFlag flags = GenerateRaceFlag(second.Contains("w"), second.Contains("s"), second.Contains("u"), second.Contains("o"), second.Contains("b"));
							mab = mab.FilterByAllowedRaces(flags);
						}

						if (first == inclusivePercentileFilterTag)
						{
							mab = mab.FilterByPercentile(db, double.Parse(second), true, true);
						}

						if (first == exclusivePercentileFilterTag)
						{
							mab = mab.FilterByPercentile(db, double.Parse(second), true, false);
						}

						if (first == ageFilterTag)
						{
							mab = mab.FilterByMaxAgeInHours(int.Parse(second));
						}
					}
				}

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
				UserIO.PrintUIPrompt("D - Remove a filter");
				UserIO.PrintUIPrompt("1 - Filter by percentile");
				UserIO.PrintUIPrompt("2 - Filter by faction");
				UserIO.PrintUIPrompt("3 - Filter by match age in hours");
				UserIO.PrintUIPrompt("Please select an operation.");

				char operation = UserIO.RunCharSelection('Q', 'S', 'D', '1', '2', '3');
				operation = char.ToLower(operation);

				if (operation == 'q')
				{
					return;
				}

				if (operation == 's')
				{
					string timestamp = DateTime.Now.ToString("ddMMyyyyHHmmss");
					string fileName = timestamp + filterHistory;
					SaveResultsToFile(mab, fileName);
				}

				if (operation == 'd')
				{
					string newFilters = string.Empty;
					filters = filterHistory.Split(',').ToList();

					if (filters.Count > 1)
					{
						for (int i = 1; i < filters.Count; i++)
						{
							UserIO.PrintUIPrompt(i.ToString() + " - " + filters[i]);
						}
						UserIO.PrintUIPrompt("Please select the filter you want to remove.");
						int selection = UserIO.RunIntegerSelection(1, filters.Count);

						for (int i = 0; i < filters.Count; i++)
						{
							if (i == selection)
							{
								continue;
							}

							if (filters[i] == string.Empty)
							{
								continue;
							}

							newFilters += ",";
							newFilters += filters[i];
						}
					}

					filterHistory = newFilters;
				}

				if (operation == '1')
				{
					UserIO.PrintUIPrompt("Please select the cutoff percentile. More options will be presented afterwards.");
					double percentile = UserIO.RunFloatingPointInput();

					UserIO.PrintUIPrompt("1 - get matches for the top {0}% of the playerbase", percentile);
					UserIO.PrintUIPrompt("2 - get matches for the bottom {0}% of the playerbase", (100.0 - percentile));
					UserIO.PrintUIPrompt("Please make your selection.");
					int topOrBottom = UserIO.RunIntegerSelection(1, 2);
					bool useTopPercentile = (topOrBottom == 1);

					string filterString = "," + inclusivePercentileFilterTag + "-";
					if (!useTopPercentile)
					{
						filterString = "," + exclusivePercentileFilterTag + "-";
					}

					filterHistory += filterString + percentile;
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

					filterHistory += "," + factionFilterTag + "-" + string.Join("", partsList);
				}

				if (operation == '3')
				{
					UserIO.PrintUIPrompt("Please select maximum allowed age for matches in hours: ");
					int hours = UserIO.RunIntegerSelection(0, 8760);

					filterHistory += "," + ageFilterTag + "-" + hours;
				}
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

		public static void RunModeOperations(DatabaseHandler db, int operatingMode, MatchTypeId gameMode)
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
				MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches(db).FilterByStartGameTime((int)relevantTimeCutoff, -1).FilterByMatchType(gameMode);
				RunInteractiveAnalysis(db, mab);
			}
		}

		private static void Main()
		{
			var culture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = culture;
			
			while (true)
			{
				MatchTypeId gameMode = RunGameModeSelection();

				DatabaseHandler db = new DatabaseHandler();
				db.Load(gameMode);

				int operatingMode = RunOperatingModeSelection();
				RunModeOperations(db, operatingMode, gameMode);
			}
		}
	}
}