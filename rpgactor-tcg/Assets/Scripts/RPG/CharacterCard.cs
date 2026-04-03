using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CharacterCard
    {
        public CharacterData Data { get; }

        public bool IsLeader => Data.isLeader;
        public List<AbilityCard> AbilityCards { get; } = new();
        public StatSet Stats => Data.stats;

        public string CharacterName => Data.characterName;
        public string CompositionString => IsLeader ? $"[{CharacterName}]" : CharacterName;

        private string abilString;
        public string AbilString
        {
            get
            {
                if (abilString == null)
                {
                    abilString = "";
                    foreach (var card in AbilityCards)
                    {
                        abilString += $"({card.GetShortDescription(this)})";
                    }
                }
                return abilString;
            }
        }
        
        public CharacterCard(CharacterData data)
        {
            Data = data;

            foreach (var abil in data.abilities)
            {
                AbilityCards.Add(new AbilityCard(abil));
            }
        }
        
        public float this[Stat tag] => Data.stats[tag];

        public override string ToString() => CompositionString;
    }
}