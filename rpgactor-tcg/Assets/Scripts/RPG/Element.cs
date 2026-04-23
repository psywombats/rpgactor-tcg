namespace RpgActorTGC
{
    public enum Element
    {
        None,
        Fire,
        Wind,
        Thunder,
        Earth,
        Ice,
        Water
    }
    
    public static class ElementExtensions
    {
        public static ElementInfo Info(this Element tag)
        {
            return DBManager.Instance.Get<ElementInfo>(tag.ToString());
        }
    }
}