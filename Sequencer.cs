using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using Sequencer.Configuration;
using Sequencer.Forms;

namespace Sequencer
{
    public class Sequencer : CustomPwGenerator
    {
        /// <summary>
        /// Loads a PasswordSequenceConfiguration configuration 
        /// </summary>
        /// <param name="profileName">The keepass profile name, that we will be using to construct the filename from, if specified</param>
        /// <returns></returns>
        public PasswordSequenceConfiguration Load(string profileName = null)
        {
            PasswordSequenceConfigurationFactory factory = new PasswordSequenceConfigurationFactory();
            return factory.LoadFromUserFile(profileName) ?? factory.LoadFromSystemFile(profileName);
        }

        public void Save(PasswordSequenceConfiguration configuration)
        {
            /* pass "true" to GetConfigurationPath to default to the user config
             * even when it doesn't exist yet; we'll create it here
             */
            PasswordSequenceConfigurationFactory factory = new PasswordSequenceConfigurationFactory();

            string configFile = factory.GetUserFilePath(configuration.Name);
            if (null != configFile)
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(PasswordSequenceConfiguration),
                                      "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

                /* create the config file's directory if needed */
                Directory.CreateDirectory(Path.GetDirectoryName(configFile));

                /* open the file for writing, creating a new one if needed */
                FileStream configStream = File.Open(configFile, FileMode.Create);

                try
                {
                    serializer.Serialize(configStream, configuration);
                }
                finally
                {
                    configStream.Close();
                }
            }
            /* TODO: should we pop up an error message or something in the
             * "else" case (i.e. when getting config file path fails)?
             */
        }

        public string GenerateSequence(PasswordSequenceConfiguration globalConfiguration,
                                       CryptoRandomRange cryptoRandom)
        {
            string targetSequence = string.Empty;
            foreach (SequenceItem sequenceItem in globalConfiguration.Sequence)
            {
                if (sequenceItem.Probability != PercentEnum.Never &&
                    (int)cryptoRandom.GetRandomInRange(1, 100) <= (int)sequenceItem.Probability)
                {
                    targetSequence += GenerateSequenceItem(sequenceItem, globalConfiguration, cryptoRandom);
                }
            }

            return targetSequence;
        }

        public string GenerateSequenceItem(SequenceItem sequenceItem,
                                           PasswordSequenceConfiguration globalConfiguration,
                                           CryptoRandomRange cryptoRandom)
        {
            if (sequenceItem is CharacterSequenceItem)
                return GenerateSequenceItem((CharacterSequenceItem)sequenceItem, globalConfiguration, cryptoRandom);
            if (sequenceItem is WordSequenceItem)
                return GenerateSequenceItem((WordSequenceItem)sequenceItem, globalConfiguration, cryptoRandom);
            return null;
        }

        public string GenerateSequenceItem(CharacterSequenceItem characterItem,
                                           PasswordSequenceConfiguration globalConfiguration,
                                           CryptoRandomRange cryptoRandom)
        {
            string targetCharacterSet = string.Empty;
            List<char> characterList = null;
            uint length = characterItem.Length;
            if (length > 0 &&
                characterItem.LengthStrength != StrengthEnum.Full &&
                (int)cryptoRandom.GetRandomInRange(1, 100) <= (uint)characterItem.LengthStrength)
            {
                length = (uint)cryptoRandom.GetRandomInRange(0, characterItem.Length - 1);
            }

            while (targetCharacterSet.Length < length &&
                   (null == characterList || characterList.Count > 0))
            {
                if (characterList == null)
                {
                    characterList = new List<char>();
                    if (characterItem.Characters != null)
                        characterList.AddRange(characterItem.Characters);
                    if (characterItem.Characters == null || !characterItem.Characters.Override)
                        characterList.AddRange(globalConfiguration.DefaultCharacters);
                }

                if (characterList.Count > 0)
                {
                    int charPos = (int)cryptoRandom.GetRandomInRange(0, (ulong)characterList.Count - 1);
                    targetCharacterSet += characterList[charPos];
                    if (!characterItem.AllowDuplicate)
                        characterList.RemoveAt(charPos);
                }
            }

            return targetCharacterSet;
        }

