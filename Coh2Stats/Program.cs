using System;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			RelicApi.RecentMatchHistory.GetBySteamId("76561198404414770");

			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}