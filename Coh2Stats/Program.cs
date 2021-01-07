using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(RelicApi.GameModeId.FourVsFour);

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches();
			var games = mab.FilterByRace(RelicApi.RaceFlag.British).FilterByMaxAgeInHours(24 * 7);
			var dict = games.GetOrderedMapPlayCount();

			foreach(var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.ReadLine();
		}
	}
}