using Effekseer;
using JetBrains.Annotations;

namespace RpgActorTGC
{
    public class AbilityCard
    {
        public AbilityData Data { get; }

        public int Power => Data.power;
        public int Cost => Data.mpCost;
        public bool IsContinuous => Data.IsContinuous;
        public EffekseerEffectAsset Anim => Data.anim;
        
        public AbilityCard(AbilityData data)
        {
            Data = data;
        }

        public string GetShortDescription([CanBeNull] CharacterCard owner, bool pretty = false)
        {
            var desc = "";
            if (Data.mpCost > 0)
            {
                desc += Data.mpCost;
                desc += pretty ? "<sprite name=\"mp\">" : " ";
                desc += Data.IsContinuous ? "+" : ":";
            }
            desc += Data.GetAbilityName(owner?.Data);
            if (Data.HasPower) desc +=  " " + Data.power;
            return desc;
        }

        public string GetLongDescription([CanBeNull] CharacterCard owner, bool pretty = false)
        {
            var desc = string.Format(Data.GetAbilityDesc(owner?.Data), Power);
            if (Data.mpCost > 0)
                desc += $"This ability activates {(!Data.IsContinuous ? "once" : "every turn")} the " +
                        $"party has {Data.mpCost} MP.";
            return desc;
        }

        public override string ToString() => GetShortDescription(null);
    }
}