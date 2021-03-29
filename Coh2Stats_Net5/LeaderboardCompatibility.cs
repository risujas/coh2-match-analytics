namespace Coh2Stats_Net5
{
	public class LeaderboardCompatibility
	{
		public static LeaderboardId GetLeaderboardByRace(RaceId raceId)
		{
			if (raceId == RaceId.German)
			{
				return LeaderboardId._1v1German_;
			}

			else if (raceId == RaceId.Soviet)
			{
				return LeaderboardId._1v1Soviet_;
			}

			else if (raceId == RaceId.WGerman)
			{
				return LeaderboardId._1v1WestGerman_;
			}

			else if (raceId == RaceId.AEF)
			{
				return LeaderboardId._1v1AEF_;
			}

			else
			{
				return LeaderboardId._1v1British_;
			}
		}

		public static RaceFlag GenerateRaceFlag(bool includeGerman, bool includeSoviet, bool includeAEF, bool includeWGerman, bool includeBritish)
		{
			RaceFlag germans = RaceFlag.None;
			RaceFlag soviets = RaceFlag.None;
			RaceFlag aef = RaceFlag.None;
			RaceFlag wgermans = RaceFlag.None;
			RaceFlag british = RaceFlag.None;

			if (includeGerman)
			{
				germans = RaceFlag.German;
			}

			if (includeSoviet)
			{
				soviets = RaceFlag.Soviet;
			}

			if (includeAEF)
			{
				aef = RaceFlag.AEF;
			}

			if (includeWGerman)
			{
				wgermans = RaceFlag.WGerman;
			}

			if (includeBritish)
			{
				british = RaceFlag.British;
			}

			RaceFlag combinedFlags = germans | soviets | aef | wgermans | british;
			return combinedFlags;
		}
	}
}
