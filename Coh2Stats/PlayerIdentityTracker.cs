using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Profile
	{
		public string Nickname;
		public string SteamId;
		public string ProfileId;
		public string PersonalStatGroupId;
		public string Country;
	}

	class PlayerIdentityTracker
	{
		private static List<Profile> playerIdentities = new List<Profile>();

		public static Profile GetPlayerByProfileId(string profileId)
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

		public static Profile GetPlayerBySteamId(string steamId)
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

		public static void LogPlayer(Profile playerIdentity)
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
