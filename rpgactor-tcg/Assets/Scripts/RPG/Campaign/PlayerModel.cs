namespace RpgActorTGC
{
    public class PlayerModel : EntrantModel
    {
        public override string EntrantName => "You";

        public Deck[] MyDecks { get; }

        public PlayerModel()
        {
            MyDecks = new Deck[ConstantsData.Instance.deckCount];
            for (var i = 0; i < MyDecks.Length; i++)
            {
                MyDecks[i] = new Deck($"Deck {i + 1}")
                {
                    DeckIndex = i
                };
            }
        }

        public void SaveDeckAs(Deck deck, int slot)
        {
            deck.DeckName = $"Deck {slot + 1}";
            deck.DeckIndex = slot;
            MyDecks[slot] = deck;
        }
        
        public Deck GetDeck(int index) => MyDecks[index];

        public void SetDeckForCurrentRound(Deck deck) => CurrentDeck = deck;
    }
}