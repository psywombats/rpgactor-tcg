using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CardCache : SingletonBehaviour<CardCache>
    {
        private readonly Dictionary<CharacterData, CharacterCard> characterCards = new();
        private readonly Dictionary<AbilityData, AbilityCard> abilityCards = new();
        private readonly Dictionary<DeckData, Deck> decks = new();

        public CharacterCard GetOrCreateCard(CharacterData data)
        {
            if (!characterCards.TryGetValue(data, out var card))
            {
                card = new CharacterCard(data);
                characterCards.Add(data, card);
            }
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