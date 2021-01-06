using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine(PlayerIdentityTracker.GetNumLoggedPlayers());
			Console.ReadLine();
		}
	}
}