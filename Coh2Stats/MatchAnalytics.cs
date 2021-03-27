using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coh2Stats
{
	static class MatchAnalytics
	{

		private const string factionFilterTag = "factions";
		private const string percentFilterTag = "ranks%";
		private const string ageFilterTag = "hours";

		public static void RunInteractiveAnalysis(string filterHistory = "")
		{
			MatchAnalyticsBundle mab = null;
			bool skipFiltering = false;
			List<string> filters;

			while (true)
			{
				if (!skipFiltering)
				{
					mab = MatchAnalyticsBundle.GetAllLoggedMatches().FilterByStartGameTime((int)Program.MatchRelevancyCutoff, -1);

					UserIO.WriteLine("Filter history: " + filterHistory);

					filters = filterHistory.Split(',').ToList();
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
								RaceFlag flags = LeaderboardCompatibility.GenerateRaceFlag(second.Contains("w"), second.Contains("s"), second.Contains("u"), second.Contains("o"), second.Contains("b"));
								mab = mab.FilterByAllowedRaces(flags);
							}

							if (first == percentFilterTag)
							{
								var moreParts = second.Split('_');
								var low = moreParts[0];
								var high = moreParts[1];

								mab = mab.FilterByPercentile(double.Parse(low), double.Parse(high), true);
							}

							if (first == ageFilterTag)
							{
								mab = mab.FilterByMaxAgeInHours(int.Parse(second));
							}
						}
					}
				}
				else
				{
					skipFiltering = false;
				}

				AnalyzeWinRatesByRace(mab, RaceFlag.German);
				AnalyzeWinRatesByRace(mab, RaceFlag.WGerman);
				AnalyzeWinRatesByRace(mab, RaceFlag.Soviet);
				AnalyzeWinRatesByRace(mab, RaceFlag.AEF);
				AnalyzeWinRatesByRace(mab, RaceFlag.British);

				UserIO.WriteLine("");
				var playCounts = mab.GetOrderedMapPlayCount();
				foreach (var p in playCounts)
				{
					UserIO.WriteLine(p.Value + " " + p.Key);
				}

				UserIO.WriteSeparator();
				UserIO.WriteLine("Q - Finish running the interactive analysis");
				UserIO.WriteLine("S - Export the current results");
				UserIO.WriteLine("D - Remove a filter");
				UserIO.WriteLine("1 - Filter by percentile brackets");
				UserIO.WriteLine("2 - Filter by faction");
				UserIO.WriteLine("3 - Filter by match age in hours");
				UserIO.WriteLine("Please select an operation.");

				char operation = UserIO.RunCharSelection('Q', 'S', 'D', '1', '2', '3');
				operation = char.ToLower(operation);

				if (operation == 'q')
				{
					return;
				}

				if (operation == 's')
				{
					string timestampSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
					string fileName = timestampSeconds + filterHistory;
					SaveResultsToFile(mab, fileName);
					skipFiltering = true;
				}

				if (operation == 'd')
				{
					string newFilters = string.Empty;
					filters = filterHistory.Split(',').ToList();

					if (filters.Count > 1)
					{
						for (int i = 1; i < filters.Count; i++)
						{
							UserIO.WriteLine(i.ToString() + " - " + filters[i]);
						}
						UserIO.WriteLine("Please select the filter you want to remove.");
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
					UserIO.WriteLine("Please select the low percentile cutoff.");
					double lowCutoff = UserIO.RunFloatingPointInput();

					UserIO.WriteLine("Please select the high percentile cutoff.");
					double highCutoff = UserIO.RunFloatingPointInput();

					filterHistory += "," + percentFilterTag + "-" + lowCutoff.ToString() + "_" + highCutoff.ToString();
				}

				if (operation == '2')
				{
					UserIO.WriteLine("W - Wehrmacht Ostheer");
					UserIO.WriteLine("S - Soviet Union");
					UserIO.WriteLine("U - United States Forces");
					UserIO.WriteLine("O - Oberkommando West");
					UserIO.WriteLine("B - British Forces");
					UserIO.WriteLine("Input the factions you want to be included in the results. You can input multiple factions by separating the characters with commas or spaces.");

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
					UserIO.WriteLine("Please select maximum allowed age for matches in hours: ");
					int hours = UserIO.RunIntegerSelection(0, 8760);

					filterHistory += "," + ageFilterTag + "-" + hours;
				}
			}
		}

		private static void AnalyzeWinRatesByRace(MatchAnalyticsBundle mab, RaceFlag raceFlag, string destination = "")
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
				UserIO.WriteLine("");
				UserIO.WriteLine("{0} win rates:", raceFlag.ToString());
				UserIO.WriteLine("{0:0.000}     {1} / {2,5}", totalWinRate, wonGames.Matches.Count, allGames.Matches.Count);
				UserIO.WriteLine("----------------------------------------------------------------");

				foreach (var m in mapsByWinRate)
				{
					int winCount = 0;
					if (mapsByWinCount.ContainsKey(m.Key))
					{
						winCount = mapsByWinCount[m.Key];
					}

					UserIO.WriteLine("{0:0.000}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
				}
			}

			else
			{
				using (StreamWriter file = File.AppendText(destination))
				{
					file.WriteLine("{0} win rates:", raceFlag.ToString());
					file.WriteLine("{0:0.000}     {1} / {2,5}", totalWinRate, wonGames.Matches.Count, allGames.Matches.Count);
					file.WriteLine("----------------------------------------------------------------");

					foreach (var m in mapsByWinRate)
					{
						int winCount = 0;
						if (mapsByWinCount.ContainsKey(m.Key))
						{
							winCount = mapsByWinCount[m.Key];
						}

						file.WriteLine("{0:0.000}     {2} / {3,5} {1,40}", m.Value, m.Key, winCount, mapsByPlayCount[m.Key]);
					}

					file.WriteLine();
				}
			}
		}

		private static void SaveResultsToFile(MatchAnalyticsBundle mab, string fileName)
		{
			string representationFile = Program.ExportsFolder + "\\" + fileName + ".txt";
			string matchDataFile = Program.ExportsFolder + "\\" + fileName + ".json";

			File.Delete(representationFile);
			File.Delete(matchDataFile);

			AnalyzeWinRatesByRace(mab, RaceFlag.German, representationFile);
			AnalyzeWinRatesByRace(mab, RaceFlag.Soviet, representationFile);
			AnalyzeWinRatesByRace(mab, RaceFlag.AEF, representationFile);
			AnalyzeWinRatesByRace(mab, RaceFlag.WGerman, representationFile);
			AnalyzeWinRatesByRace(mab, RaceFlag.British, representationFile);

			var matchData = JsonConvert.SerializeObject(mab.Matches, Formatting.Indented);
			File.WriteAllText(matchDataFile, matchData);

		}
	}
}
