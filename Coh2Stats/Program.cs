using System;
using System.Diagnostics;
using System.Threading;

namespace Coh2Stats
{
	public class Program
	{
		public static MatchTypeId RunGameModeSelection()
		{
			UserIO.WriteSeparator();
			UserIO.WriteLine("1 - 1v1 automatch");
			UserIO.WriteLine("2 - 2v2 automatch");
			UserIO.WriteLine("3 - 3v3 automatch");
			UserIO.WriteLine("4 - 4v4 automatch");
			UserIO.WriteLine("Please select a game mode.");

			return (MatchTypeId)UserIO.RunIntegerSelection(1, 4);
		}

		public static int RunOperatingModeSelection()
		{
			UserIO.WriteSeparator();
			UserIO.WriteLine("1 - Match logging");
			UserIO.WriteLine("2 - Match logging (repeating)");
			UserIO.WriteLine("3 - Match analysis");
			UserIO.WriteLine("Please select an operating mode.");

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
					UserIO.WriteLine("Resuming operations in {0:0} seconds. You can press CTRL + C to exit this program, or ESCAPE to return to the start screen.", difference);
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

		public static void RunModeOperations(int operatingMode, MatchTypeId gameMode)
		{
			if (operatingMode == 1)
			{
				DatabaseHandler.ParseAndProcess(gameMode);
			}

			if (operatingMode == 2)
			{
				while (true)
				{
					DatabaseHandler.ParseAndProcess(gameMode);
					RunCooldownProcedure();
				}
			}

			if (operatingMode == 3)
			{
				MatchAnalytics.RunInteractiveAnalysis(gameMode);
			}
		}

		private static void Main()
		{
			while (true)
			{
				MatchTypeId gameMode = RunGameModeSelection();

				DatabaseHandler.Load(gameMode);

				int operatingMode = RunOperatingModeSelection();
				RunModeOperations(operatingMode, gameMode);
			}
		}
	}
}