using System.Collections.Generic;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorTCGData
    {
        [JsonProperty] public int hp;
        [JsonProperty] public int mp;
        [JsonProperty] public int atk;
        [JsonProperty] public int def;
        [JsonProperty] public int spd;
        [JsonProperty] public string ability;
        
        [JsonProperty("_meta")] public Dictionary<string, string> meta;
    }
}