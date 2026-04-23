using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsData
    {
        [JsonProperty] public RPGActorStatsTCG tcg;
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

        private HashSet<GenericRPGStat> bestStats;
        public HashSet<GenericRPGStat> BestStats
        {
            get
            {
                if (bestStats == null)
                {
                    bestStats = new HashSet<GenericRPGStat>();
                    if (dnd?.abilities != null) bestStats.Add(dnd.abilities.HighestStat);
                    if (rmmz?.HighestStat != null) bestStats.Add(rmmz.HighestStat);
                }

                return bestStats;
            }
        }
    }
}