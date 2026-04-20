namespace RpgActorTGC
{
    public class NPCModel : EntrantModel
    {
        public sealed override string EntrantName { get; }
        
        public NPCModel()
        {
            EntrantName = ConstantsData.Instance.oppoNames.GeneratePlayerName();
            CurrentDeck = Deck.CreateRandom(EntrantName, CampaignManager.Instance.GloballyAvailableHeroes,
                CampaignManager.Instance.GloballyAvailableLeaders);
        }

        public override void SetupForNewRound()
        {
            base.SetupForNewRound();
        }
    }
}
