using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class CardCache : SingletonBehaviour<CardCache>
    {
        private readonly Dictionary<CharacterData, CharacterCard> heroCards = new();
        private readonly Dictionary<CharacterData, CharacterCard> leaderCards = new();
        private readonly Dictionary<AbilityData, AbilityCard> abilityCards = new();
        private readonly Dictionary<DeckData, Deck> decks = new();

        protected override void Init()
        {
            foreach (var cardData in DBManager.Instance.GetAll<CharacterData>())
            {
                GetOrCreateCard(cardData);
            }
        }
        
        public IEnumerable<CharacterCard> AllHeroCards => heroCards.Values;
        public IEnumerable<CharacterCard> AllLeaderCards => leaderCards.Values;
        public IEnumerable<CharacterCard> AllCards => AllHeroCards.Union(AllLeaderCards);

        public bool TryGetCharacter(string key, out CharacterCard card)
        {
            var data = DBManager.Instance.GetOrNull<CharacterData>(key);
            card = data == null ? null : GetOrCreateCard(data);
            return card != null;
        }
        
        public CharacterCard GetRandomCharacter(bool isLeader,
            List<CharacterCard> availableHeroes = null, List<CharacterCard> availableLeaders = null)
        {
            var heroes = availableHeroes ?? AllHeroCards;
            var leaders = availableLeaders ?? AllLeaderCards;
            var set = isLeader ? leaders : heroes;
            return set.Choose();
        }
        
        public CharacterCard GetOrCreateCard(CharacterData data)
        {
            if (heroCards.TryGetValue(data, out var heroCard))
            {
                return heroCard;
            }
            if (leaderCards.TryGetValue(data, out var leaderCard))
            {
                return leaderCard;
            }
            
            var card = new CharacterCard(data);
            var cache = data.isLeader ? leaderCards : heroCards;
            cache.Add(data, card);
            return card;
        }
        
        public AbilityCard GetOrCreateCard(AbilityData data)
        {
            if (!abilityCards.TryGetValue(data, out var card))
            {
                card = new AbilityCard(data);
                abilityCards.Add(data, card);
            }
            return card;
        }
        
        public Deck GetOrCreateDeck(DeckData data)
        {
            if (!decks.TryGetValue(data, out var deck))
            {
                deck = new Deck(data);
                decks.Add(data, deck);
            }
            return deck;
        }

        public void ResetAssignments()
        {
            foreach (var card in AllCards)
            {
                card.Actor = null;
            }
        }
    }
}