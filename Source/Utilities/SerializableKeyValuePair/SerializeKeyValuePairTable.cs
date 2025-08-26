using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity_MTool
{
    [Serializable]
    public class SerializeKeyPairTable<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Avoid frequent GC check
        [Serializable]
        private struct Element
        {
            [SerializeField]
            internal SerializeKeyPair<TKey,TValue> Pair;
        }
        [SerializeField]
        private Element[] _serializableDictionary;
        public virtual TKey DefaultKey => default;
        public virtual TValue DefaultValue => default;

        public virtual void OnBeforeSerialize()
        {
            _serializableDictionary = new Element[Count];
            for(int i = 0 ; i < Count ; ++i)
            {
                var kvpair = this.ElementAt(i);
                _serializableDictionary[i].Pair = new SerializeKeyPair<TKey, TValue>(kvpair.Key,kvpair.Value);
            }
        }
        public virtual void OnAfterDeserialize()
        {
            Clear();
            foreach(var element in _serializableDictionary)
            {
                this[ContainsKey(element.Pair.Key) ? DefaultKey : element.Pair.Key] = element.Pair.Value;
            }
        }

        public void ClearAllKVPair()
        {
            Clear();
            _serializableDictionary = null;
        }
    }
}