using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    public enum StrengthEnum : byte
    {
        [XmlEnum("full")]
        Full = 100
    }
}
