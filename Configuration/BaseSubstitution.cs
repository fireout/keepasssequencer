using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    public abstract class BaseSubstitution
    {
        [XmlAttribute("replace")]
        public string Replace { get; set; }
        [XmlAttribute("with")]
        public string With { get; set; }

        [XmlAttribute("caseSensitive")]
        public bool CaseSensitive { get; set; }
    }
}
