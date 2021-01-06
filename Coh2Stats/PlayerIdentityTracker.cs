using System.Collections.Generic;

namespace Coh2Stats
{
	class PlayerIdentity
	{
		public string Alias;
		public string SteamId;
		public string ProfileId;

		public PlayerIdentity()
		{
		}

		public PlayerIdentity(string alias, string steamId, string profileId)
		{
			Alias = alias;
			SteamId = steamId;
			ProfileId = profileId;
		}
	}

	class PlayerIdentityTracker
	{
		private static List<PlayerIdentity> playerIdentities = new List<PlayerIdentity>();

		public static PlayerIdentity GetPlayerByProfileId(string profileId)
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
	}
}
