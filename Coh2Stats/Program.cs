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
			var players = LogParser.GetPlayerList("C:/Users/johan/Documents/my games/company of heroes 2/warnings.log");

            Console.ReadLine();
		}
	}
}