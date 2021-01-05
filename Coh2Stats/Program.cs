using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			string s1 = Utilities.GetSteamIdFromProfileId("1186615");
			string s2 = Utilities.GetProfileIdFromSteamId("76561198050674754");

			Console.WriteLine(s1);
			Console.WriteLine(s2);

			Console.ReadLine();
		}
	}
}