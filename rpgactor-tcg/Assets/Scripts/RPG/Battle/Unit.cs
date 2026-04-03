using System.Collections.Generic;

namespace RpgActorTGC
{
    public class Unit
    {
        public CharacterCard Card { get; }
        private readonly LaneType originalLane;

        public Party Party { get; }
        public List<AbilityInstance> Abilities { get; } = new();
        public int Hp => (int)this[Stat.HP];
        public LaneType Lane { get; set; }
        
        public bool IsLeader => Card.IsLeader;
        public bool IsDead => Hp <= 0;

        public string CharacterName => Card.CharacterName;
        public string LivenessString => $"{(Hp >= 0 ? Hp : 0)}/{this[Stat.MHP]}";
        public string LivenessChar => IsDead
            ? IsLeader ? "X" : "x" 
            : IsLeader ? "0" : "o";
        public string CompositionString => $"{Card.CompositionString} ({Lane})";
        public string StateString => $"{{ {CompositionString}: {this[Stat.HP]}/{this[Stat.MHP]} " +
                                     $"{this[Stat.ATK]},{this[Stat.DEF]},{this[Stat.SPD]},{this[Stat.MP]} {Card.AbilString} }}";

        private readonly StatSet stats = new();
        public float this[Stat tag]
        {
            get => stats[tag];
            set => stats[tag] = value;
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

        public override string ToString() => StateString;

        #region Battle sim

        public void Reset()
        {
            foreach (var abil in Abilities)
            {
                abil.Reset();
            }
            
            stats.SetTo(Card.Stats);
            Lane = originalLane;
        }

        public void SimulateTurn(BattleModel battle)
        {
            if (IsDead)
            {
                return;
            }

            if (battle.UseVerboseLogging) battle.Log($"Next up: {StateString}");

            if (this[Stat.MP] > 0)
            {
                Party.GenerateMp((int)this[Stat.MP]);
                if (battle.UseVerboseLogging) battle.Log($"Party MP is now: {Party.Mp}");
            }
            
            foreach (var abil in Abilities)
            {
                abil.SimulateTurn(battle);
            }

            var opponent = battle.GetOppositeUnit(this);
            if (opponent != null)
            {
                AttackUnit(battle, opponent);
            }
            else
            {
                if (battle.UseVerboseLogging) battle.Log("No viable opponent.");
            }
            if (battle.UseVerboseLogging) battle.Log("");
        }

        public void AttackUnit(BattleModel battle, Unit opponent)
        {
            var dmg = (int)this[Stat.ATK] - opponent[Stat.DEF];
            if (battle.UseVerboseLogging) battle.Log($"Attacked {opponent.CompositionString} for {dmg} damage " +
                                                     $"({opponent[Stat.HP]} => {opponent[Stat.HP] - dmg})");
            
            opponent[Stat.HP] -= dmg;
            if (battle.UseVerboseLogging && opponent.IsDead) battle.Log("Knock out!!");
        }
        
        #endregion
    }
}