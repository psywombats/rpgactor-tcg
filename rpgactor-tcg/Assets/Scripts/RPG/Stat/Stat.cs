namespace RpgActorTGC
{
    public enum Stat
    {
        NONE,
        MHP,
        HP,
        ATK,
        DEF,
        SPD,
    }
    
    public static class StatExtensions
    {
        public static StatInfo Info(this Stat tag) => DBManager.Instance.Get<StatInfo>(tag.ToString());
    }
}