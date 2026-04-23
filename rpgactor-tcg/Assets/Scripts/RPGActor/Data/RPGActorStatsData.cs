using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsData
    {
        [JsonProperty] public RPGActorTCGData tcg;
        [JsonProperty] public RPGActorStatsRMMZ rmmz;
        [JsonProperty] public RPGActorStatsDND dnd;
        [JsonProperty("$type")] public string type;
        [JsonProperty] public DateTime createdAt;
        [JsonProperty] public DateTime updatedAt;

        // TODO: more stat classes
        private List<string> classes;
        public IEnumerable<string> Classes
        {
            get
            {
                if (classes == null)
                {
                    classes = new List<string>();
                    if (rmmz?.@class != null)
                    {
                        classes.Add(rmmz.@class);
                    }
                    
                    if (dnd?.identity?.@class != null)
                    {
                        classes.Add(dnd.identity.@class);
                    }
                    if (dnd?.identity?.race != null)
                    {
                        classes.Add(dnd.identity.race);
                    }
                }
                return classes;
            }
        }

        public GenericRPGStat BestStat
        {
            get
            {
                if (dnd?.abilities != null)
                {
                    return dnd.abilities.HighestStat;
                }

                if (rmmz != null)
                {
                    return rmmz.HighestStat;
                }

                return GenericRPGStat.None;
            }
        }
    }
}