using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class OpponentButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        
        private DeckEditView editor;
        private Deck opponent;

        public void Awake()
        {
            button.onClick.AddListener(StartBattle);
        }

        public void Populate(Deck newOpponent, DeckEditView newEditor)
        {
            opponent = newOpponent;
            editor = newEditor;

            label.text = $"vs {newOpponent.DeckName}";
            //button.interactable = opponent != editor.Deck;
        }

        private void StartBattle()
        {
            if (editor.TryPopIncompletionDialog())
            {
                return;
            }

            if (opponent.IsIncomplete)
            {
                editor.PopDialogAsync($"Can't practice against {opponent.DeckName} because it doesn't have " +
                                      $"a leader and three followers.").Forget();
                return;
            }

            if (opponent == editor.Deck)
            {
                editor.PopDialogAsync($"Can't practice against {opponent.DeckName} because it doesn't have " +
                                      $"a leader and three followers.").Forget();
                return;
            }
            
            var battle = new BattleModel(new Party(editor.Deck), new Party(opponent));
            editor.GameplayView.PlaybackBattleAsync(battle).Forget();
        }
    }
}