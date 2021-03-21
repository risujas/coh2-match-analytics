using System;
using System.IO;
using System.Threading;

namespace Coh2Stats
{
	class Program
	{
		public static string ApplicationDataFolder
		{
			get;
		} = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\coh2stats";

		private static void Main()
		{
			Directory.CreateDirectory(ApplicationDataFolder);
			Directory.CreateDirectory(DatabaseHandler.DatabaseFolder);
			Directory.CreateDirectory(UserIO.LogFolder);

			UserIO.PrintStartingInfo();

			while (true)
			{
				try
				{
					DatabaseHandler.Load();
					DatabaseHandler.ProcessPlayers();
					DatabaseHandler.ProcessMatches();
					DatabaseHandler.Unload();

					UserIO.WriteLine("Processing finished. The program will continue operations in 10 minutes from now.");
					Thread.Sleep(1000 * 60 * 10);
				}
				catch (Exception e)
				{
					UserIO.WriteExceptions(e);
				}
			}
		}
	}
}