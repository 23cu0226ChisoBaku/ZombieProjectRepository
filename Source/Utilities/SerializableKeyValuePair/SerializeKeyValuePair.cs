using System;

namespace Unity_MTool
{
    [System.Serializable]
    internal class SerializeKeyPair<TKey,TValue> where TKey : notnull
    {
        [UnityEngine.SerializeField]
        private TKey _key;
        [UnityEngine.SerializeField]
        private TValue _value;

        public TKey Key => _key;
        public TValue Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        public SerializeKeyPair(TKey key,TValue value)
        {
            _key = key;
            _value = value;
        }
    }
}