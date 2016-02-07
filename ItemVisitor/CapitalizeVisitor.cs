using System;
using System.Globalization;
using Sequencer.Configuration.Model;

namespace Sequencer.ItemVisitor
{
    class CapitalizeVisitor : IItemVisitor<WordSequenceItem>
    {
        public string Visit(WordSequenceItem item, string word, CryptoRandomRange cryptoRandom)
        {
            if (word == null || word.Length == 0)
                return word;

            if (item.Capitalize == CapitalizeEnum.Proper)
            {
                word = word[0].ToString(CultureInfo.InvariantCulture).ToUpper() + word.Substring(1);
            }
            else if (item.Capitalize != CapitalizeEnum.Never)
            {
                string capitalizedWord = string.Empty;
                foreach (char c in word)
                    if ((int)cryptoRandom.GetRandomInRange(0, 101) <= (int)item.Capitalize)
                        capitalizedWord += c.ToString(CultureInfo.InvariantCulture).ToUpper();
                    else
                        capitalizedWord += c.ToString(CultureInfo.InvariantCulture).ToLower();
                word = capitalizedWord;
            }
            else
            {
                word = word.ToLower();
            }
            return word;
        }
    }
}
