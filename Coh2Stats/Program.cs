using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(MatchTypeId._1v1_);

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches().FilterByMatchType(MatchTypeId._1v1_);
			var games = mab.FilterByMatchType(MatchTypeId._1v1_);

			var dict = games.GetOrderedMapPlayCount();
			foreach (var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.WriteLine();

			dict = games.FilterByMinimumHighRank(253333, false).GetOrderedMapPlayCount();
			foreach (var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.WriteLine();

			dict = games.FilterByMinimumHighRank(253333, true).GetOrderedMapPlayCount();
			foreach (var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.ReadLine();
		}
	}
}