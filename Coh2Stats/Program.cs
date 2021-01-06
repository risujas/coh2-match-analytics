using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			List<string> list = new List<string>();
			
			list.Add("/steam/76561198050674754");
			RelicApi.JsonPlayerSummaries.GetBySteamId(list);
			Console.WriteLine(PlayerIdentityTracker.GetAllPlayersAsString());
			Console.ReadLine();
		}
	}
}