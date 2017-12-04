//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BrainCloud.Entity.Internal
{
// Similar to ListWrapper but for a Dictionary.
//
    internal class DictionaryWrapper<TValue, SValue> : IDictionary<string, TValue>
    {
        IDictionary<string, SValue> m_sourceDictionary;

        protected DictionaryWrapper()
        {
        }

        public DictionaryWrapper(IDictionary<string, SValue> sourceDictionary)
        {
            m_sourceDictionary = sourceDictionary;
        }

        // IDictionary<>
        public ICollection<string> Keys
        {
            get
            {
                return m_sourceDictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> list = new List<TValue>();
                foreach (SValue item in m_sourceDictionary.Values)
                {
                    list.Add(EntityUtil.GetObjectAsType<TValue>(item));
                }
                return list;
            }
        }

        public TValue this[string key]
        {
            get
            {
                return EntityUtil.GetObjectAsType<TValue>(m_sourceDictionary[key]);
            }
            set
            {
                m_sourceDictionary[key] = (SValue) (object) value;
            }
        }

        public void Add(string key, TValue value)
        {
            m_sourceDictionary.Add(key, (SValue) (object) value);
        }

        public bool ContainsKey(string key)
        {
            return m_sourceDictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return m_sourceDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        // ICollection<KeyValuePair<TKey, TValue>>
        public int Count
        {
            get
            {
                return m_sourceDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_sourceDictionary.IsReadOnly;
            }
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            KeyValuePair<string, SValue> kvp = new KeyValuePair<string, SValue>(item.Key, (SValue)(object)item.Value);
            m_sourceDictionary.Add(kvp);
        }

        public void Clear()
        {
            m_sourceDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            KeyValuePair<string, SValue> kvp = new KeyValuePair<string, SValue>(item.Key, (SValue)(object)item.Value);
            return m_sourceDictionary.Contains(kvp);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            int arrLen = array.Length;
            KeyValuePair<string, SValue>[] src = new KeyValuePair<string, SValue>[arrLen];
            m_sourceDictionary.CopyTo(src, arrayIndex);
            for (int i = 0; i < arrLen; ++i)
            {
                array[i] = new KeyValuePair<string, TValue>(src[i].Key, EntityUtil.GetObjectAsType<TValue>(src[i].Value));
            }
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            KeyValuePair<string, SValue> kvp = new KeyValuePair<string, SValue>(item.Key, (SValue)(object)item.Value);
            return m_sourceDictionary.Remove(kvp);
        }

        // IEnumerable<KeyValuePair<TKey, TValue>>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return new DictionaryWrapperEnumerator<TValue, SValue>(m_sourceDictionary.GetEnumerator());
        }

        // IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DictionaryWrapperEnumerator<TValue, SValue>(m_sourceDictionary.GetEnumerator());
        }
    }
}
