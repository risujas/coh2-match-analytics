using System;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			RelicApi.PersonalStat.GetBySteamId("76561198050674754");


			Console.WriteLine(PlayerIdentityTracker.GetNumLoggedPlayers());
			Console.ReadLine();
		}
	}
}