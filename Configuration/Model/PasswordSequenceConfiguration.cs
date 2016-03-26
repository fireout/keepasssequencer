using Sequencer.Configuration.Model;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    [XmlRoot(ElementName = "PasswordSequenceConfiguration")]
    public class PasswordSequenceConfiguration
    {
        /* this constructor is for use with the serializer, it doesn't fill in
         * any of the data members that come from the XML config file
         */
        public PasswordSequenceConfiguration()
        {
            XmlNamespace = new XmlSerializerNamespaces();
            XmlNamespace.Add(string.Empty, "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

            DefaultWords = new WordList();
            DefaultCharacters = new CharacterList();
            DefaultSubstitutions = new List<BaseSubstitution>();
            Sequence = new List<SequenceItem>();
        }

        /* this constructor is for creating an empty configuration if reading
         * the config file fails somehow; the argument is unused but I don't
         * know a better way to do it
         */
        public PasswordSequenceConfiguration(bool throwaway)
            : this()
        {
            if (throwaway)
            {
                DefaultWords.AddRange(new[] { "Replace", "those", "words", "with", "your", "own", "selection" });
                DefaultCharacters.AddRange("1234567890!\"$%?&*()_+");
                DefaultSubstitutions.Add(new AnySubstitution()
                {
                    Replace = "bcdfghjklmnpqrstvwxz",
                    With = "~",
                    CaseSensitive = true
                });
                Sequence.Add(new WordSequenceItem());
                Sequence.Add(new CharacterSequenceItem());
            }
        }

        public string Name { get; set; }

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
