using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(GameMode.OneVsOne);

			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}