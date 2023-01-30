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
// T is type user expects
// S is underlying list type
// so for ListWrapper<int, object>...
// the underyling list is a list of type object (S) and
// all operations on the wrapper should return list items of type int (T)
    internal class ListWrapper<T, S> : IList<T>
    {
        IList<S> m_sourceList;

        protected ListWrapper()
        {
        }

        public ListWrapper(IList<S> sourceList)
        {
            m_sourceList = sourceList;
        }

        // IList interface
        public T this[int index]
        {
            get
            {
                return EntityUtil.GetObjectAsType<T>(m_sourceList[index]);
            }
            set
            {
                m_sourceList[index] = (S) (object) value;
            }
        }

        public int IndexOf(T item)
        {
            return m_sourceList.IndexOf((S)(object)item);
        }

        public void Insert(int index, T item)
        {
            m_sourceList.Insert(index, (S)(object)item);
        }

        public void RemoveAt(int index)
        {
            m_sourceList.RemoveAt(index);
        }

        // ICollection interface
        public int Count
        {
            get
            {
                return m_sourceList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_sourceList.IsReadOnly;
            }
        }

        public void Add(T item)
        {
            m_sourceList.Add((S)(object)item);
        }

        public void Clear()
        {
            m_sourceList.Clear();
        }

        public bool Contains(T item)
        {
            return m_sourceList.Contains((S)(object)item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int arrLen = array.Length;
            S[] src = new S[arrLen];
            m_sourceList.CopyTo(src, arrayIndex);
            for (int i = 0; i < arrLen; ++i)
            {
                array[i] = EntityUtil.GetObjectAsType<T>(src[i]);
            }
        }

        public bool Remove(T item)
        {
            return m_sourceList.Remove((S)(object)item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListWrapperEnumerator<T,S>(m_sourceList.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListWrapperEnumerator<T, S>(m_sourceList.GetEnumerator());
        }

    }
}
