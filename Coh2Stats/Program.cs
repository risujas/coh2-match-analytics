using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			List<string> list = new List<string>();
			
			list.Add("76561198050674754");
			list.Add("76561198088254955");
			RelicApi.RecentMatchHistory.GetBySteamId(list);
			Console.WriteLine(MatchHistoryTracker.GetNumLoggedMatches());

			Console.ReadLine();
		}
	}
}