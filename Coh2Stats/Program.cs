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
			list.Add("/steam/76561198088254955");
			RelicApi.JsonRecentMatchHistory.GetBySteamId(list);
			Console.WriteLine(MatchHistoryTracker.GetNumLoggedMatches());

			Console.ReadLine();
		}
	}
}