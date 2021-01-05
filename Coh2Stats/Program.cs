using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.BuildPlayerList();
			db.BuildMatchList();
			db.PrintMatchList();

			Console.ReadLine();
		}
	}
}