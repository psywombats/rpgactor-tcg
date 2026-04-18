using System;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private TMP_Text deckNameText;
        [SerializeField] private CardView leaderView;
        [SerializeField] private ListView followerViews;
        
        public void Populate(Deck deck, Action<CardView> onSelect = null)
        {
            deckNameText.text = deck.DeckName;
            leaderView.Populate(deck.Leader);
            followerViews.Populate(deck.Followers, (obj, card) =>
            {
                obj.GetComponent<CardView>().Populate(card, onSelect);
            });
        }
    }
}