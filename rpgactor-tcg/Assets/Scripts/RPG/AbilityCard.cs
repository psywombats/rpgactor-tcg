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
        
        public string GetShortDescription(CharacterCard owner) => $"{Data.mpCost}: {Data.GetAbilityName(owner.Data)} {Power}";
        public string GetLongDescription(CharacterCard owner) => $"{Data.mpCost}: {string.Format(Data.GetAbilityDesc(owner.Data), Power)}";
    }
}