using System;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CardCache : SingletonBehaviour<CardCache>
    {
        private readonly Dictionary<CharacterData, CharacterCard> heroCards = new();
        private readonly Dictionary<CharacterData, CharacterCard> leaderCards = new();
        private readonly Dictionary<AbilityData, AbilityCard> abilityCards = new();
        private readonly Dictionary<DeckData, Deck> decks = new();

        public void Start()
        {
            foreach (var cardData in DBManager.Instance.GetAll<CharacterData>())
            {
                GetOrCreateCard(cardData);
            }
        }

        public CharacterCard GetRandomCharacter(bool isLeader)
        {
            var set = isLeader ? leaderCards : heroCards;
            return set.Values.Choose();
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
    }
}