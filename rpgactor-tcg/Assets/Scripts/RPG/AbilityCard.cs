using JetBrains.Annotations;

namespace RpgActorTGC
{
    public class AbilityCard
    {
        public AbilityData Data { get; }

        public int Power => Data.power;
        public int Cost => Data.mpCost;
        public bool IsContinuous => Data.IsContinuous;
        
        public AbilityCard(AbilityData data)
        {
            Data = data;
        }
        
        public string GetShortDescription([CanBeNull] CharacterCard owner, bool pretty = false) 
            => $"{Data.mpCost}{(pretty ? "<sprite name=\"mp\"" : "")}: {Data.GetAbilityName(owner?.Data)} {Power}";
        public string GetLongDescription([CanBeNull] CharacterCard owner, bool pretty = false) 
            => $"{Data.mpCost}{(pretty ? "<sprite name=\"mp\"" : "")}: {string.Format(Data.GetAbilityDesc(owner?.Data), Power)}";

        public override string ToString() => GetShortDescription(null);
    }
}