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
			RelicApi.Leaderboard.GetById(4);

			PlayerIdentityTracker.PrintLoggedPlayers();

			Console.ReadLine();
		}
	}
}