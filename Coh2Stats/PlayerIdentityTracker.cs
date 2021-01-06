﻿using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class PlayerIdentity
	{
		public string Nickname;
		public string SteamId;
		public int ProfileId;
		public int PersonalStatGroupId;
		public string Country;
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
				if (pi.SteamId == steamId)
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
			foreach (var pi in playerIdentities)
			{
				Console.WriteLine(pi.SteamId + " " + pi.ProfileId + " " + pi.Nickname);
			}
		}
	}
}
