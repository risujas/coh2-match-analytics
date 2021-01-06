using System;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			RelicApi.Leaderboard.GetById(4, 1, 5);
			RelicApi.Leaderboard.GetById(5, 1, 5);

			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}