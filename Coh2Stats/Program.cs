using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(RelicApi.MatchTypeId._1v1_, 10);

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches();
			var games = mab.FilterByMatchType(RelicApi.MatchTypeId._1v1_);
			var dict = games.GetOrderedMapPlayCount();

			foreach(var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.ReadLine();
		}
	}
}