using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	public class LoggedPlayer
	{
		public int slot = 0;
		public int team = 0;
		public string profileId = "";
		public string nickName = "";
		public string race = "";
		public int rank = 0;
	}

	class LogParser
	{
		public static string LogFile { get; set; }

		static LogParser()
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			path += "\\my games\\company of heroes 2\\warnings.log";
			LogFile = path;
		}
		
		public static List<LoggedPlayer> GetPlayerList()
		{
			List<LoggedPlayer> players = new List<LoggedPlayer>();

			string clonedLogFile = AppDomain.CurrentDomain.BaseDirectory + "\\warnings.log";
			File.Delete(clonedLogFile);
			File.Copy(LogFile, clonedLogFile);

			StreamReader reader = new StreamReader(clonedLogFile);

			string line = "";
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("GAME -- Scenario"))
				{
					players.Clear();
				}

				if (line.Contains("GAME -- Human Player"))
				{
					string playerInfo = line.Substring(36);
					var parts = playerInfo.Split(' ');

					LoggedPlayer player = new LoggedPlayer();
					player.slot = int.Parse(parts[0]);
					player.race = parts[parts.Length - 1];
					player.team = int.Parse(parts[parts.Length - 2]);
					player.profileId = parts[parts.Length - 3];
					player.nickName = playerInfo.Substring(2, playerInfo.IndexOf(player.profileId) - 2);
					players.Add(player);
				}
			}

			reader.Close();
			players = players.OrderBy(p => p.team).ToList();

			return players;
		}
	}
}
