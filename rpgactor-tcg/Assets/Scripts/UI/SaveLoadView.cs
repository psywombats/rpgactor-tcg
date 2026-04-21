using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class SaveLoadView : MonoBehaviour
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text saveLabel;
        [SerializeField] private TMP_Text loadLabel;
        [SerializeField] private TooltipSpawnComponent saveTooltip;
        [SerializeField] private TooltipSpawnComponent loadTooltip;
        
        public int DeckIndex { get; private set; }
        
        private DeckEditView editor;

        public void Awake()
        {
            saveButton.onClick.AddListener(SaveDeck);
            loadButton.onClick.AddListener(LoadDeck);
        }

        public void Populate(int deckIndex, DeckEditView newEditor)
        {
            editor = newEditor;
            DeckIndex = deckIndex;
            loadButton.enabled = deckIndex != editor.Deck.DeckIndex;
            saveButton.enabled = deckIndex != editor.Deck.DeckIndex;
            
            saveLabel.text = $"Save as Deck {deckIndex + 1}";
            loadLabel.text = $"Load Deck {deckIndex + 1}";

            //saveTooltip.Message = $"Stores the party in slot {deckIndex + 1} so it can be edited later";
            loadTooltip.Message = $"Switch to editing party {deckIndex + 1}";
        }

        public void Repopulate() => Populate(DeckIndex, editor);

        private void SaveDeck()
        {
            var newDeck = new Deck(editor.Deck);
            CampaignManager.Instance.Player.SaveDeckAs(newDeck, DeckIndex);
            editor.Populate(newDeck);
            Repopulate();
        }

        private void LoadDeck()
        {
            editor.Populate(CampaignManager.Instance.Player.GetDeck(DeckIndex));
            Repopulate();
        }
    }
}