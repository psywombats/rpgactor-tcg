using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorData
    {
        [JsonProperty] public string did;
        [JsonProperty] public string pds;
        [JsonProperty] public bool pdsAvailable;
        [JsonProperty] public string handle;
        [JsonProperty] public string displayName;
        [JsonProperty] public string avatar;
        [JsonProperty] public RPGActorSpriteData sprite;
        [JsonProperty] public RPGActorStatsData stats;
        [JsonProperty] public bool hasSprite;
        [JsonProperty] public bool hasStats;
        [JsonProperty] public DateTime cachedAt;
        [JsonProperty] public DateTime lastAccessed;

        public bool IsValidForTCG() => hasSprite
                                       && sprite != null
                                       && !string.IsNullOrEmpty(displayName.ToAscii());

        public IEnumerable<string> Classes => stats == null ? Enumerable.Empty<string>() : stats.Classes;
    }
}
