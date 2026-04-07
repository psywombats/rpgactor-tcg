using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RpgActorTGC
{
    public static class RandomUtils
    {
        public static T Choose<T>(this IEnumerable<T> set)
        {
            var index = Random.Range(0, set.Count());
            return set.ElementAt(index);
        }
        
        public static T WeightedChoose<T>(List<T> candidates)
        {
            var r = Random.Range(0f, 1f);
            r *= r; // parabolic weighting
            var index = Mathf.FloorToInt(r * candidates.Count);
            return candidates[index];
        }

        public static bool Flip() => Random.Range(0f, 1f) > .5f;
        
        public static void Shuffle<T>(IList<T> list) {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = Random.Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}