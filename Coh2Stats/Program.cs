using System;
using System.IO;

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
				DatabaseHandler.Load();
				DatabaseHandler.ProcessPlayers();
				DatabaseHandler.ProcessMatches();
			}
		}
	}
}