using System.Collections.Generic;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorSpritesheetData
    {
        [JsonProperty("ref")] public Dictionary<string, string> @ref;
        [JsonProperty] public int size;
        [JsonProperty("$type")] public string type;
        [JsonProperty] public string mimeType;
    }
}