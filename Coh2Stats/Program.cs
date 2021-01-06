using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			MatchHistoryTracker.BuildDatabase();
			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}