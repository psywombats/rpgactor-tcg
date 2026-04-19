using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace RpgActorTGC
{
     [Serializable]
    public class StatSet : ISerializationCallbackReceiver 
    {
        [SerializeField] private StatDictionary serializedStats;
        private readonly Dictionary<Stat, float> stats = new();

        public StatSet() {}
        
        public StatSet(StatSet other) : this(other.serializedStats) {}

        public void SetTo(StatSet other) => SetFromSerializedStats(other.serializedStats);
        
        private StatSet(StatDictionary stats) : this()
        {
            SetFromSerializedStats(stats);
        }

        private void SetFromSerializedStats(StatDictionary stats)
        {
            var statStrings = stats.ToDictionary();
            foreach (var stat in statStrings) 
            {
                if (Enum.TryParse(stat.Key, true, out Stat result)) 
                {
                    this[result] = stat.Value;
                }
            }
        }

        #region Accessors

        public float Get(Stat tag)
        {
            return tag == Stat.NONE 
                ? 0.0f 
                : stats.GetValueOrDefault(tag, 0f);
        }

        public bool Is(Stat tag) 
        {
            return Get(tag) > 0.0f;
        }

        public void Set(Stat tag, float value) 
        {
            stats[tag] = value;
        }

        public float this[Stat tag] 
        {
            get => Get(tag);
            set => Set(tag, value);
        }

        public IEnumerable<Tuple<Stat, float>> ToTuples() =>
            Enum.GetValues(typeof(Stat)).Cast<Stat>().Select(stat => new Tuple<Stat, float>(stat, Get(stat)));

        #endregion
        
        #region Operations

        public void Add(Stat tag, float value) 
        {
            this[tag] += value;
        }

        public void Sub(Stat tag, float value) 
        {
            Add(tag, -value);
        }

        public static StatSet operator +(StatSet a, StatSet b) => a.AddSet(b);
        public StatSet AddSet(StatSet other) 
        {
            foreach (var tag in other.stats.Keys)
            {
                this[tag] += other[tag];
            }
            return this;
        }

        public static StatSet operator -(StatSet a, StatSet b) => a.RemoveSet(b);
        public StatSet RemoveSet(StatSet other) 
        {
            foreach (var tag in other.stats.Keys) 
            {
                this[tag] -= other[tag];
            }
            return this;
        }

        #endregion
        
        #region Serialization

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context) 
        {
            WriteSerializedStats();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ReadFromSerializedStats();
        }

        private void WriteSerializedStats() 
        {
            if (stats.Count == 0) return;
            var statStrings = new Dictionary<string, float>();
            foreach (var stat in stats) 
            {
                if (stat.Value != 0f) 
                {
                    statStrings[stat.Key.ToString()] = stat.Value;
                }
            }
            serializedStats = new StatDictionary(statStrings);
        }

        private void ReadFromSerializedStats() 
        {
            var statStrings = serializedStats.ToDictionary();
            foreach (var stat in statStrings) 
            {
                if (Enum.TryParse(stat.Key, true, out Stat result)) 
                {
                    this[result] = stat.Value;
                }
            }
        }

        public void OnBeforeSerialize() 
        {
            WriteSerializedStats();
        }

        public void OnAfterDeserialize() 
        {
            ReadFromSerializedStats();
        }

        [Serializable]
        public class StatDictionary : SerialDictionary<string, float> 
        {
            public StatDictionary(Dictionary<string, float> dictionary) : base(dictionary) {}
        }

        #endregion

        public bool IsEmpty() => stats.All(pair => pair.Value == 0.0f);

        public string GetOneLiner() 
        {
            var result = "";
            foreach (Stat tag in Enum.GetValues(typeof(Stat))) 
            {
                if (this[tag] != 0)
                {
                    if (result.Length > 0) {
                        result += ", ";
                    }
                    result += tag + ": " + (int)this[tag];
                }
            }
            return result;
        }

        public override string ToString() => GetOneLiner();
    }   
}
