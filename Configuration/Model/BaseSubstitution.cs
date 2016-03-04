using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    public abstract class BaseSubstitution
    {
        protected BaseSubstitution() { Replace = ""; With = ""; CaseSensitive = false; }

        [XmlAttribute("replace")]
        public string Replace { get; set; }
        [XmlAttribute("with")]
        public string With { get; set; }

        [XmlAttribute("caseSensitive")]
        public bool CaseSensitive { get; set; }
    }
}
