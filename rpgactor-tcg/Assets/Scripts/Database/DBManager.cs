using System.Collections.Generic;
using UnityEngine;

public class DBManager : SingletonBehaviour<DBManager>
{
    [SerializeField] private ScriptableDB database;
    [Space]
    [SerializeField] private ConstantsData constantsData;
    
    public T Get<T>(string key) where T : ScriptableObject, IDatabaseKeyable => database.Get<T>(key, nullAllowed: false);
    public T GetOrNull<T>(string key) where T : ScriptableObject, IDatabaseKeyable => database.Get<T>(key);
    public T GetRandom<T>() where T : ScriptableObject, IDatabaseKeyable => database.GetRandom<T>();
    public IEnumerable<T> GetAll<T>() where T : ScriptableObject, IDatabaseKeyable => database.GetAll<T>();
    
    public ConstantsData Constants => constantsData;

    protected override void Init()
    {
        if (database == null)
        {
            database = Resources.Load<ScriptableDB>("Database");
            constantsData = Resources.Load<ConstantsData>("Constants");
        }
    }
}
