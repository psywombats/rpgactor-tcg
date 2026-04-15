using System.Linq;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private CharaModelView chara;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private ListView statList;
        [SerializeField] private ListView abilList;
        
        public void Populate(CharacterCard card)
        {
            chara.Sprite = card.Sprite;
            nameText.text = card.CharacterName;

            var statTuples = card.Stats.ToTuples()
                .Where(tuple => tuple.Item1 is Stat.MHP or Stat.ATK or Stat.DEF or Stat.SPD 
                                || (tuple.Item1 == Stat.MP && card.IsLeader));
            statList.Populate(statTuples, (obj, statAndValue) =>
            {
                obj.GetComponent<StatView>().Populate(statAndValue);
            });
            abilList.Populate(card.AbilityCards, (obj, abil) =>
            {
                obj.GetComponent<AbilView>().Populate(abil, card);
            });
        }
    }
}
