using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class PlayerIdentity
	{
		[JsonProperty("profile_id")] public int ProfileId { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("alias")] public string Alias { get; set; }
		[JsonProperty("personal_statgroup_id")] public int PersonalStatGroupId { get; set; }
		[JsonProperty("xp")] public int Xp { get; set; }
		[JsonProperty("level")] public int Level { get; set; }
		[JsonProperty("leaderboardregion_id")] public int LeaderboardRegionId { get; set; }
		[JsonProperty("country")] public string Country { get; set; }

		public int GetHighestRank()
		{
			Console.WriteLine(Alias + " " + Name); // TODO REMOVE

			return LeaderboardStatTracker.GetHighestStatByStatGroup(PersonalStatGroupId).Rank;
		}

		public void PrintPlayer()
		{
			Console.WriteLine(ProfileId + " " + Name + " " + PersonalStatGroupId + " " + Xp + " " + Level + " " + Country + " " + LeaderboardRegionId + " " + Alias);
		}
	}

	class PlayerIdentityTracker
	{
		private static List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();
		public static ReadOnlyCollection<PlayerIdentity> PlayerIdentities
		{
			get { return playerIdentities.AsReadOnly(); }
		}

		public static bool LoadPlayerList(string filePath, int dataExpirationMinutes)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine("No existing player list found");
				return false;
			}

			var lines = File.ReadAllLines(filePath);
			int dataUnixTime = int.Parse(lines[0]);
			int dataExpirationTime = dataUnixTime + (dataExpirationMinutes * 60);

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset currentDto = new DateTimeOffset(dt);
			long currentUnixTime = currentDto.ToUnixTimeSeconds();

			if (currentUnixTime >= dataExpirationTime)
			{
				Console.WriteLine("Existing player list is outdated");
				return false;
			}
			else
			{
				Console.WriteLine("Existing player list is valid");
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

				LogPlayer(player);
			}

			return true;
		}

		public static void WritePlayerList(string filePath)
		{
			File.Delete(filePath);

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt);
			long dataUnixTime = dto.ToUnixTimeSeconds();

			List<string> contents = new List<string>();
			contents.Add(dataUnixTime.ToString());

			foreach (var p in playerIdentities)
			{
				string line = "";
				line += p.ProfileId + " " + p.Name + " " + p.PersonalStatGroupId + " " + p.Xp + " " + p.Level + " " + p.Country + " " + p.LeaderboardRegionId + " \"" + p.Alias + "\"";
				contents.Add(line);
			}

			File.WriteAllLines(filePath, contents);
		}

		public static PlayerIdentity GetPlayerByProfileId(int profileId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.ProfileId == profileId)
				{
					return pi;
				}
			}

			return null;
		}

		public static PlayerIdentity GetPlayerBySteamId(string steamId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.Name == steamId)
				{
					return pi;
				}
			}

			return null;
		}

		public static PlayerIdentity GetPlayerByPersonalStatGroupId(int personalStatGroupId)
		{
			foreach (var pi in playerIdentities)
			{
				if (pi.PersonalStatGroupId == personalStatGroupId)
				{
					return pi;
				}
			}

			return null;
		}

		public static void LogPlayer(PlayerIdentity playerIdentity)
		{
			if (GetPlayerByProfileId(playerIdentity.ProfileId) == null)
			{
				playerIdentities.Add(playerIdentity);
			}
		}

		public static int GetNumLoggedPlayers()
		{
			return playerIdentities.Count;
		}

		public static string GetPlayersForWebRequest()
		{
			List<string> names = new List<string>();
			foreach(var p in playerIdentities)
			{
				names.Add(p.Name);
			}
			string idString = "\"" + string.Join("\",\"", names) + "\"";
			return idString;
		}

		public static void PrintLoggedPlayers()
		{
			foreach (var p in playerIdentities)
			{
				Console.WriteLine(p.Name + " " + p.ProfileId + " " + p.Alias + " " + p.GetHighestRank());
			}
		}

		public static void SortPlayersByHighestRank()
		{
			Console.WriteLine("Sorting player list by best rank");

			playerIdentities = playerIdentities.OrderBy(p => p.GetHighestRank()).ToList();
		}
	}
}
