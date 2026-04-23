using UnityEditor;
using Application = UnityEngine.Application;

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
        
        ARCHER = 7,
        DUALSTRIKE = 8,
        BLOCK = 9,
    }
    
    public static class StatExtensions
    {
        public static StatInfo Info(this Stat tag)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var stat = AssetDatabase.LoadAssetAtPath<StatInfo>("Assets/Resources/Database/Stats/Stat" + tag + ".asset");
                return stat ?? AssetDatabase.LoadAssetAtPath<StatInfo>("Assets/Resources/Database/Stats/Flag" + tag + ".asset");
            }
#endif
            return DBManager.Instance.Get<StatInfo>(tag.ToString());
        }
    }
}