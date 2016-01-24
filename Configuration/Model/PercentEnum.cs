using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    public enum PercentEnum : byte
    {
        [XmlEnum(Name = "never")]
        Never = 0,
        [XmlEnum(Name = "always")]
        Always = 100
    }
}
