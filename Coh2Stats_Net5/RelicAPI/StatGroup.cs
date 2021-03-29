using Newtonsoft.Json;

using System.Collections.Generic;

namespace Coh2Stats_Net5.RelicAPI
{
	public class StatGroup
	{
		[JsonProperty("id")] public int Id { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("type")] public int Type { get; set; }
		[JsonProperty("members")] public List<PlayerIdentity> Members { get; set; }
	}
}
