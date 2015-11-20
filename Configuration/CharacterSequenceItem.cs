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
                uint len = Length;
                uint charlist_len = 0;
                if (Characters != null)
                {
                    charlist_len = (uint)Characters.Count;
                }
                if (Characters == null || !Characters.Override)
                {
                    charlist_len += (uint)config.DefaultCharacters.Count;
                }

                if (!AllowDuplicate)
                {
                    len = (len <= charlist_len ? len : charlist_len);
                }

                double cur_len_entropy = 0.0;
                uint cur_len = len;
                while (cur_len > 0 && charlist_len > 0)
                {
                    cur_len_entropy += Math.Log(charlist_len, 2);

                    if (LengthStrength < StrengthEnum.Full)
                    {
                        if (cur_len > 1)
                        {
                            entropyVal += (1 - (double)LengthStrength / 100) * cur_len_entropy / len;
                        }
                    }

                    if (!AllowDuplicate)
                    {
                        charlist_len -= 1;
                    }
                    cur_len -= 1;
                }
                entropyVal +=  ((double)LengthStrength / 100) * cur_len_entropy;
            }

            return entropyVal;
        }

        [XmlAttribute("allowDuplicate")]
        public bool AllowDuplicate { get; set; }

        [XmlAttribute("length")]
        public uint Length { get; set; }

        [XmlIgnore]
        private StrengthEnum _myLenStren;
        public StrengthEnum LengthStrength
        {
            get { return _myLenStren; }
            set
            {
                if (value > StrengthEnum.Full)
                    _myLenStren = StrengthEnum.Full;
                else if (value < 0)
                    _myLenStren = 0;
                else
                    _myLenStren = value;
            }
        }

        [XmlAttribute("lengthStrength")]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string XmlLengthStrength
        {
            get { return LengthStrength.ToString().ToLower(); }
            set { LengthStrength = (StrengthEnum)Enum.Parse(typeof(StrengthEnum), value, true); }
        }
    }
}
