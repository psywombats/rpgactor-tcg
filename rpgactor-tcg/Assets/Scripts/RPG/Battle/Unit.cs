using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgActorTGC
{
    public class Unit
    {
        public CharacterCard Card { get; }
        private readonly LaneType originalLane;

        public Party Party { get; }
        public List<AbilityInstance> Abilities { get; } = new();
        public int HP => (int)this[Stat.HP];
        public LaneType Lane { get; set; }
        public SpritesheetData Sprite => Card.Sprite;
        public Element Element => Card.Element;
        
        public bool IsLeader => Card.IsLeader;
        public bool IsDead => HP <= 0;

        public string PrettyName => CharacterName; // TODO: pretty names: $"<color=\"{Party.Color}\">{CharacterName}</color>";
        public string CharacterName => Card.CharacterName;
        public string LivenessString => $"{(HP >= 0 ? HP : 0)}/{this[Stat.MHP]}";
        public string LivenessChar => IsDead
            ? IsLeader ? "X" : "x" 
            : IsLeader ? "0" : "o";
        public string CompositionString => $"<color={Party.Color}>{Card.CompositionString}</color> ({Lane.ToString()[0]})";
        public string StateString => $"{{ {CompositionString}: {this[Stat.HP]}/{this[Stat.MHP]} " +
                                     $"{this[Stat.ATK]},{this[Stat.DEF]},{this[Stat.SPD]},{this[Stat.MP]} {Card.AbilString} }}";

        public StatSet Stats { get; } = new();
        public float this[Stat tag]
        {
            get => Stats[tag];
            set => Stats[tag] = value;
        }

        public Unit(Party party, CharacterCard card, LaneType lane)
        {
            Party = party;
            Card = card;
            Lane = lane;
            originalLane = Lane;

            foreach (var abil in Card.AbilityCards)
            {
                Abilities.Add(new AbilityInstance(this, abil));
            }

            Reset();
        }

        public override string ToString() => CompositionString;

        #region Battle sim

        public void Reset()
        {
            foreach (var abil in Abilities)
            {
                abil.Reset();
            }
            
            Stats.SetTo(Card.Stats);
            Stats[Stat.HP] = Stats[Stat.MHP];
            Lane = originalLane;
        }

        public async Task SimulateTurnAsync(BattleModel battle)
        {
            if (IsDead)
            {
                return;
            }

            if (battle.UseVerboseLogging) battle.SimLog($"Next up: {StateString}");
            if (!battle.IsSim) await battle.View.ViewForUnit[this].AnimateTurnStartAsync();

            if (this[Stat.MP] > 0)
            {
                Party.GenerateMp((int)this[Stat.MP]);
                if (battle.UseVerboseLogging) battle.SimLog($"Party MP is now: {Party.MP}");
                if (!battle.IsSim) await battle.View.GenerateMPAsync(Party, this, (int)this[Stat.MP]);
            }
            
            foreach (var abil in Abilities)
            {
                await abil.SimulateTurnAsync(battle);
            }

            if (!IsDead)
            {
                var opponent = battle.GetOppositeUnit(this);
                if (opponent != null)
                {
                    for (var i = 0; i < (Stats.Is(Stat.DUALSTRIKE) ? 2 : 1); i++)
                    {
                        await AttackUnitAsync(battle, opponent);
                        if (!battle.IsSim)
                        {
                            battle.View.RepopulateUnit(this);
                            battle.View.RepopulateUnit(opponent);
                        }
                    }
                }
                else
                {
                    if (battle.UseVerboseLogging) battle.SimLog("No viable opponent.");
                    if (!battle.IsSim) await battle.View.WriteLineAsync($"{PrettyName} had no one to attack.", true);
                }
            }

            if (battle.UseVerboseLogging) battle.SimLog("");
            if (!battle.IsSim) await battle.View.WriteLineAsync("");
        }

        public async Task AttackUnitAsync(BattleModel battle, Unit opponent)
        {
            if (opponent.IsDead) return;
            var dmg = (int)(this[Stat.ATK] - opponent[Stat.DEF]);
            if (dmg < 0) dmg = 0;
            dmg = await opponent.TakeDamageAsync(battle, dmg);
            if (battle.UseVerboseLogging) battle.SimLog($"Attacked {opponent.CompositionString} for {dmg} damage " +
                                                     $"({opponent[Stat.HP]} => {opponent[Stat.HP] - dmg})");
            if (!battle.IsSim) await battle.View.AnimateAttackAsync(this, opponent, dmg);
            opponent.CleanupPostAttack(battle);
            if (opponent.IsDead)
            {
                if (battle.UseVerboseLogging) battle.SimLog("Knock out!!");
                if (!battle.IsSim) await battle.View.WriteLineAsync($"{opponent.PrettyName} has fallen!", true);
            }
            else if (opponent.Stats.Is(Stat.COUNTER))
            {
                if (!battle.IsSim) await battle.View.WriteLineAsync($"{opponent.PrettyName} counterattacks.");
                await opponent.AttackUnitAsync(battle, this);
            }
        }

        public async Task<int> TakeDamageAsync(BattleModel battle, int dmg)
        {
            if (this[Stat.BLOCK] > 0)
            {
                this[Stat.BLOCK] -= 1;
                if (!battle.IsSim) await battle.View.WriteLineAsync($"...but {PrettyName} blocked the damage.");
                return 0;
            }
            else
            {
                this[Stat.HP] -= dmg;
                if (HP > this[Stat.MHP]) this[Stat.HP] = this[Stat.MHP];
                if (HP < 0) this[Stat.HP] = 0;
                return dmg;
            }
        }

        public void CleanupPostAttack(BattleModel battle)
        {
            if (IsDead)
            {
                var promotion = Party.CheckPromotion();
                if (promotion != null)
                {
                    Party.SwapSpaces(promotion.Item1, promotion.Item2);
                    if (!battle.IsSim) battle.View.AnimateSwapAsync(promotion).Forget();
                }
            }
        }
        
        #endregion
    }
}