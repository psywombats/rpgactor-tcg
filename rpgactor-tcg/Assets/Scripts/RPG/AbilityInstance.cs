using System.Threading.Tasks;

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

        public async Task SimulateTurnAsync(BattleModel battle)
        {
            if ((!HasActivated || Card.IsContinuous) && Party.MP >= Card.Cost)
            {
                if (battle.UseVerboseLogging) battle.SimLogPartial($"{Owner.PrettyName} activated {GetShortDescription()}: ");
                HasActivated = true;
                if (!battle.IsSim)
                {
                    await battle.View.WriteLineAsync($"Activated {GetShortDescription()}: ");
                }
                Card.Data.Activate(battle, Owner, this);
                if (!battle.IsSim)
                {
                    AnimateAsync().Forget();
                    await battle.View.WriteLineAsync(Card.Data.GetUseMessage(battle, Owner, this));
                }
            }
        }

        private Task AnimateAsync()
        {
            // TODO: animateasync
            return Task.CompletedTask;
        }
    }
}