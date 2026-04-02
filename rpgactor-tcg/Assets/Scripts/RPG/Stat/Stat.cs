namespace RpgActorTGC
{
    public enum Stat
    {
        NONE  = 0,
        MHP   = 1,
        MP    = 2,
        HP    = 3,
        ATK   = 4,
        DEF   = 5,
        SPD   = 6,
    }
    
    public static class StatExtensions
    {
        public static StatInfo Info(this Stat tag) => DBManager.Instance.Get<StatInfo>(tag.ToString());
    }
}