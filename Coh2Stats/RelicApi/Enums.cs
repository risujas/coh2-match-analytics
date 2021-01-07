using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats.RelicApi
{
	[Flags]
	enum RaceFlag
	{
		German = 1,
		Soviet = 2,
		WGerman = 4,
		AEF = 8,
		British = 16
	}

	enum RaceId
	{
		German = 0,
		Soviet = 1,
		WGerman = 2,
		AEF = 3,
		British = 4
	}

	enum FactionId
	{
		Axis = 0,
		Allies = 1
	}

	enum MatchTypeId
	{
        _Custom_ = 0,
		_1v1_ = 1,
		_2v2_ = 2,
		_3v3_ = 3,
		_4v4_ = 4,
        _2v2AiEasy_ = 5,
        _2v2AiMedium_ = 6,
		_2v2AiHard_ = 7,
		_2v2AiExpert_ = 8,
        _3v3AiEasy_ = 9,
		_3v3AiMedium_ = 10,
		_3v3AiHard_ = 11,
		_3v3AiExpert_ = 12,
		_4v4AiEasy_ = 13,
		_4v4AiMedium_ = 14,
		_4v4AiHard_ = 15,
		_4v4AiExpert_ = 16,
        _CustomPublic_ = 22
	}

	enum LeaderboardId
	{
		_CustomGerman_ = 0,
		_CustomSoviet_ = 1,
		_CustomWestGerman_ = 2,
		_CustomAEF_ = 3,
		_1v1German_ = 4,
		_1v1Soviet_ = 5,
		_1v1WestGerman_ = 6,
		_1v1AEF_ = 7,
		_2v2German_ = 8,
		_2v2Soviet_ = 9,
		_2v2WestGerman_ = 10,
		_2v2AEF_ = 11,
		_3v3German_ = 12,
		_3v3Soviet_ = 13,
		_3v3WestGerman_ = 14,
		_3v3AEF_ = 15,
		_4v4German_ = 16,
		_4v4Soviet_ = 17,
		_4v4WestGerman_ = 18,
		_4v4AEF_ = 19,
		_TeamOf2Axis_ = 20,
		_TeamOf2Allies_ = 21,
		_TeamOf3Axis_ = 22,
		_TeamOf3Allies_ = 23,
		_TeamOf4Axis_ = 24,
		_TeamOf4Allies_ = 25,
		_2v2AIEasyAxis_ = 26,
		_2v2AIEasyAllies_ = 27,
		_2v2AIMediumAxis_ = 28,
		_2v2AIMediumAllies_ = 29,
		_2v2AIHardAxis_ = 30,
		_2v2AIHardAllies_ = 31,
		_2v2AIExpertAxis_ = 32,
		_2v2AIExpertAllies_ = 33,
		_3v3AIEasyAxis_ = 34,
		_3v3AIEasyAllies_ = 35,
		_3v3AIMediumAxis_ = 36,
		_3v3AIMediumAllies_ = 37,
		_3v3AIHardAxis_ = 38,
		_3v3AIHardAllies_ = 39,
		_3v3AIExpertAxis_ = 40,
		_3v3AIExpertAllies_ = 41,
		_4v4AIEasyAxis_ = 42,
		_4v4AIEasyAllies_ = 43,
		_4v4AIMediumAxis_ = 44,
		_4v4AIMediumAllies_ = 45,
		_4v4AIHardAxis_ = 46,
		_4v4AIHardAllies_ = 47,
		_4v4AIExpertAxis_ = 48,
		_4v4AIExpertAllies_ = 49,
		_CustomBritish_ = 50,
		_1v1British_ = 51,
		_2v2British_ = 52,
		_3v3British_ = 53,
		_4v4British_ = 54,
	}
}
