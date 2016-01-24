using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sequencer.Configuration.Model
{
    public class WordSequenceOption
    {
        private WordSequenceOption()
        {

        }

        public string[] WordSelection { get; set; }
        public int WordCount { get; set; }

        public int SubstitutionCount { get; set; }
        public string[] SubstitutionFrom { get; set; }
        public string[] SubstitutionTo { get; set; }

        public int ExtraCount { get; set; }
        public char[] ExtraChars { get; set; }

        public int Capitalize { get; set; }

        public static WordSequenceOption Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    FileStream file = new FileStream(filePath, FileMode.Open);

                    XmlSerializer serializer = new XmlSerializer(typeof(WordSequenceOption));
                    WordSequenceOption wordSequenceOption = (WordSequenceOption)serializer.Deserialize(file);
                    file.Close();
                    return wordSequenceOption;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error loading '{0}'.\n{1}", filePath, ex.ToString()), "Error loading options", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return WordSequenceOption.CreateDefaults();
        }

        public static void Save(string filePath, WordSequenceOption options)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WordSequenceOption));
                FileStream textWriter = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                serializer.Serialize(textWriter, options);
                textWriter.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error saving '{0}'.\n{1}", filePath, ex.ToString()), "Error saving options", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static WordSequenceOption CreateDefaults()
        {
            return new WordSequenceOption()
            {
                Capitalize = -1,
                ExtraChars = new char[] { ';', '!' },
                ExtraCount = 1,
                SubstitutionCount = 1,
                SubstitutionFrom = new string[] { "a\r\ni" },
                SubstitutionTo = new string[] { "4\r\n1" },
                WordCount = 3,
                WordSelection = new string[] { "please", "enter", "your", "own", "words" }
            };
        }
    }
}
