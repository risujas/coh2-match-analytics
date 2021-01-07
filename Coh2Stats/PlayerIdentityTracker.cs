using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
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

		int[] leaderboardRanks = new int[55];

		public PlayerIdentity() 
		{
			InitRanks();
		}

		public PlayerIdentity (PlayerIdentity relicObject)
		{
			ProfileId = relicObject.ProfileId;
			Name = relicObject.Name;
			Alias = relicObject.Alias;
			PersonalStatGroupId = relicObject.PersonalStatGroupId;
			Xp = relicObject.Xp;
			Level = relicObject.Level;
			LeaderboardRegionId = relicObject.LeaderboardRegionId;
			Country = relicObject.Country;
		}

		private void InitRanks()
		{
			for (int i = 0; i < leaderboardRanks.Length; i++)
			{
				leaderboardRanks[i] = int.MaxValue;
			}
		}

		public int BestRank()
		{
			int best = int.MaxValue;
			int indexOfBest = 0;

			for (int i = 0; i < leaderboardRanks.Length; i++)
			{
				if (leaderboardRanks[i] < best)
				{
					best = leaderboardRanks[i];
					indexOfBest = i;
				}
			}

			return indexOfBest;
		}
	}

	class PlayerIdentityTracker
	{
		private static List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();
		public static ReadOnlyCollection<PlayerIdentity> PlayerIdentities
		{
			get { return playerIdentities.AsReadOnly(); }
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

		public static void PrintLoggedPlayers()
		{
			foreach (var p in playerIdentities)
			{
				Console.WriteLine(p.Name + " " + p.ProfileId + " " + p.Alias + " " + p.BestRank);
			}
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

		public static void SortPlayersByHighestRank()
		{
			playerIdentities = playerIdentities.OrderBy(p => p.BestRank).ToList();
		}
	}
}
