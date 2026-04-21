using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class TopPlayerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text winLossLabel;
        [SerializeField] private GameObject myPlayerArea;
        [SerializeField] private TooltipSpawnComponent tooltip;
        
        public void Populate((EntrantModel entrant, int rank) rankedPlayer, bool useOverallTopPlayers)
        {
            nameLabel.text = $"{rankedPlayer.rank.ToString()}. {rankedPlayer.entrant.EntrantName}";
            winLossLabel.text = useOverallTopPlayers 
                ? $"{rankedPlayer.entrant.LifetimeWins} - {rankedPlayer.entrant.LifetimeLosses}" 
                : $"{rankedPlayer.entrant.CurrentRoundResult.Wins} - {rankedPlayer.entrant.CurrentRoundResult.Losses}";
            tooltip.Message = useOverallTopPlayers
                ? $"{rankedPlayer.entrant.CurrentRoundResult.Wins}-{rankedPlayer.entrant.CurrentRoundResult.Losses} this round"
                : $"{rankedPlayer.entrant.LifetimeWins}-{rankedPlayer.entrant.LifetimeLosses} overall" ;
            
            myPlayerArea.SetActive(rankedPlayer.entrant == CampaignManager.Instance.Player);
        }
    }
}