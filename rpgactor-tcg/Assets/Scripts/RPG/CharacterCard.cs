using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CharacterCard
    {
        public CharacterData Data { get; }

        public bool IsLeader => Data.isLeader;
        public List<AbilityCard> AbilityCards { get; } = new();
        public StatSet Stats => Data.stats;
        
        public CharacterCard(CharacterData data)
        {
            Data = data;

            foreach (var abil in data.abilities)
            {
                AbilityCards.Add(new AbilityCard(abil));
            }
        }
        
        public float this[Stat tag] => Data.stats[tag];
    }
}