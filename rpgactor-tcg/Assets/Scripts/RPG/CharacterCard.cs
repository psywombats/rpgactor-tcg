using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class CharacterCard
    {
        public CharacterData Data { get; }

        public bool IsLeader => Data.isLeader;
        public List<AbilityCard> AbilityCards { get; } = new();
        public StatSet Stats => Data.stats;

        public string CharacterName => Data.characterName;
        public SpritesheetData Sprite => Data.sprite;
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
        
        public RPGActorModel Actor { get; set; }
        
        private readonly List<(RPGActorModel actor, int score)> sortedScores = new();
        
        public CharacterCard(CharacterData data)
        {
            Data = data;

            foreach (var abil in data.abilities)
            {
                AbilityCards.Add(CardCache.Instance.GetOrCreateCard(abil));
            }
        }
        
        public float this[Stat tag] => Data.stats[tag];

        public override string ToString() => CompositionString;
        
        public (RPGActorModel actor, int score) GetHighestUnasignedScore()
        {
            return sortedScores.FirstOrDefault(score => score.actor.Card == null);
        }
        
        public void CalculateScores(IEnumerable<RPGActorModel> actors)
        {
            foreach (var actor in actors)
            {
                sortedScores.Add((actor, actor.DistanceFromCard(this)));
            }
            sortedScores.Sort((a, b) => a.score.CompareTo(b.score));
        }
    }
}