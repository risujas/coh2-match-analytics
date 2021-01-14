using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(MatchTypeId._1v1_);
			while(true)
			{
				db.ProcessMatches();
			}
		}
	}
}