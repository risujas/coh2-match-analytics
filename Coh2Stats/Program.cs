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

			var temp = MapAnalysis.GetMapPopularityDictionary(RaceId.British);
			Console.Clear();
			foreach (var t in temp)
			{
				Console.WriteLine(t.Value + " " + t.Key);
			}

			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.ReadLine();
		}
	}
}