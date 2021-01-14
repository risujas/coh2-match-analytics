using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

// TODO load all players from all gamemodes, add filters for getting the subset of players that you want

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
			var stat = LeaderboardStatTracker.GetHighestStatByStatGroup(PersonalStatGroupId);
			if (stat == null)
			{
				return int.MaxValue;
			}

			return stat.Rank;
		}

		public void PrintPlayer()
		{
			Console.WriteLine(ProfileId + " " + Name + " " + PersonalStatGroupId + " " + Xp + " " + Level + " " + Country + " " + LeaderboardRegionId + " " + Alias);
		}
	}

	public class PlayerDataSet
	{
		public List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();
		public long unixTimeStamp = 0;
	}

	class PlayerIdentityTracker
	{
		private const string playerListFilePath = "players.txt";

		private static List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();
		public static ReadOnlyCollection<PlayerIdentity> PlayerIdentities
		{
			get { return playerIdentities.AsReadOnly(); }
		}

		public static bool LoadPlayerList(int dataExpirationMinutes)
		{
			if (!File.Exists(playerListFilePath))
			{
				Console.WriteLine("No existing player list found");
				return false;
			}

			string text = File.ReadAllText(playerListFilePath);
			var json = JsonConvert.DeserializeObject<PlayerDataSet>(text);

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset currentDto = new DateTimeOffset(dt);
			long currentUnixTime = currentDto.ToUnixTimeSeconds();
			long dataExpirationTime = json.unixTimeStamp + (dataExpirationMinutes * 60);

			if (currentUnixTime >= dataExpirationTime)
			{
				Console.WriteLine("Existing player list is outdated");
				return false;
			}

			Console.WriteLine("Existing player list is valid");
			playerIdentities = json.playerIdentities;

			return true;
		}

		public static void WritePlayerList()
		{
			PlayerDataSet pids = new PlayerDataSet();
			pids.playerIdentities = playerIdentities;

			DateTime dt = DateTime.UtcNow;
			DateTimeOffset dto = new DateTimeOffset(dt);
			pids.unixTimeStamp = dto.ToUnixTimeSeconds();

			var text = JsonConvert.SerializeObject(pids, Formatting.Indented);
			File.WriteAllText(playerListFilePath, text);
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
			foreach (var p in playerIdentities)
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
