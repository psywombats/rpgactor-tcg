using System;
using UnityEngine;

namespace RpgActorTGC
{
    public class ResultsSpriteView : MonoBehaviour
    {
        [SerializeField] private CharaModelView charaModel;
        [SerializeField] private GameObject leaderArea;
        [SerializeField] private TooltipSpawnComponent tooltip;

        public void Populate(CharacterCard card)
        {
            charaModel.Sprite = card.Sprite;
            leaderArea.SetActive(card.IsLeader);
            
            var message = card.CharacterName + "\n";
            foreach (var (stat, value) in card.Stats.ToTuples())
            {
                if (stat is Stat.ATK or Stat.MHP or Stat.SPD
                    || (stat is Stat.DEF && value > 0)
                    || (stat is Stat.MP && card.IsLeader))
                {
                    message += $" <sprite name=\"{stat.ToString().ToLowerInvariant()}\">{value}";
                }
            }
            foreach (var abil in card.AbilityCards)
            {
                message += $"\n{abil.GetShortDescription(card, true)}";
            }
            tooltip.Message = message;
        }
    }
}