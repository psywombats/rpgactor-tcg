using UnityEngine;

namespace RpgActorTGC
{
    public class DBManager : SingletonBehaviour<DBManager>
    {
        [SerializeField] private ScriptableDB database;
        
        public T Get<T>(string key) where T : ScriptableObject, IDatabaseKeyable => database.Get<T>(key);

        public void Awake()
        {
            if (database == null)
            {
                database = Resources.Load<ScriptableDB>("Database");
            }
        }
    }
}