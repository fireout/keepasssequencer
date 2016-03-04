using Sequencer.ItemVisitor;
using System;
using System.Collections.Generic;
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


            List<string> wordList = new List<string>();
            if (Words != null)
                wordList.AddRange(Words);
            if (Words == null || !Words.Override)
                wordList.AddRange(config.DefaultWords);

            List<BaseSubstitution> applicableSubstitution = new List<BaseSubstitution>();
            if (Substitution > PercentEnum.Never)
            {
                if (Substitutions != null)
                {
                    applicableSubstitution.AddRange(Substitutions);
                }
                if (Substitutions == null || !Substitutions.Override)
                {
                    applicableSubstitution.AddRange(config.DefaultSubstitutions);
                }
            }

            if (wordList.Count > 0)
            {
                entropyVal += Math.Log(wordList.Count, 2);
            }

            /* collect stats about the word list and how substitions apply */
            double avg_word_len = 0;
            int subst_hit_count = 0;
            foreach (string word in wordList)
            {
                avg_word_len += word.Length;
                foreach (BaseSubstitution substitution in applicableSubstitution)
                {
                    subst_hit_count += new SubstitutionVisitor(null)
                                            .CountSubstitution(substitution, word);
                }
            }
            avg_word_len /= wordList.Count;
            double avg_subst_hits = (double)subst_hit_count / wordList.Count;

            /* apply information added by capitalization */
            double cap_chance = 0;
            if (Capitalize != CapitalizeEnum.Proper)
            {
                cap_chance = (double)Capitalize / 100.0;
            }
            if (cap_chance > 0.0 && cap_chance < 1.0)
            {
                entropyVal += avg_word_len * cap_chance * Math.Log(1 / cap_chance, 2);
                entropyVal += avg_word_len * (1 - cap_chance) * Math.Log(1 / (1 - cap_chance), 2);
            }

            /* apply average information added by substitutions */
            double subst_chance = (double)Substitution / 100.0;
            subst_chance = (double)Substitution / 100.0;
            if (subst_chance > 0.0 && subst_chance < 1.0)
            {
                entropyVal += avg_subst_hits * subst_chance * Math.Log(1 / subst_chance, 2);
                entropyVal += avg_subst_hits * (1 - subst_chance) * Math.Log(1 / (1 - subst_chance), 2);
            }

            /* Chance of not including the word actually decreases entropy
             * because there is a chance the attacker needs to try fewer words.
             */
            entropyVal = entropyVal * (double)Probability / 100;

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
        public PercentEnum Substitution
        {
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
