using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using Sequencer.Configuration;
using Sequencer.Forms;

namespace WordSequence
{
    public class Sequencer : CustomPwGenerator
    {
        /* if returnEmptyConfig is true, or a user config file is found, returns
         * the path to the user config file.
         *
         * if returnEmptyConfig is false, or the user config file path is not
         * specified, and the user config file is not found, returns the path to
         * the global config.
         *
         * Note reading the global config at startup (for a missing user config
         * file) and then writing the user config after changing settings, allows
         * a default configuration to be specified in the global config.
         */
        private string GetConfigurationPath(bool returnEmptyConfig)
        {
            string config = System.Configuration.ConfigurationManager.AppSettings["userConfigPath"];

            if (null != config)
            {
                config = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        config);
            }

            if (null == config || !(returnEmptyConfig || File.Exists(config)))
            {
                config = System.Configuration.ConfigurationManager.AppSettings["configPath"];
            }

            if (null != config && (returnEmptyConfig || File.Exists(config)))
            {
                return System.IO.Path.GetFullPath(config);
            }
            else
            {
                return null; /* TODO: better to throw exception? */
            }
        }

        public PasswordSequenceConfiguration Load()
        {
            /* pass "false" to GetConfigurationPath to default to the global
             * config when user config not found
             */
            string configFile = GetConfigurationPath(false);
            if (null != configFile && File.Exists(configFile))
            {
                /* TODO: replace xsd path with local path instead of web path
                 * that could change?
                 */
                XmlSerializer serializer = new XmlSerializer(typeof(PasswordSequenceConfiguration), "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");
                FileStream configStream = File.OpenRead(configFile);
                try
                {
                    return (PasswordSequenceConfiguration)serializer.Deserialize(XmlReader.Create(configStream));
                }
                finally
                {
                    configStream.Close();
                }
            }
            else
            {
                /* Config file not found; create empty config */
                return new PasswordSequenceConfiguration(true);
                /* TODO: pop up an error message or something? */
            }
        }

