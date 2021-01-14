using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			Database db = new Database();
			db.LoadFromFile();

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db).FilterByMatchType(MatchTypeId._1v1_).FilterByCompletionTime(1609459200, 1610599515);
			var dict = mab.GetOrderedMapPlayCount();

			foreach (var d in dict)
			{
				Console.WriteLine(d.Value + " " + d.Key);
			}

			Console.ReadLine();
		}
	}
}