using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsData
    {
        [JsonProperty] public RPGActorTCGData tcg;
        [JsonProperty("$type")] public string type;
        [JsonProperty] public DateTime createdAt;
        [JsonProperty] public DateTime updatedAt;

        // TODO: other stat classes
        public IEnumerable<string> Classes => Enumerable.Empty<string>();
    }
}