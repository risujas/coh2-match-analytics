namespace Coh2Stats
{
	public class LeaderboardCompatibility
	{
		public static bool LeaderboardIsCompatibleWithGameMode(LeaderboardId leaderboardId, MatchTypeId gameMode)
		{
			if (gameMode == MatchTypeId._1v1_)
			{
				if (leaderboardId == LeaderboardId._1v1German_) { return true; }
				if (leaderboardId == LeaderboardId._1v1Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._1v1WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._1v1AEF_) { return true; }
				if (leaderboardId == LeaderboardId._1v1British_) { return true; }
			}

			if (gameMode == MatchTypeId._2v2_)
			{
				if (leaderboardId == LeaderboardId._2v2German_) { return true; }
				if (leaderboardId == LeaderboardId._2v2Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._2v2WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._2v2AEF_) { return true; }
				if (leaderboardId == LeaderboardId._2v2British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf2Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf2Axis_) { return true; }
			}

			if (gameMode == MatchTypeId._3v3_)
			{
				if (leaderboardId == LeaderboardId._3v3German_) { return true; }
				if (leaderboardId == LeaderboardId._3v3Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._3v3WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._3v3AEF_) { return true; }
				if (leaderboardId == LeaderboardId._3v3British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf3Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf3Axis_) { return true; }
			}

			if (gameMode == MatchTypeId._4v4_)
			{
				if (leaderboardId == LeaderboardId._4v4German_) { return true; }
				if (leaderboardId == LeaderboardId._4v4Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._4v4WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._4v4AEF_) { return true; }
				if (leaderboardId == LeaderboardId._4v4British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf4Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf4Axis_) { return true; }
			}

			return false;
		}
	}
}
