using System.Collections.Generic;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorFullCachedData
    {
        [JsonProperty] public List<RPGActorData> actors;
    }
}