using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class LogParser
	{
		public class Player
		{
			public int slot = 0;
			public int team = 0;
			public string profileId = "";
			public string nickName = "";
			public string race = "";
		}

		public static List<Player> GetPlayerList(string logFilePath)
		{
			List<Player> players = new List<Player>();
			StreamReader reader = new StreamReader(logFilePath);

			string line = "";
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("GAME -- Human Player"))
				{
					string playerInfo = line.Substring(36);
					var parts = playerInfo.Split(' ');

					Player player = new Player();
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
