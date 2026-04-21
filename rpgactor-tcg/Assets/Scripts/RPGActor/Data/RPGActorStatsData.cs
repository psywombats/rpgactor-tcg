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
        [JsonProperty("$type")] public string type;
        [JsonProperty] public DateTime createdAt;
        [JsonProperty] public DateTime updatedAt;

        // TODO: other stat classes
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
                }
                return classes;
            }
        }
    }
}