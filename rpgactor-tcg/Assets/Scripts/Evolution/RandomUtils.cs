using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public static class RandomUtils
    {
        private static Random rand = new Random();
        
        public static int Range(int min, int max) => rand.Next(min, max);

        public static float NextFloat() => (float)rand.NextDouble();
        
        public static T Choose<T>(this IEnumerable<T> set)
        {
            var count = set.Count();
            if (count == 0) throw new InvalidOperationException("Cannot choose an empty set.");
            var index = Range(0, count);
            return set.ElementAt(index);
        }
        
        public static T WeightedChoose<T>(List<T> candidates)
        {
            var r = rand.NextDouble();
            r *= r; // parabolic weighting
            var index = UnityEngine.Mathf.FloorToInt((float)(r * candidates.Count));
            return candidates[index];
        }

        public static bool Flip() => rand.NextDouble() < 0.5;
        
        public static void Shuffle<T>(IList<T> list) {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}