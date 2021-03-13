using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Coh2Stats
{
	internal class Program
	{
		public static MatchTypeId RunGameModeSelection()
		{
			UserIO.ClearConsoleWithCountdown(3);

			UserIO.PrintUIPrompt("1 - 1v1 automatch");
			UserIO.PrintUIPrompt("2 - 2v2 automatch");
			UserIO.PrintUIPrompt("3 - 3v3 automatch");
			UserIO.PrintUIPrompt("4 - 4v4 automatch");
			UserIO.PrintUIPrompt("Please select a game mode.");

			return (MatchTypeId)UserIO.RunIntegerSelection(1, 4);
		}

		public static int RunOperatingModeSelection()
		{
			UserIO.ClearConsoleWithCountdown(3);

			UserIO.PrintUIPrompt("1 - Match logging");
			UserIO.PrintUIPrompt("2 - Match logging (repeating)");
			UserIO.PrintUIPrompt("3 - Match analysis");
			UserIO.PrintUIPrompt("Please select an operating mode.");

			return UserIO.RunIntegerSelection(1, 4);
		}

		public static void RunCooldownProcedure()
		{
			Stopwatch sw = Stopwatch.StartNew();
			double sessionInterval = 1200;
			int notificationInterval = 60;

			while (sw.Elapsed.TotalSeconds < sessionInterval)
			{
				double difference = sessionInterval - sw.Elapsed.TotalSeconds;
				int intDiff = (int)difference;

				if (intDiff % notificationInterval == 0)
				{
					UserIO.PrintUIPrompt("Resuming operations in {0:0} seconds. You can press CTRL + C to exit this program, or ESCAPE to return to the start screen.", difference);
					Thread.Sleep(1000);
				}

				if (Console.KeyAvailable)
				{
					var cki = Console.ReadKey();
					if (cki.Key == ConsoleKey.Escape)
					{
						return;
					}
				}
			}
		}

		public static void RunModeOperations(DatabaseHandler db, int operatingMode, MatchTypeId gameMode)
		{
			if (operatingMode == 1)
			{
				db.ParseAndProcess(gameMode);
			}

			if (operatingMode == 2)
			{
				while (true)
				{
					db.ParseAndProcess(gameMode);
					RunCooldownProcedure();
				}
			}

			if (operatingMode == 3)
			{
				MatchAnalytics.RunInteractiveAnalysis(db, gameMode);
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