using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
			var players = LogParser.GetPlayerList();
			foreach (var p in players)
			{
				Console.WriteLine("{0} {1}", p.profileId, p.name);
			}

			Console.ReadLine();
		}
	}
}