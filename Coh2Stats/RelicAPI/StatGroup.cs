using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats.RelicAPI
{
	public class StatGroup
	{
		[JsonProperty("id")] public int Id { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("type")] public int Type { get; set; }
		[JsonProperty("members")] public List<PlayerIdentity> Members { get; set; }
	}
}
