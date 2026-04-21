using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsRMMZ
    {
        [JsonProperty] public int hp;
        [JsonProperty] public int mp;
        [JsonProperty] public int tp;
        [JsonProperty] public int xp;
        [JsonProperty] public int agi;
        [JsonProperty] public int atk;
        [JsonProperty] public int cri;
        [JsonProperty] public int def;
        [JsonProperty] public int eva;
        [JsonProperty] public int hit;
        [JsonProperty] public int luk;
        [JsonProperty] public int mat;
        [JsonProperty] public int mdf;
        [JsonProperty("class")] public string @class;
        [JsonProperty] public int level;
        [JsonProperty] public int maxHp;
        [JsonProperty] public int maxMp;
        [JsonProperty] public int maxTp;
    }
}