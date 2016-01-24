using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    public enum StrengthEnum : byte
    {
        [XmlEnum("full")]
        Full = 100
    }
}
