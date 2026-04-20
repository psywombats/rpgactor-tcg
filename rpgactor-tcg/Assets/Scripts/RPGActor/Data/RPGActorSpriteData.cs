using System;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorSpriteData
    {
        [JsonProperty] public int rows;
        [JsonProperty("$type")] public string type;
        [JsonProperty] public int width;
        [JsonProperty] public int frames;
        [JsonProperty] public int height;
        [JsonProperty] public int columns;
        [JsonProperty] public DateTime createdAt;
        [JsonProperty] public int frameWidth;
        [JsonProperty] public int frameHeight;
        [JsonProperty] public RPGActorSpritesheetData spriteSheet;
        [JsonProperty] public string url;
    }
}