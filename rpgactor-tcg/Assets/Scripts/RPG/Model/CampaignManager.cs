using System;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class CampaignManager : SingletonBehaviour<CampaignManager>
    {
        public List<CharacterCard> UnlockedCards { get; } = new();

        private List<Deck> decks = new();

        protected override void Init()
        {
            base.Init();
            for (var i = 0; i < ConstantsData.Instance.deckCount; i++)
            {
                decks.Add(new Deck($"Deck {i+1}"));
            }
        }
        
        public Deck GetDeck(int index) => decks[index];
    }
}