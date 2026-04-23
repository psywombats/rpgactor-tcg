using System.Threading.Tasks;
using Effekseer;

namespace RpgActorTGC
{
    public class AbilityInstance
    {
        public AbilityCard Card { get; }
        public Unit Owner { get; }
        public Party Party => Owner.Party;
        
        public bool HasActivated { get; private set; }
        
        public Element Element => Owner.Element;
        public EffekseerEffectAsset Anim => Card.Anim;
        
        public string GetShortDescription(bool pretty = false) => Card.GetShortDescription(Owner.Card, pretty);
        public string GetLongDescription(bool pretty = false) => Card.GetLongDescription(Owner.Card, pretty);
        
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
                    battle.View.ViewForUnit[Owner].AnimateCastAsync(this).Forget();
                    await battle.View.WriteLineAsync($"{Owner.PrettyName} activated {GetShortDescription()}: ");
                }
                await Card.Data.ActivateAsync(battle, Owner, this);
                if (!battle.IsSim)
                {
                    await battle.View.WriteLineAsync(Card.Data.GetUseMessage(battle, Owner, this), true);
                }
            }
        }

        public Task AnimateOnTargetAsync(BattleModel battle, Unit target)
        {
            if (!battle.IsSim && Card.Anim != null)
            {
                return battle.View.ViewForUnit[target].PlayAnimAsync(Anim);
            }
            return Task.CompletedTask;
        }
    }
}