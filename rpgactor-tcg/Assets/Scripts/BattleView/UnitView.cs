using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RpgActorTGC
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private CharaModelView chara;
        [SerializeField] private HPSliderView hpSlider;
        [SerializeField] private ListView statList;
        [SerializeField] private ListView abilList;
        [Space]
        [SerializeField] private List<GameObject> liveObjects;
        [SerializeField] private List<GameObject> deadObjects;
        [SerializeField] private SpritesheetData deadSprite;

        private Unit unit;
        
        public void Populate(Unit newUnit)
        {
            unit = newUnit;

            foreach (var obj in liveObjects) obj.SetActive(!unit.IsDead);
            foreach (var obj in deadObjects) obj.SetActive(unit.IsDead);
            
            chara.Sprite = unit.IsDead ? unit.Sprite : deadSprite;
            chara.Animates = !unit.IsDead;
            
            hpSlider.Populate((int)newUnit[Stat.HP], (int)newUnit[Stat.MHP]);

            var statTuples = unit.Stats.ToTuples()
                .Where(tuple => tuple.Item1 is Stat.ATK or Stat.DEF or Stat.SPD 
                                || (tuple.Item1 == Stat.MP && unit.IsLeader));
            statList.Populate(statTuples, (obj, statAndValue) =>
            {
                obj.GetComponent<StatView>().Populate(statAndValue);
            });
            abilList.Populate(unit.Abilities, (obj, abil) =>
            {
                obj.GetComponent<AbilView>().Populate(abil);
            });
        }
    }
}