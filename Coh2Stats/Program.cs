using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main(string[] args)
		{
			var games = MatchAnalysis.Build1v1MatchList(1, 5, 24*14);
			foreach (var g in games)
			{
				Console.WriteLine("Game --------------------------------");
				Console.WriteLine(g.completiontime + " (" + DateTimeOffset.FromUnixTimeSeconds(g.completiontime) + ")");
				Console.WriteLine(g.id);
			}
			Console.ReadLine();
		}
	}
}