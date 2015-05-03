using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    public enum StrengthEnum : byte
    {
        [XmlEnum("full")]
        Full = 100
    }
}
