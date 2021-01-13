using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coh2Stats
{
	class DatabaseBuilder
	{
		private const string playerList = "players.txt";

		public void Build(MatchTypeId gameMode)
		{
			if (!LoadPlayerList(playerList, 1))
			{
				BuildPlayerList(gameMode, 1, -1);
			}

			PlayerIdentityTracker.SortPlayersByHighestRank();

			BuildMatchList(20);
		}

		private bool LoadPlayerList(string filePath, int dataExpirationMinutes)
		{
			if (!File.Exists(filePath))
			{
				return false;
			}

			var lines = File.ReadAllLines(filePath);
			var dataUnixTime = int.Parse(lines[0]);

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt).AddMinutes(-dataExpirationMinutes);
			long cutoffUnixTime = dto.ToUnixTimeSeconds();

			if (dataUnixTime >= cutoffUnixTime)
			{
				return false;
			}

			var contents = File.ReadAllLines(filePath);
			for (int i = 1; i < contents.Length; i++)
			{
				var parts = contents[i].Split(' ');
				var quoteOpen = contents[i].IndexOf('"');

				PlayerIdentity player = new PlayerIdentity();
				player.ProfileId = int.Parse(parts[0]);
				player.Name = parts[1];
				player.PersonalStatGroupId = int.Parse(parts[2]);
				player.Xp = int.Parse(parts[3]);
				player.Level = int.Parse(parts[4]);
				player.Country = parts[5];
				player.LeaderboardRegionId = int.Parse(parts[6]);

				string alias = contents[i].Substring(quoteOpen + 1);
				alias = alias.Substring(0, alias.Length - 1);
				player.Alias = alias;

				player.PrintPlayer();
				PlayerIdentityTracker.LogPlayer(player);
			}

			return true;
		}

		private void WritePlayerList(string filePath)
		{
			File.Delete(filePath);

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt);
			long dataUnixTime = dto.ToUnixTimeSeconds();

			List<string> contents = new List<string>();
			contents.Add(dataUnixTime.ToString());

			var players = PlayerIdentityTracker.PlayerIdentities;
			foreach (var p in players)
			{
				string line = "";
				line += p.ProfileId + " " + p.Name + " " + p.PersonalStatGroupId + " " + p.Xp + " " + p.Level + " " + p.Country + " " + p.LeaderboardRegionId + " \"" + p.Alias + "\"";
				contents.Add(line);
			}

			File.WriteAllLines(filePath, contents);
		}

		private void BuildPlayerList(MatchTypeId matchTypeId, int startingRank = 1, int maxRank = -1)
		{
			for (int leaderboardIndex = 0; leaderboardIndex < 100; leaderboardIndex++)
			{
				if (LeaderboardCompatibility.LeaderboardBelongsWithMatchType((LeaderboardId)leaderboardIndex, matchTypeId) == false)
				{
					continue;
				}

				var probeResponse = JsonLeaderboard.GetById(leaderboardIndex, 1, 1);
				int leaderboardMaxRank = probeResponse.RankTotal;
				int batchStartingIndex = startingRank;

				if (maxRank != -1)
				{
					leaderboardMaxRank = maxRank;
				}

				while (batchStartingIndex < leaderboardMaxRank)
				{
					int difference = leaderboardMaxRank - batchStartingIndex;
					int batchSize = 200;

					if (difference < 200)
					{
						batchSize = difference + 1;
					}

					JsonLeaderboard.GetById(leaderboardIndex, batchStartingIndex, batchSize);
					Console.WriteLine("Parsing leaderboard #{0}: {1} - {2} ({3} total)", leaderboardIndex, batchStartingIndex, batchStartingIndex + batchSize - 1, PlayerIdentityTracker.GetNumLoggedPlayers());
					batchStartingIndex += batchSize;
				}
			}

			var players = PlayerIdentityTracker.PlayerIdentities.ToList();
			int magic = 200;
			while (players.Count > 0)
			{
				Console.WriteLine(players.Count);

				if (players.Count >= magic)
				{
					var range = players.GetRange(0, magic);
					players.RemoveRange(0, magic);

					List<int> profileIds = new List<int>();
					foreach (var p in range)
					{
						profileIds.Add(p.ProfileId);
					}

					JsonPersonalStat.GetByProfileId(profileIds);
				}

				else
				{
					magic = players.Count;
				}
			}

			WritePlayerList(playerList);
		}

		private void BuildMatchList(int maxPlayersProcessed = -1)
		{
			int max = PlayerIdentityTracker.GetNumLoggedPlayers();
			if (maxPlayersProcessed != -1)
			{
				max = maxPlayersProcessed;
			}

			for (int i = 0; i < max; i++)
			{
				var p = PlayerIdentityTracker.PlayerIdentities[i];
				JsonRecentMatchHistory.GetBySteamId(p.Name);

				Console.WriteLine("Fetched recent match history for {0} ({1})", p.Name, p.Alias);
			}
		}
	}
}
