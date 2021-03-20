namespace Coh2Stats
{
	class LeaderboardCompatibility
	{
		public static LeaderboardId GetLeaderboardByRaceId(RaceId raceId)
		{
			if (raceId == RaceId.German)
			{
				return LeaderboardId.German1v1;
			}

			else if (raceId == RaceId.Soviet)
			{
				return LeaderboardId.Soviet1v1;
			}

			else if (raceId == RaceId.WGerman)
			{
				return LeaderboardId.WGerman1v1;
			}

			else if (raceId == RaceId.AEF)
			{
				return LeaderboardId.AEF1v1;
			}

			else
			{
				return LeaderboardId.British1v1;
			}
		}
	}
}
