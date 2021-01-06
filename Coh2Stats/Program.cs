using System;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			RelicApi.PlayerSummaries.GetBySteamId("76561198050674754");
			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}