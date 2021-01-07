using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(RelicApi.GameModeId.OneVsOne);

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches();

			var games = mab.FilterByRace(RelicApi.RaceFlag.AEF).GetOrderedMapPlayCount();
			Console.WriteLine("\nUSF TOTAL");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.AEF).FilterByResult(true, RelicApi.FactionId.Allies).GetOrderedMapPlayCount();
			Console.WriteLine("\nUSF WINS");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.AEF).FilterByResult(false, RelicApi.FactionId.Allies).GetOrderedMapPlayCount();
			Console.WriteLine("\nUSF LOSSES");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			Console.ReadLine();
		}
	}
}