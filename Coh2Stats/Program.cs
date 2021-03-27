using System;
using System.Diagnostics;
using System.Threading;

namespace Coh2Stats
{
	public class Program
	{
		public static readonly long RelevantTimeCutoffSeconds = 0;

		static Program()
		{
			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt).AddDays(-30);
			RelevantTimeCutoffSeconds = dto.ToUnixTimeSeconds();
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

		public static bool RunCooldownProcedure()
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
						return false;
					}
				}
			}

			return true;
		}

		public static void RunModeOperations(int operatingMode)
		{
			if (operatingMode == 1)
			{
				DatabaseHandler.ParseAndProcess();
			}

			if (operatingMode == 2)
			{
				while (true)
				{
					DatabaseHandler.ParseAndProcess();
					
					if (RunCooldownProcedure() == false)
					{
						return;
					}
				}
			}

			if (operatingMode == 3)
			{
				MatchAnalytics.RunInteractiveAnalysis();
			}
		}

		private static void Main(string[] args)
		{
			UserIO.WriteLine("Relevant data cutoff set to " + RelevantTimeCutoffSeconds);

			if (args.Length > 0)
			{
				if (args[0] == "-auto")
				{
					UserIO.PrintStartingInfo();
					DatabaseHandler.Load();
					RunModeOperations(1);
				}
			}

			else
			{
				while (true)
				{
					UserIO.PrintStartingInfo();

					DatabaseHandler.Load();

					int operatingMode = RunOperatingModeSelection();
					RunModeOperations(operatingMode);
				}
			}
		}
	}
}