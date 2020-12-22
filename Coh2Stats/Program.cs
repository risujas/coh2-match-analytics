using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			var games = MatchAnalysis.Get1v1MatchList(20, 10, 24*30);
			MatchAnalysis.ResultBundle bundle = new MatchAnalysis.ResultBundle();

			bundle.ParseMatches(games);
			bundle.PrintWinRates();

			Console.ReadLine();
		}
	}
}