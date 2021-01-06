using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			RelicApi.PlayerSummaries.GetBySteamId("76561198050674754");


			Console.WriteLine(PlayerIdentityTracker.GetNumLoggedPlayers());
			Console.ReadLine();
		}
	}
}