using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class TopPlayerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text winLossLabel;
        [SerializeField] private GameObject myPlayerArea;

        public void Populate((EntrantModel entrant, int rank) rankedPlayer)
        {
            nameLabel.text = $"{rankedPlayer.rank.ToString()}. {rankedPlayer.entrant.EntrantName}";
            winLossLabel.text = $"{rankedPlayer.entrant.CurrentRoundResult.Wins} - {rankedPlayer.entrant.CurrentRoundResult.Losses}";
            myPlayerArea.SetActive(rankedPlayer.entrant == CampaignManager.Instance.Player);
        }
    }
}