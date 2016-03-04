using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    [XmlInclude(typeof(WordSequenceItem))]
    [XmlInclude(typeof(CharacterSequenceItem))]
    public abstract class SequenceItem
    {
        protected SequenceItem()
        {
            Probability = (PercentEnum)100;
        }

        [XmlIgnore]
        private PercentEnum _myProb;
        [XmlIgnore]
        public PercentEnum Probability
        {
            get { return _myProb; }
            set
            {
                if (value > PercentEnum.Always)
                    _myProb = PercentEnum.Always;
                else if (value < PercentEnum.Never)
                    _myProb = PercentEnum.Never;
                else
                    _myProb = value;
            }
        }

        public abstract double entropy(PasswordSequenceConfiguration config);

        [XmlAttribute("probability")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlProbability
        {
            get { return Probability.ToString().ToLower(); }
            set { Probability = (PercentEnum)Enum.Parse(typeof(PercentEnum), value, true); }
        }
    }
}
