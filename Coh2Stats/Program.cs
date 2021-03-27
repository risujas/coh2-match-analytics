using System;
using System.IO;

namespace Coh2Stats
{
	public class Program
	{
		public static string ApplicationDataFolder
		{
			get;
		} = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";

		public static string DatabaseFolder
		{
			get;
		} = ApplicationDataFolder + "\\database";

		public static string LogFolder
		{
			get;
		} = ApplicationDataFolder + "\\logs";

		public static string ExportsFolder
		{
			get;
		} = ApplicationDataFolder + "\\exports";

		public static readonly long RelevantTimeCutoffSeconds = 0;

		static Program()
		{
			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt).AddDays(-30);
			RelevantTimeCutoffSeconds = dto.ToUnixTimeSeconds();
		}
		private static int RunOperatingModeSelection()
		{
			UserIO.WriteSeparator();
			UserIO.WriteLine("1 - Match logging");
			UserIO.WriteLine("2 - Match analysis");
			UserIO.WriteLine("Please select an operating mode.");

			return UserIO.RunIntegerSelection(1, 2);
		}

		private static void RunModeOperations(int operatingMode)
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

		private static void CreateFolders()
		{
			Directory.CreateDirectory(ApplicationDataFolder);
			Directory.CreateDirectory(DatabaseFolder);
			Directory.CreateDirectory(LogFolder);
			Directory.CreateDirectory(ExportsFolder);
		}

		private static void Main(string[] args)
		{
			CreateFolders();	

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