using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    public enum CapitalizeEnum : byte
    {
        [XmlEnum("never")]
        Never = 0,
        [XmlEnum("always")]
        Always = 100,
        [XmlEnum("proper")]
        Proper = 101
    }
}
