using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    [XmlRoot(ElementName = "Characters")]
    public class CharacterSequenceItem : SequenceItem
    {
        public CharacterSequenceItem()
        {
            Length = 1;
            LengthStrength = StrengthEnum.Full;
            AllowDuplicate = true;
            Characters = new OverridingCharacterList();
        }

        public OverridingCharacterList Characters { get; set; }

        public override double entropy(PasswordSequenceConfiguration config)
        {
            double entropyVal = 0;

            if (Length > 0)
            {
                if (Characters.Count > 0)
                {
                    entropyVal += Math.Log(Characters.Count, 2);
                }
                if (config.DefaultCharacters.Count > 0 && !Characters.Override)
                {
                    entropyVal += Math.Log(config.DefaultCharacters.Count, 2);
                }
                entropyVal *= Length;
            }
            /* TODO: other properties */

            return entropyVal;
        }

        [XmlAttribute("allowDuplicate")]
        public bool AllowDuplicate { get; set; }

        [XmlAttribute("length")]
        public byte Length { get; set; }

        [XmlIgnore]
        public StrengthEnum LengthStrength { get; set; }

        [XmlAttribute("lengthStrength")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlLengthStrength
        {
            get { return LengthStrength.ToString().ToLower(); }
            set { LengthStrength = (StrengthEnum)Enum.Parse(typeof(StrengthEnum), value, true); }
        }

    }
}
