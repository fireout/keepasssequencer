using System.Xml.Serialization;

namespace Sequencer.Configuration
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
