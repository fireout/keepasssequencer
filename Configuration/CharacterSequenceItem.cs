using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    [XmlRoot(ElementName = "Characters")]
    public class CharacterSequenceItem : SequenceItem
    {
        public CharacterSequenceItem()
        {
            Length = 1;
            LengthStrength = StrengthEnum.Full;
            AllowDuplicate = true;
        }

        public OverridingCharacterList Characters { get; set; }

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
