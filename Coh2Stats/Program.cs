using System;

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
			UserIO.WriteLine("2 - Match analysis");
			UserIO.WriteLine("Please select an operating mode.");

			return UserIO.RunIntegerSelection(1, 2);
		}

		public static void RunModeOperations(int operatingMode)
		{
			if (operatingMode == 1)
			{
				DatabaseHandler.ParseAndProcess();
			}

			if (operatingMode == 2)
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