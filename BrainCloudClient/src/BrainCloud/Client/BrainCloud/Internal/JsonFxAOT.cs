//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------


// Silly class to fool unity into NOT stripping
// conversion classes required by JsonFx

#if !DOT_NET
using System.ComponentModel;

namespace BrainCloud.Internal
{
    internal class JsonFxAOT
    {
        static JsonFxAOT m_aot = new JsonFxAOT();
        bool m_fakeFlag = false;

        private JsonFxAOT()
        {
            TypeConverter c;

            c = new ArrayConverter();
            m_fakeFlag = c.Equals(c);
            //c = new BaseNumberConverter();
            //m_fakeFlag = c.Equals(c);
            c = new BooleanConverter();
            m_fakeFlag = c.Equals(c);
            c = new ByteConverter();
            m_fakeFlag = c.Equals(c);
            c = new CollectionConverter();
            m_fakeFlag = c.Equals(c);
            c = new ComponentConverter(typeof(int));
            m_fakeFlag = c.Equals(c);
            c = new CultureInfoConverter();
            m_fakeFlag = c.Equals(c);
            c = new DateTimeConverter();
            m_fakeFlag = c.Equals(c);
            c = new DecimalConverter();
            m_fakeFlag = c.Equals(c);
            c = new DoubleConverter();
            m_fakeFlag = c.Equals(c);
            c = new EnumConverter(typeof(int));
            m_fakeFlag = c.Equals(c);
            c = new ExpandableObjectConverter();
            m_fakeFlag = c.Equals(c);
            c = new Int16Converter();
            m_fakeFlag = c.Equals(c);
            c = new Int32Converter();
            m_fakeFlag = c.Equals(c);
            c = new Int64Converter();
            m_fakeFlag = c.Equals(c);
            c = new NullableConverter(typeof(object));
            m_fakeFlag = c.Equals(c);
            c = new SByteConverter();
            m_fakeFlag = c.Equals(c);
            c = new SingleConverter();
            m_fakeFlag = c.Equals(c);
            c = new StringConverter();
            m_fakeFlag = c.Equals(c);
            c = new TimeSpanConverter();
            m_fakeFlag = c.Equals(c);
            c = new UInt16Converter();
            m_fakeFlag = c.Equals(c);
            c = new UInt32Converter();
            m_fakeFlag = c.Equals(c);
            c = new UInt64Converter();
            m_fakeFlag = c.Equals(c);
        }

        private bool GetFakeFlag()
        {
            return m_fakeFlag;
        }
        private JsonFxAOT GetAOT()
        {
            return m_aot;
        }

    }
}
#endif
