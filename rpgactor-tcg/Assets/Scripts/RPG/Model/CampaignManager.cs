using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CampaignManager : SingletonBehaviour<CampaignManager>
    {
        public List<CharacterCard> UnlockedCards { get; } = new();

        private Deck[] decks;

        protected override void Init()
        {
            base.Init();

            decks = new Deck[ConstantsData.Instance.deckCount];
            for (var i = 0; i < decks.Length; i++)
            {
                decks[i] = new Deck($"Deck {i + 1}");
            }
        }

        public bool DoesDeckExist(int deckIndex) => GetDeck(deckIndex) != null;

        public void SaveDeckAs(Deck deck, int slot)
        {
            deck.DeckName = $"Deck {slot + 1}";
            decks[slot] = deck;
        }
        
        public Deck GetDeck(int index) => decks[index];
    }
}