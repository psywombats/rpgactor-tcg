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
            loadButton.enabled = CampaignManager.Instance.DoesDeckExist(deckIndex);
            
            saveLabel.text = $"Save as Deck {deckIndex + 1}";
            loadLabel.text = $"Load Deck {deckIndex + 1}";
        }

        private void SaveDeck()
        {
            CampaignManager.Instance.SaveDeckAs(editor.Deck, DeckIndex);
            editor.Repopulate();
        }

        private void LoadDeck()
        {
            editor.Populate(CampaignManager.Instance.GetDeck(DeckIndex));
        }
    }
}