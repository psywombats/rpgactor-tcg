using System.Collections.Generic;

namespace RpgActorTGC
{
    public class Unit
    {
        public CharacterCard Card { get; }
        private readonly LaneType originalLane;

        public Party Party { get; }
        public List<AbilityInstance> Abilities { get; } = new();
        public int Hp { get; private set; }
        public LaneType Lane { get; set; }
        
        public bool IsLeader => Card.IsLeader;
        public bool IsDead => Hp <= 0;


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
        
        #region Battle sim

        public void Reset()
        {
            Hp = (int)Card[Stat.MHP];
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
            
            Party.GenerateMp((int)this[Stat.MP]);
            
            foreach (var abil in Abilities)
            {
                abil.SimulateTurn(battle);
            }

            var opponent = battle.GetOppositeUnit(this);
            if (opponent != null)
            {
                AttackUnit(opponent);
            }
        }

        public void AttackUnit(Unit opponent)
        {
            opponent.Hp -= (int)this[Stat.ATK];
        }
        
        #endregion
    }
}