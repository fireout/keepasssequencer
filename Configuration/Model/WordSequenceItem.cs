using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    public class WordSequenceItem : SequenceItem
    {
        public WordSequenceItem()
        {
            Capitalize = CapitalizeEnum.Proper;
            Substitution = PercentEnum.Always;
            Words = new OverridingWordList();
        }

        public override double entropy(PasswordSequenceConfiguration config)
        {
            double entropyVal = 0;

            if (Words.Count > 0)
            {
                entropyVal += Math.Log(Words.Count, 2);
            }
            if (config.DefaultWords.Count > 0 && !Words.Override)
            {
                entropyVal += Math.Log(config.DefaultWords.Count, 2);
            }
            /* TODO: other properties */

            return entropyVal;
        }

        public OverridingWordList Words { get; set; }

        //[XmlArrayItem("SubstituteAny", typeof(AnySubstitution))]
        //[XmlArrayItem("SubstituteWhole", typeof(WholeSubstitution))]
        public SubstitutionList Substitutions { get; set; }

        [XmlIgnore]
        private CapitalizeEnum _myCapChance;
        [XmlIgnore]
        public CapitalizeEnum Capitalize
        {
            get
            {
                return _myCapChance;
            }
            set
            {
                if (value > CapitalizeEnum.Always && value != CapitalizeEnum.Proper)
                    _myCapChance = CapitalizeEnum.Always;
                else if (value < CapitalizeEnum.Never)
                    _myCapChance = CapitalizeEnum.Never;
                else
                    _myCapChance = value;
            }
        }

        [XmlAttribute("capitalize")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlCapitalize
        {
            get { return Capitalize.ToString().ToLower(); }
            set { Capitalize = (CapitalizeEnum)Enum.Parse(typeof(CapitalizeEnum), value, true); }
        }

        [XmlIgnore]
        private PercentEnum _mySubstPercent;
        [XmlIgnore]
        public PercentEnum Substitution {
            get { return _mySubstPercent; }
            set
            {
                if (value > PercentEnum.Always)
                    _mySubstPercent = PercentEnum.Always;
                else if (value < PercentEnum.Never)
                    _mySubstPercent = PercentEnum.Never;
                else
                    _mySubstPercent = value;
            }
        }

        [XmlAttribute("substitution")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlSubstitution
        {
            get { return Substitution.ToString().ToLower(); }
            set { Substitution = (PercentEnum)Enum.Parse(typeof(PercentEnum), value, true); }
        }
    }
}
