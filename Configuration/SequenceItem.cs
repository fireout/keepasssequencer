using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sequencer.Configuration
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
        public PercentEnum Probability { get; set; }

        [XmlAttribute("probability")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlProbability
        {
            get { return Probability.ToString().ToLower(); }
            set { Probability = (PercentEnum)Enum.Parse(typeof(PercentEnum), value, true); }
        }
    }
}
