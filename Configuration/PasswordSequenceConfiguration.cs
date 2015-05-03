using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    [XmlRoot(ElementName = "PasswordSequenceConfiguration")]
    public class PasswordSequenceConfiguration
    {
        public PasswordSequenceConfiguration()
        {
            XmlNamespace = new XmlSerializerNamespaces();
            XmlNamespace.Add(string.Empty, "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

            Sequence = new List<SequenceItem>();
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespace;

        public WordList DefaultWords { get; set; }
        public CharacterList DefaultCharacters { get; set; }
        [XmlArrayItem("SubstituteAny", typeof(AnySubstitution))]
        [XmlArrayItem("SubstituteWhole", typeof(WholeSubstitution))]
        public List<BaseSubstitution> DefaultSubstitutions { get; set; }

        [XmlArrayItem("Word", typeof(WordSequenceItem))]
        [XmlArrayItem("Characters", typeof(CharacterSequenceItem))]
        public List<SequenceItem> Sequence { get; set; }
    }
}
