using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			var games = MatchAnalysis.Build1v1MatchList(1, 100, 24);
			Console.WriteLine(games.Count);
			Console.ReadLine();
		}
	}
}