using System;
using System.IO;
using System.Threading;
using System.Globalization;
using Microsoft.VisualBasic.Devices;

namespace Coh2Stats
{
	static class UserIO
	{
		public static string LogFolder
		{
			get;
		} = Program.ApplicationDataFolder + "\\logs";

		private static readonly string logFile;

		static UserIO()
		{
			var culture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = culture;

			DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
			logFile = LogFolder + "\\" + dto.ToUnixTimeSeconds().ToString() + "_log.txt";
		}

		public static void PrintStartingInfo()
		{
			DateTime dt = DateTime.Now;
			WriteLine(dt.ToShortDateString() + " " + dt.ToShortTimeString());
#if DEBUG
			WriteLine("DEBUG BUILD - NOT FOR RELEASE");
#endif

			var computerInfo = new ComputerInfo();
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
