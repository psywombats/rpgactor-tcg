using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Effekseer;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    [RequireComponent(typeof(RectTransform))]
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private CharaModelView chara;
        [SerializeField] private HPSliderView hpSlider;
        [SerializeField] private ListView statList;
        [SerializeField] private ListView abilList;
        [SerializeField] private MPView mp;
        [SerializeField] private TooltipSpawnComponent tooltip;
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
        [SerializeField] private float shakeRandomness = 10f;
        [SerializeField] private int shakeVibrato = 10;
        [Space]
        [SerializeField] private Image overlaySprite;
        [SerializeField] private float flashDuration = .2f;
        [SerializeField] private Color healColor = new(0.3f, .4f, .7f);
        [SerializeField] private Color damageColor = new(1f, 0f, 0f, .7f);
        [SerializeField] private Color turnStartColor = new(1f, 1f, 1f, .5f);
        [Space]
        [SerializeField] private BattleAnimPlayer animPlayer;

        public Unit Unit { get; private set; }

        private RectTransform rectTrans;
        private RectTransform RectTrans => rectTrans ??= GetComponent<RectTransform>();

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

            mp.Populate((int)Unit[Stat.MP]);
            var statTuples = Unit.Stats.ToTuples()
                .Where(tuple => tuple.Item1 is Stat.ATK or Stat.SPD 
                                || (tuple.Item1 == Stat.DEF && Unit[Stat.DEF] > 0));
            statList.Populate(statTuples, (obj, statAndValue) =>
            {
                obj.GetComponent<StatView>().Populate(statAndValue);
            });
            abilList.Populate(Unit.Abilities, (obj, abil) =>
            {
                obj.GetComponent<AbilView>().Populate(abil);
            });

            tooltip.Message = newUnit.CharacterName;
        }

        public void Repopulate() => Populate(Unit);

        public async Task AnimateAttackAsync(UnitView victimView, int dmg)
        {
            var originalPos = spriteTransform.anchoredPosition;
            var targetPos = attackMoveRatio * victimView.spriteTransform.position
                            + (1f - attackMoveRatio) * spriteTransform.position;
            await spriteTransform.DOMove(targetPos, attackMoveToDuration).AsTask();
            await victimView.AnimateDamageAsync(dmg);
            await spriteTransform.DOAnchorPos(originalPos, attackMoveBackDuration).AsTask();
        }

        public Task AnimateDamageAsync(int dmg)
        {
            var str = dmg > 0 ? shakeStrength : Vector2.zero;
            return Task.WhenAll(
                spriteTransform.DOShakeAnchorPos(shakeDuration, str, shakeVibrato, shakeRandomness).AsTask(),
                hpSlider.TweenTo(Unit.HP, attackMoveBackDuration),
                FlashAsync(damageColor));
        }

        public Task SwapToUnitPosAsync(UnitView unit2View)
        {
            return spriteTransform.DOMove(unit2View.spriteTransform.position, attackMoveBackDuration).AsTask();
        }

        public async Task AnimateTurnStartAsync()
        {
            await new WaitForSeconds(flashDuration / 2f);
            FlashAsync(turnStartColor).Forget();
        }

        public Task AnimateCastAsync(AbilityInstance abil)
        {
            return Task.WhenAll(
                PlayAnimAsync(abil.Element.Info().CastAnim),
                FlashAsync(abil.Element.Info().PrimaryColor));
        }

        public Task PlayAnimAsync(EffekseerEffectAsset effect) => animPlayer.PlayEffectAsync(RectTrans, effect);

        public async Task FlashAsync(Color color)
        {
            await overlaySprite.DOColor(color, flashDuration / 2f).AsTask();
            await overlaySprite.DOColor(Color.clear, flashDuration / 2f).AsTask();
        }
    }
}