﻿using System;
using System.Collections.Generic;
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
		[JsonProperty("country")] public string country { get; set; }

		public PlayerIdentity() {}

		public PlayerIdentity (PlayerIdentity relicObject)
		{
			ProfileId = relicObject.ProfileId;
			Name = relicObject.Name;
			Alias = relicObject.Alias;
			PersonalStatGroupId = relicObject.PersonalStatGroupId;
			Xp = relicObject.Xp;
			Level = relicObject.Level;
			LeaderboardRegionId = relicObject.LeaderboardRegionId;
			country = relicObject.country;
		}
	}

	class PlayerIdentityTracker
	{
		private static List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();

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
				Console.WriteLine(p.Name + " " + p.ProfileId + " " + p.Alias);
			}
		}
	}
}
