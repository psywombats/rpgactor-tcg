using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEditor;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Database", menuName = "Data/ScriptableDB")]
    public class ScriptableDB : ScriptableObject
    {
        [Serializable]
        private struct ScriptSet
        {
            public List<ScriptableObject> scriptables;
        }
        
        [SerializeField] private List<string> orderedTypeNames = new();
        [SerializeField] private List<ScriptSet> orderedScriptSets = new();

        private readonly Dictionary<string, List<ScriptableObject>> scriptablesByTypeName = new();
        private readonly Dictionary<string, Dictionary<string, ScriptableObject>> indexedScriptablesByType = new();
        
        public T Get<T>(string key) where T : ScriptableObject, IDatabaseKeyable
        {
            if (!scriptablesByTypeName.Any())
            {
                for (var i = 0; i < orderedTypeNames.Count; i += 1)
                {
                    scriptablesByTypeName.Add(orderedTypeNames[i], orderedScriptSets[i].scriptables);
                }
            }
            if (!indexedScriptablesByType.TryGetValue(typeof(T).Name, out var scriptablesByKey))
            {
                scriptablesByKey = new Dictionary<string, ScriptableObject>();
                var typeName = typeof(T).Name;
                indexedScriptablesByType.Add(typeName, scriptablesByKey);

                foreach (var scriptable in scriptablesByTypeName[typeName])
                {
                    var instance = (T)scriptable;
                    scriptablesByKey.Add(instance.Key, instance);
                }
            }
            return scriptablesByKey[key] as T;
        }
        
#if UNITY_EDITOR
        [Button]
        public void Repopulate()
        {
            var scriptablesByType = new Dictionary<Type, List<ScriptableObject>>();
            
            var guids = AssetDatabase.FindAssets("t:ScriptableObject"); 
    
            // load all scriptable objects
            foreach (var guid in guids) 
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                var type = asset.GetType();
                if (!scriptablesByType.TryGetValue(type, out var scriptableList))
                {
                    scriptableList = new List<ScriptableObject>();
                    scriptablesByType.Add(type, scriptableList);
                }
                scriptableList.Add(asset);
            }
            
            // iterate over each type and check for attribute
            var result = new Dictionary<string, List<ScriptableObject>>();
            foreach (var scriptableType in scriptablesByType.Keys)
            {
                var attr = (DatabaseIndexedAttribute) Attribute.GetCustomAttribute(scriptableType, typeof(DatabaseIndexedAttribute));
                if (attr == null)
                {
                    continue;
                }

                var key = scriptableType.Name;
                foreach (var instance in scriptablesByType[scriptableType])
                {
                    if (!result.TryGetValue(key, out var resultList))
                    {
                        resultList = new List<ScriptableObject>();
                        result.Add(key, resultList);
                    }
                    resultList.Add(instance);
                }
            }

            scriptablesByTypeName.Clear();
            orderedTypeNames = result.Keys.ToList();
            orderedScriptSets = result.Values.Select(r => new ScriptSet
            {
                scriptables = r.ToList() 
                
            }).ToList();
            
            EditorUtility.SetDirty(this);
        }
#endif
    }
}