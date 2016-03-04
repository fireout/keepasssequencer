using System.Globalization;
using Sequencer.Configuration.Model;
using System;

namespace Sequencer.ItemVisitor
{
    class AnySubstitutionVisitor : ISubstitutionVisitor<AnySubstitution>
    {
        public AnySubstitutionVisitor(CryptoRandomRange cryptoRandom) 
        {
            myCryptoRandom = cryptoRandom;
        }

        public string ApplySubstitutionItem(AnySubstitution substitution, string word, ulong substChance)
        {
            return ForEachSubstitution(substitution, word, substChance, (f, r) => f + r);
        }

        public int CountSubstitution(AnySubstitution substitution, string word)
        {
            int count = 0;
            ForEachSubstitution(substitution, word, 100, (f, r) =>
            {
                count++;
                return f;
            });
            return count;
        }

        private string ForEachSubstitution(AnySubstitution substitution,
                                           string word,
                                           ulong substChance,
                                           Func<string, string, string> operation)
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
                if (replacePattern.Contains(cursorWord[i].ToString(CultureInfo.InvariantCulture)) &&
                        (substChance >= 100 || myCryptoRandom.GetRandomInRange(1, 100) <= substChance) )
                {
                    substitutedWord = operation(substitutedWord, substitution.With);
                }
                else
                {
                    substitutedWord += word[i];
                }
            }
            return substitutedWord;
        }

        private CryptoRandomRange myCryptoRandom;
    }
}
