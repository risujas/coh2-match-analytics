using System;
using System.IO;
using System.Threading;
using System.Globalization;
using Microsoft.VisualBasic.Devices;

namespace Coh2Stats
{
	public static class UserIO
	{
		private static readonly string logFile;
		private const string logFolder = "\\logs";

		static UserIO()
		{
			var culture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = culture;

			string fullLogDirectory = DatabaseHandler.ApplicationDataFolder + logFolder;
			Directory.CreateDirectory(fullLogDirectory);

			DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
			logFile = fullLogDirectory + "\\" + dto.ToUnixTimeSeconds().ToString() + "_log.txt";

			if (Properties.Settings1.Default.DeleteOldLogs)
			{
				string[] files = Directory.GetFiles(DatabaseHandler.ApplicationDataFolder, "*_log.txt");

				foreach (var f in files)
				{
					File.Delete(f);
				}
			}

			DateTime dt = DateTime.Now;
			WriteLine(dt.ToShortDateString() + " " + dt.ToShortTimeString());
#if DEBUG
			WriteLine("DEBUG BUILD - NOT FOR RELEASE");
#endif

			var computerInfo =  new ComputerInfo();
			WriteLine(computerInfo.OSFullName + " - " + computerInfo.OSPlatform + " - " + computerInfo.OSVersion);
			WriteLine("Total physical memory: " + computerInfo.TotalPhysicalMemory / 1000 / 1000 + " MB");
			WriteLine("Available: " + computerInfo.AvailablePhysicalMemory / 1000 / 1000 + " MB");
		}

		public static void WriteLine(string text, params object[] args)
		{
			if (args.Length > 0)
			{
				text = string.Format(text, args);
			}

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] " + text;

			Console.WriteLine(message);

			using (StreamWriter file = File.AppendText(logFile))
			{
				file.WriteLine(message);
			}
		}

		public static void WriteSeparator()
		{
			WriteLine("");
			WriteLine("****************************************************************");
		}
		
		public static void WriteExceptions(Exception e)
		{
			if (e.InnerException == null)
			{
				WriteLine("An error occurred: " + e.Message);
			}

			else
			{
				WriteExceptions(e.InnerException);
			}
		}

		public static int RunIntegerSelection(int inclusiveMin, int inclusiveMax)
		{
			int selection = 0;

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] << ";

			Console.ForegroundColor = ConsoleColor.Yellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = int.TryParse(input, out int result);
				if (!goodParse)
				{
					continue;
				}

				if (result < inclusiveMin || result > inclusiveMax)
				{
					continue;
				}

				selection = result;
				good = true;
			}

			Console.ResetColor();

			WriteLine("User input: " + selection);
			Console.Clear();

			return selection;
		}

		public static char RunCharSelection(params char[] options)
		{
			char selection = ' ';

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] << ";

			Console.ForegroundColor = ConsoleColor.Yellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = char.TryParse(input, out char result);
				if (!goodParse)
				{
					continue;
				}

				result = char.ToLower(result);

				bool matchFound = false;
				for (int i = 0; i < options.Length; i++)
				{
					if (result == char.ToLower(options[i]))
					{
						matchFound = true;
						break;
					}
				}
				if (!matchFound)
				{
					continue;
				}

				selection = result;
				good = true;
			}

			Console.ResetColor();

			WriteLine("User input: " + selection);
			Console.Clear();

			return selection;
		}

		public static double RunFloatingPointInput()
		{
			double floatInput = 0;

			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] << ";

			Console.ForegroundColor = ConsoleColor.Yellow;

			bool good = false;
			while (!good)
			{
				Console.Write(message);
				string input = Console.ReadLine();

				bool goodParse = double.TryParse(input, out double result);
				if (!goodParse)
				{
					continue;
				}

				floatInput = result;
				good = true;
			}

			Console.ResetColor();

			WriteLine("User input: " + floatInput);
			Console.Clear();

			return floatInput;
		}

		public static string RunStringInput()
		{
			DateTime dt = DateTime.Now;
			string time = dt.ToLongTimeString();
			string message = "[" + time + "] << ";

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(message);
			string input = Console.ReadLine();
			Console.ResetColor();

			WriteLine("User input: " + input);
			Console.Clear();

			return input;
		}

		public static void AllowPause()
		{
			if (Console.KeyAvailable)
			{
				var cki = Console.ReadKey(true);
				if (cki.Key == ConsoleKey.P)
				{
					WriteLine("Paused. Press P again to unpause.");

					bool paused = true;
					while (paused)
					{
						if (Console.KeyAvailable)
						{
							cki = Console.ReadKey(true);
							if (cki.Key == ConsoleKey.P)
							{
								paused = false;
							}
						}

						Thread.Sleep(1000);
					}
				}
			}
		}
	}
}