        public string GenerateSequenceItem(WordSequenceItem wordItem,
                                           PasswordSequenceConfiguration globalConfiguration,
                                           CryptoRandomRange cryptoRandom)
        {
            string targetWord = string.Empty;
            {
                List<string> wordList = new List<string>();
                if (wordItem.Words != null)
                    wordList.AddRange(wordItem.Words);
                if (wordItem.Words == null || !wordItem.Words.Override)
                    wordList.AddRange(globalConfiguration.DefaultWords);

                if (wordList.Count > 0)
                {
                    targetWord = wordList[(int)cryptoRandom.GetRandomInRange(0, (ulong)wordList.Count - 1)];
                }
            }

            /* somehow count is 1 sometimes when the word coming back is empty...not
             * sure what's going on there, but guard against it here rather than
             * just returning early above.
             */
            if (targetWord != string.Empty)
            {
                if (wordItem.Capitalize == CapitalizeEnum.Proper)
                {
                    targetWord = targetWord[0].ToString().ToUpper() + targetWord.Substring(1);
                }
                else if (wordItem.Capitalize != CapitalizeEnum.Never)
                {
                    string capitalizedWord = string.Empty;
                    foreach (char c in targetWord)
                    {
                        if ((int)cryptoRandom.GetRandomInRange(1, 100) <= (int)wordItem.Capitalize)
                            capitalizedWord += c.ToString().ToUpper();
                        else
                            capitalizedWord += c.ToString().ToLower();
                    }
                    targetWord = capitalizedWord;
                }
                else
                {
                    targetWord = targetWord.ToLower();
                }

                if (wordItem.Substitution > PercentEnum.Never)
                {
                    List<BaseSubstitution> applicableSubstitution = new List<BaseSubstitution>();
                    if (wordItem.Substitutions != null)
                    {
                        applicableSubstitution.AddRange(wordItem.Substitutions);
                    }
                    if (wordItem.Substitutions == null || !wordItem.Substitutions.Override)
                    {
                        applicableSubstitution.AddRange(globalConfiguration.DefaultSubstitutions);
                    }
                    foreach (BaseSubstitution substitution in applicableSubstitution)
                    {
                        if ((int)cryptoRandom.GetRandomInRange(1, 100) <= (int)wordItem.Substitution)
                        {
                            int numhits = 0;
                            targetWord = ApplySubstitutionItem(substitution, targetWord, ref numhits);
                        }
                    }
                }
            }
            return targetWord;
        }

        public static string ApplySubstitutionItem(BaseSubstitution substitution, string word, ref int substitutionCount)
        {
            if (substitution is WholeSubstitution)
                return ApplySubstitutionItem((WholeSubstitution)substitution, word, ref substitutionCount);
            else if (substitution is AnySubstitution)
                return ApplySubstitutionItem((AnySubstitution)substitution, word, ref substitutionCount);
            else
                return null;
        }

        public static string ApplySubstitutionItem(WholeSubstitution substitution, string word, ref int substitutionCount)
        {
            string substitutedWord = string.Empty;
            string replacePattern = substitution.Replace;
            if (!substitution.CaseSensitive)
                replacePattern = replacePattern.ToUpper();
            string cursorWord = word;
            if (!substitution.CaseSensitive)
                cursorWord = cursorWord.ToUpper();

            for (int i = 0; i < word.Length; i++)
            {
                if (cursorWord.Substring(i).StartsWith(replacePattern))
                {
                    substitutedWord += substitution.With;
                    substitutionCount += 1;
                }
                else
                {
                    substitutedWord += word[i];
                }
            }
            return substitutedWord;
        }
        public static string ApplySubstitutionItem(AnySubstitution substitution, string word, ref int substitutionCount)
        {
            string substitutedWord = string.Empty;
            string replacePattern = substitution.Replace;
            if (!substitution.CaseSensitive)
                replacePattern = replacePattern.ToUpper();
            string cursorWord = word;
            if (!substitution.CaseSensitive)
                cursorWord = cursorWord.ToUpper();

            for (int i = 0; i < word.Length; i++)
            {
                if (replacePattern.Contains(cursorWord[i].ToString()))
                {
                    substitutedWord += substitution.With;
                    substitutionCount += 1;
                }
                else
                    substitutedWord += word[i];
            }
            return substitutedWord;
        }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            return new ProtectedString(true, GenerateSequence(Load(prf.CustomAlgorithmOptions), new CryptoRandomRange(crsRandomSource)));
        }

        public override string GetOptions(string strCurrentOptions)
        {
            MainForm form = new MainForm();
            form.Configuration = new Sequencer().Load(strCurrentOptions);
            form.ShowDialog();
            return form.Configuration.Name;
        }

        public override bool SupportsOptions
        {
            get
            {
                return true;
            }
        }

        private static readonly PwUuid UUID =
            new PwUuid(new byte[] {
                0x53, 0x81, 0x36, 0x0E, 0xA7, 0xFC, 0x48, 0x36,
                0x9E, 0x9F, 0xA4, 0x4F, 0x1A, 0xF0, 0x58, 0x37
            });
        public override PwUuid Uuid
        {
            get { return UUID; }
        }
        public override string Name
        {
            get { return "Sequencer"; }
        }
    }
}

/* vim: set ts=4 sw=4 et: */
