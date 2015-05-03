using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    public class WordSequenceItem : SequenceItem
    {
        public WordSequenceItem()
        {
            Capitalize = CapitalizeEnum.Proper;
            Substitution = PercentEnum.Always;
        }

        public OverridingWordList Words { get; set; }

        //[XmlArrayItem("SubstituteAny", typeof(AnySubstitution))]
        //[XmlArrayItem("SubstituteWhole", typeof(WholeSubstitution))]
        public SubstitutionList Substitutions { get; set; }

        [XmlIgnore]
        public CapitalizeEnum Capitalize { get; set; }

        [XmlAttribute("capitalize")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlCapitalize
        {
            get { return Capitalize.ToString().ToLower(); }
            set { Capitalize = (CapitalizeEnum)Enum.Parse(typeof(CapitalizeEnum), value, true); }
        }

        [XmlIgnore]
        public PercentEnum Substitution { get; set; }

        [XmlAttribute("substitution")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlSubstitution
        {
            get { return Substitution.ToString().ToLower(); }
            set { Substitution = (PercentEnum)Enum.Parse(typeof(PercentEnum), value, true); }
        }

    }
}
