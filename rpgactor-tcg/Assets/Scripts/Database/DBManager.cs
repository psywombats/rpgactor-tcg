using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    public class DBManager : SingletonBehaviour<DBManager>
    {
        [SerializeField] private ScriptableDB database;
        
        public T Get<T>(string key) where T : ScriptableObject, IDatabaseKeyable => database.Get<T>(key);
        public T GetRandom<T>() where T : ScriptableObject, IDatabaseKeyable => database.GetRandom<T>();
        public IEnumerable<T> GetAll<T>() where T : ScriptableObject, IDatabaseKeyable => database.GetAll<T>();

        protected override void Awake()
        {
            base.Awake();
            if (database == null)
            {
                database = Resources.Load<ScriptableDB>("Database");
            }
        }
    }
}