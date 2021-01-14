using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			Database db = new Database();
			db.LoadFromFile();

			var mab = MatchAnalyticsBundle.GetAllLoggedMatches(db);

			Console.ReadLine();
		}
	}
}