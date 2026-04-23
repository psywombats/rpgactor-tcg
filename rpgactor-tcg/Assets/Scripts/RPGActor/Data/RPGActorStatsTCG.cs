using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsTCG
    {
        public int hp;
        public int mp;
        public int atk;
        public int def;
        public int spd;
        public string abil;
    }
}