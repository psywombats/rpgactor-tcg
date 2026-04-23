using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsDND 
    {
        // there is a bunch of crap in here I can't be assed to model at the moment
        [JsonProperty] public Abilities abilities;
        [JsonProperty] public Identity identity;

        [JsonObject(MemberSerialization.OptIn)]
        public class Abilities
        {
            [JsonProperty] public int cha;
            [JsonProperty] public int con;
            [JsonProperty] public int dex;
            [JsonProperty("int")] public int @int;
            [JsonProperty] public int str;
            [JsonProperty] public int wis;

            public GenericRPGStat HighestStat
            {
                get
                {
                    var max = new List<int> { cha, con, dex, @int, str, wis }.Max();
                    if (max == cha) return GenericRPGStat.CHA;
                    if (max == con) return GenericRPGStat.DEF;
                    if (max == dex) return GenericRPGStat.DEX;
                    if (max == @int) return GenericRPGStat.MAG;
                    if (max == str) return GenericRPGStat.STR;
                    if (max == wis) return GenericRPGStat.WIS;
                    return GenericRPGStat.None;
                }
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Identity
        {
            [JsonProperty] public int xp;
            [JsonProperty] public string race;
            [JsonProperty("class")] public string @class;
            [JsonProperty] public int level;
            [JsonProperty] public string alignment;
            [JsonProperty] public string background;
            [JsonProperty] public int proficiency;
        }
    }
}