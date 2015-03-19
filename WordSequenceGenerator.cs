using System;
using System.Diagnostics;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace WordSequence
{
    public class WordSequence : CustomPwGenerator
    {
        private WordSequenceOption _options;
        public WordSequenceOption Options
        {
            get { return _options ?? (_options = WordSequenceOption.Load(Name + ".xml")); }
            set { _options = value; }
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
            get { return "WordSequence"; }
        }

        public override bool SupportsOptions
        {
            get
            {
                return true;
            }
        }

        public override string GetOptions(string strCurrentOptions)
        {
            WordSequenceForm form = new WordSequenceForm();
            form.Options = Options;
            if (form.ShowDialog() == DialogResult.OK)
                WordSequenceOption.Save(Name + ".xml", Options);

            return base.GetOptions(strCurrentOptions);
        }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            if (prf == null) { Debug.Assert(false); }
            else
            {
                Debug.Assert(prf.CustomAlgorithmUuid == Convert.ToBase64String(
                    UUID.UuidBytes, Base64FormattingOptions.None));
            }
            string generated = string.Empty;
            try
            {
                Random rnd = new Random((int)crsRandomSource.GetRandomUInt64());

                Collection<int> selectedWordIndexes = new Collection<int>();
                List<int> extraElementPosition = new List<int>();
                List<int> capitalizePosition = new List<int>();
                List<int> substiteElementPosition = new List<int>();
                for (int i = 0; i < Options.WordCount; i++)
                {
                    if (rnd.Next(0, 10) < Options.ExtraCount)
                        extraElementPosition.Add(i);
                    if (rnd.Next(0, 10) < Options.Capitalize)
                        capitalizePosition.Add(i);
                    if (rnd.Next(0, 10) < Options.SubstitutionCount)
                        substiteElementPosition.Add(i);
                }


                byte includedWord = 0;
                while (includedWord < Options.WordCount)
                {
                    string currentWord = string.Empty;
                    try
                    {
                        int selectedWordIndex = rnd.Next(0, Options.WordSelection.Length);
                        if (selectedWordIndexes.Contains(selectedWordIndex))
                            continue;
                        selectedWordIndexes.Add(selectedWordIndex);
                        currentWord = Options.WordSelection[selectedWordIndex];

                        if ((Options.Capitalize == -1 && includedWord > 0) || (Options.Capitalize == -2 && includedWord == 0) || capitalizePosition.Contains(includedWord))
                        {
                            currentWord = currentWord.Substring(0, 1).ToUpper() + currentWord.Substring(1);
                        }
                        if (extraElementPosition.Contains(includedWord))
                        {
                            currentWord += Options.ExtraChars[rnd.Next(0, Options.ExtraChars.Length)];
                        }
                        if (substiteElementPosition.Contains(includedWord))
                        {
                            for (int i = 0; i < Math.Min(Options.SubstitutionFrom.Length, Options.SubstitutionTo.Length); i++)
                            {
                                currentWord = currentWord.Replace(Options.SubstitutionFrom[i], Options.SubstitutionTo[i]);
                            }
                        }

                        generated += currentWord;
                        includedWord++;

                    }
                    catch (Exception ex)
                    {
                        switch (MessageBox.Show(string.Format("Error including password word '{0}'.\n{1}", currentWord, ex.ToString()), "Error generating word", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error))
                        {
                            case DialogResult.Abort: throw;
                            case DialogResult.Ignore: includedWord++; break;
                        }
                    }
                }

                string uniqueCharGenerated = string.Empty;
                foreach (char c in generated)
                    if ((!prf.NoRepeatingCharacters || !uniqueCharGenerated.Contains(c.ToString())) &&
                        (!prf.ExcludeLookAlike || !"1Ii!|0Z2S5oOl".Contains(c.ToString())) &&
                        (prf.ExcludeCharacters.Length == 0 || !prf.ExcludeCharacters.Contains(c.ToString())))
                        uniqueCharGenerated += c;
                generated = uniqueCharGenerated;

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error generating passwords.\n{0}", ex.ToString()), "Error generating passwords", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new ProtectedString(true, generated);
        }
    }
}
