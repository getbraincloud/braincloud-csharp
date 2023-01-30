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
    internal class DictionaryWrapperEnumerator<TValue, SValue> : IEnumerator<KeyValuePair<string, TValue>>
    {
        IEnumerator<KeyValuePair<string, SValue>> m_sourceEnumerator;

        public DictionaryWrapperEnumerator(IEnumerator<KeyValuePair<string, SValue>> sourceEnumerator)
        {
            m_sourceEnumerator = sourceEnumerator;
        }

        // IEnumerator<KeyValuePair<string, TValue>>
        public KeyValuePair<string, TValue> Current
        {
            get
            {
                KeyValuePair<string, SValue> srcKvp = m_sourceEnumerator.Current;
                return new KeyValuePair<string, TValue>(srcKvp.Key, EntityUtil.GetObjectAsType<TValue>(srcKvp.Value));
            }
        }

        // IDisposable
        public void Dispose()
        {
            m_sourceEnumerator.Dispose();
        }

        // IEnumerator
        object System.Collections.IEnumerator.Current
        {
            get
            {
                return m_sourceEnumerator.Current;
            }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return m_sourceEnumerator.MoveNext();
        }

        void System.Collections.IEnumerator.Reset()
        {
            m_sourceEnumerator.Reset();
        }

    }
}
