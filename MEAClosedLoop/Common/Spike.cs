using System;
using System.Runtime.Serialization;

namespace Common
{
//    [DataContract]
    public class Spike
    {
        UInt64 m_timestamp; // or DateTime?
        UInt64 m_meaBits;

        public Spike(UInt64 _timestamp, UInt64 _meaBits)
        {
            m_timestamp = _timestamp;
            m_meaBits = _meaBits;
        }

//        [DataMember]
        public UInt64 timestamp
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

//        [DataMember]
        public UInt64 meaBits
        {
            get { return m_meaBits; }
            set { m_meaBits = value; }
        }
    }
}
