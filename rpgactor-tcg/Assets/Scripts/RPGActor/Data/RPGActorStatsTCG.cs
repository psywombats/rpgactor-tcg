using System;
using Newtonsoft.Json;
using UnityEngine;

namespace RpgActorTGC
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RPGActorStatsTCG
    {
        [JsonProperty] public int hp;
        [JsonProperty] public int mp;
        [JsonProperty] public int atk;
        [JsonProperty] public int def;
        [JsonProperty] public int spd;
        [JsonProperty] public string ability;

        public int DistanceFromCard(CharacterCard card)
        {
            var dist = 0;
            if (string.IsNullOrEmpty(ability) || ability != card.AbilString)
            {
                // does not match our exact ability or mp cost
                dist += 5;
                if (string.IsNullOrEmpty(ability) || ability.StripNonAlpha() != card.AbilString.StripNonAlpha())
                {
                    // does not match keyword either
                    dist += 5;
                }
            }

            var statDist = 0f;
            statDist += Mathf.Abs(hp - card[Stat.MHP]) / card[Stat.MHP];
            statDist += Mathf.Abs(def - card[Stat.DEF]) / card[Stat.DEF];
            statDist += Mathf.Abs(atk - card[Stat.ATK]) / card[Stat.ATK];
            statDist += Mathf.Abs(spd - card[Stat.SPD]) / card[Stat.SPD];
            statDist += Mathf.Abs(mp - card[Stat.MP]) / card[Stat.MP];
            return dist + Mathf.FloorToInt(statDist * 5f);
        }
    }
}