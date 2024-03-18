using System.Collections.Generic;
using UnityEngine;

namespace PVM
{
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys;
        [SerializeField] private List<TValue> _values;
        private Dictionary<TKey, TValue> _dict;
        public Dictionary<TKey, TValue> Dict => _dict;

        public SerializableDictionary()
        {
            _dict = new Dictionary<TKey, TValue>();
        }


        public void OnBeforeSerialize()
        {
            Debug.Log("------------------OnBeforeSerialize");
            _keys = new List<TKey>(_dict.Keys);
            _values = new List<TValue>(_dict.Values);
        }

        public void OnAfterDeserialize()
        {
            Debug.Log("------------------OnAfterDeserialize");
            _dict.Clear();
            if (_keys == null || _values == null) return;
            var count = Mathf.Min(_keys.Count, _values.Count);
            for (var i = 0; i < count; ++i)
            {
                _dict.Add(_keys[i], _values[i]);
            }
        }
    }
}