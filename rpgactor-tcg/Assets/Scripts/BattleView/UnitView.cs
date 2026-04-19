using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace RpgActorTGC
{
    [RequireComponent(typeof(RectTransform))]
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
        [Space]
        [SerializeField] private RectTransform spriteTransform;
        [SerializeField] private float attackMoveRatio = .5f;
        [SerializeField] private float attackMoveToDuration = .2f;
        [SerializeField] private float attackMoveBackDuration = .4f;
        [SerializeField] private float shakeDuration = .2f;
        [SerializeField] private Vector2 shakeStrength = new(0f, 20f);
        [SerializeField] private float shakeRandomness = 0f;
        [SerializeField] private int shakeVibrato = 10;

        public Unit Unit { get; private set; }

        private Vector3 initialPos;

        protected void Awake()
        {
            initialPos = spriteTransform.position;
        }

        public void Populate(Unit newUnit)
        {
            Unit = newUnit;

            foreach (var obj in liveObjects) obj.SetActive(!Unit.IsDead);
            foreach (var obj in deadObjects) obj.SetActive(Unit.IsDead);
            
            chara.Sprite = !Unit.IsDead ? Unit.Sprite : deadSprite;
            chara.Animates = !Unit.IsDead;

            spriteTransform.position = initialPos;
            
            hpSlider.Populate((int)newUnit[Stat.HP], (int)newUnit[Stat.MHP]);

            var statTuples = Unit.Stats.ToTuples()
                .Where(tuple => tuple.Item1 is Stat.ATK or Stat.DEF or Stat.SPD 
                                || (tuple.Item1 == Stat.MP && Unit.IsLeader));
            statList.Populate(statTuples, (obj, statAndValue) =>
            {
                obj.GetComponent<StatView>().Populate(statAndValue);
            });
            abilList.Populate(Unit.Abilities, (obj, abil) =>
            {
                obj.GetComponent<AbilView>().Populate(abil);
            });
        }

        public void Repopulate() => Populate(Unit);

        public async Task AnimateAttackAsync(UnitView victimView, int dmg)
        {
            var originalPos = spriteTransform.anchoredPosition;
            var targetPos = attackMoveRatio * victimView.spriteTransform.position
                            + (1f - attackMoveRatio) * spriteTransform.position;
            await spriteTransform.DOMove(targetPos, attackMoveToDuration).AsTask();
            await victimView.spriteTransform.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness).AsTask();
            await Task.WhenAll(victimView.hpSlider.TweenTo(victimView.Unit.Hp - dmg, attackMoveBackDuration),
                spriteTransform.DOAnchorPos(originalPos, attackMoveBackDuration).AsTask());
        }

        public Task SwapToUnitPosAsync(UnitView unit2View)
        {
            return spriteTransform.DOMove(unit2View.spriteTransform.position, attackMoveBackDuration).AsTask();
        }
    }
}