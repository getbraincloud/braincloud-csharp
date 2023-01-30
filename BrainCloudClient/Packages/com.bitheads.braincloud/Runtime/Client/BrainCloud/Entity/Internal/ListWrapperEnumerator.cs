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
    internal class ListWrapperEnumerator<T, S> : IEnumerator<T>
    {
        IEnumerator<S> m_sourceEnumerator;

        public ListWrapperEnumerator(IEnumerator<S> sourceEnumerator)
        {
            m_sourceEnumerator = sourceEnumerator;
        }

        // IEnumerator<T>
        public T Current
        {
            get
            {
                return EntityUtil.GetObjectAsType<T>(m_sourceEnumerator.Current);
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
