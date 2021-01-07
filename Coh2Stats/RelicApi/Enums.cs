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
        CustomFriends = 0,
		Auto1v1 = 1,
		Auto2v2 = 2,
		Auto3v3 = 3,
		Auto4v4 = 4,
        EasyAi2v2 = 5,
        MediumAi2v2 = 6,
        HardAi2v2 = 7,
        ExpertAi2v2 = 8,
        EasyAi3v3 = 9,
        MediumAi3v3 = 10,
        HardAi3v3 = 11,
        ExpertAi3v3 = 12,
        EasyAi4v4 = 13,
        MediumAi4v4 = 14,
        HardAi4v4 = 15,
        ExpertAi4v4 = 16,
        CustomPublic = 22
	}
}
