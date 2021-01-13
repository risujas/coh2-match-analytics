using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class StatGroup
	{
		[JsonProperty("id")] public int Id { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("type")] public int Type { get; set; }
		[JsonProperty("members")] public List<PlayerIdentity> Members { get; set; }
	}

	class StatGroupTracker
	{
		private static List<StatGroup> statGroups = new List<StatGroup>();
		public static ReadOnlyCollection<StatGroup> StatGroups
		{
			get { return statGroups.AsReadOnly(); }
		}

		public static StatGroup GetStatGroupById(int id)
		{
			foreach (var x in statGroups)
			{
				if (x.Id == id)
				{
					return x;
				}
			}

			return null;
		}

		public static void LogStatGroup(StatGroup psg)
		{
			if (GetStatGroupById(psg.Id) == null)
			{
				statGroups.Add(psg);
			}
		}
	}
}
