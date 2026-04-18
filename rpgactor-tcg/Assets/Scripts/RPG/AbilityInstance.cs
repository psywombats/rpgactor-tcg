namespace RpgActorTGC
{
    public class AbilityInstance
    {
        public AbilityCard Card { get; }
        public Unit Owner { get; }
        public Party Party => Owner.Party;
        
        public bool HasActivated { get; private set; }
        
        public string GetShortDescription(bool pretty = false) => Card.GetShortDescription(Owner.Card, pretty);
        public string GetLongDescription(bool pretty = false) => Card.GetShortDescription(Owner.Card, pretty);
        
        public AbilityInstance(Unit owner, AbilityCard card)
        {
            Card = card;
            Owner = owner;
        }

        public void Reset()
        {
            HasActivated = false;
        }

        public void SimulateTurn(BattleModel battle)
        {
            if ((!HasActivated || Card.IsContinuous) && Party.Mp >= Card.Cost)
            {
                if (battle.UseVerboseLogging) battle.LogPartial($"Activated {GetShortDescription()}: ");
                HasActivated = true;
                Card.Data.Activate(battle, Owner, this);
            }
        }
    }
}