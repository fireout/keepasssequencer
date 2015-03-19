using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    public enum PercentEnum : byte
    {
        [XmlEnum(Name = "never")]
        Never = 0,
        [XmlEnum(Name = "always")]
        Always = 100
    }
}
