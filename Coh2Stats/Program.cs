using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			List<string> list = new List<string>();
			list.Add("76561198002068856");
			list.Add("76561198050674754");
			RelicApi.PersonalStat.GetBySteamId(list);
			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}