        public void Save(PasswordSequenceConfiguration configuration)
        {
            /* pass "true" to GetConfigurationPath to default to the user config
             * even when it doesn't exist yet; we'll create it here
             */
            string configFile = GetConfigurationPath(true);
            if (null != configFile)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PasswordSequenceConfiguration), "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

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
            /* TODO: pop up an error message or something? */
        }

        public string GenerateSequence(PasswordSequenceConfiguration globalConfiguration, Random randomSeed)
        {
            string targetSequence = string.Empty;
            foreach (SequenceItem sequenceItem in globalConfiguration.Sequence)
                if (sequenceItem.Probability != PercentEnum.Never && randomSeed.Next(101) <= (int)sequenceItem.Probability)
                    targetSequence += GenerateSequenceItem(sequenceItem, globalConfiguration, randomSeed);

            return targetSequence;
        }

        public string GenerateSequenceItem(SequenceItem sequenceItem, PasswordSequenceConfiguration globalConfiguration, Random randomSeed)
        {
            if (sequenceItem is CharacterSequenceItem)
                return GenerateSequenceItem((CharacterSequenceItem)sequenceItem, globalConfiguration, randomSeed);
            if (sequenceItem is WordSequenceItem)
                return GenerateSequenceItem((WordSequenceItem)sequenceItem, globalConfiguration, randomSeed);
            return null;
        }

        public string GenerateSequenceItem(CharacterSequenceItem characterItem, PasswordSequenceConfiguration globalConfiguration, Random randomSeed)
        {
            string targetCharacterSet = string.Empty;
            List<char> characterList = null;
            int length = characterItem.Length;
            if (characterItem.LengthStrength != StrengthEnum.Full && randomSeed.Next(101) < (int)characterItem.LengthStrength)
                length = randomSeed.Next(characterItem.Length + 1);

            while (targetCharacterSet.Length < length)
            {
                if (characterList == null || characterList.Count == 0)
                {
                    characterList = new List<char>();
                    if (characterItem.Characters != null)
                        characterList.AddRange(characterItem.Characters);
                    if (characterItem.Characters == null || !characterItem.Characters.Override)
                        characterList.AddRange(globalConfiguration.DefaultCharacters);
                }

                int charPos = randomSeed.Next(characterList.Count);
                targetCharacterSet += characterList[charPos];
                if (!characterItem.AllowDuplicate)
                    characterList.RemoveAt(charPos);
            }

            return targetCharacterSet;
        }

        public string GenerateSequenceItem(WordSequenceItem wordItem, PasswordSequenceConfiguration globalConfiguration, Random randomSeed)
        {
            string targetWord;
            {
                List<string> wordList = new List<string>();
                if (wordItem.Words != null)
                    wordList.AddRange(wordItem.Words);
                if (wordItem.Words == null || !wordItem.Words.Override)
                    wordList.AddRange(globalConfiguration.DefaultWords);

                targetWord = wordList[randomSeed.Next(wordList.Count)];
            }

            if (wordItem.Substitution > PercentEnum.Never)
            {
                List<BaseSubstitution> applicableSubstitution = new List<BaseSubstitution>();
                if (wordItem.Substitutions != null)
                    applicableSubstitution.AddRange(wordItem.Substitutions);
                if (wordItem.Substitutions == null || !wordItem.Substitutions.Override)
                    applicableSubstitution.AddRange(globalConfiguration.DefaultSubstitutions);
                foreach (BaseSubstitution substitution in applicableSubstitution)
                    if (randomSeed.Next(101) <= (int)wordItem.Substitution)
                        targetWord = ApplySubstitutionItem(substitution, targetWord);
            }

            if (wordItem.Capitalize == CapitalizeEnum.Proper)
            {
                targetWord = targetWord[0].ToString().ToUpper() + targetWord.Substring(1);
            }
            else if (wordItem.Capitalize != CapitalizeEnum.Never)
            {
                string capitalizedWord = string.Empty;
                foreach (char c in targetWord)
                    if (randomSeed.Next(101) <= (int)wordItem.Capitalize)
                        capitalizedWord += c.ToString().ToUpper();
                    else
                        capitalizedWord += c.ToString().ToLower();
                targetWord = capitalizedWord;
            }
            else
            {
                targetWord = targetWord.ToLower();
            }
            return targetWord;
        }

        public string ApplySubstitutionItem(BaseSubstitution substitution, string word)
        {
            if (substitution is WholeSubstitution)
                return ApplySubstitutionItem((WholeSubstitution)substitution, word);
            if (substitution is AnySubstitution)
                return ApplySubstitutionItem((AnySubstitution)substitution, word);
            return null;
        }

        public string ApplySubstitutionItem(WholeSubstitution substitution, string word)
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
                    substitutedWord += substitution.With;
                else
                    substitutedWord += word[i];
            }
            return substitutedWord;
        }
        public string ApplySubstitutionItem(AnySubstitution substitution, string word)
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
                    substitutedWord += substitution.With;
                else
                    substitutedWord += word[i];
            }
            return substitutedWord;
        }

        private PasswordSequenceConfiguration _configuration;
        protected PasswordSequenceConfiguration Configuration { get { return _configuration ?? (_configuration = Load()); } }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            return new ProtectedString(true, GenerateSequence(Load(), new Random((int)(crsRandomSource.GetRandomUInt64() % int.MaxValue))));
        }

        public override string GetOptions(string strCurrentOptions)
        {
            MainForm form = new MainForm();
            form.Configuration = Configuration;
            form.ShowDialog();
            _configuration = null;
            return base.GetOptions(strCurrentOptions);
        }


        public override bool SupportsOptions
        {
            get
            {
                return true;
            }
        }

        private static readonly PwUuid UUID = new PwUuid(new byte[] {
			0x53, 0x81, 0x36, 0x0E, 0xA7, 0xFC, 0x48, 0x36,
			0x9E, 0x9F, 0xA4, 0x4F, 0x1A, 0xF0, 0x58, 0x37 });
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
