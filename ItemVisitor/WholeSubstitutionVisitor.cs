using Sequencer.Configuration.Model;
using System;
namespace Sequencer.ItemVisitor
{
    class WholeSubstitutionVisitor : ISubstitutionVisitor<WholeSubstitution>
    {
        public WholeSubstitutionVisitor(CryptoRandomRange cryptoRandom) 
        {
            myCryptoRandom = cryptoRandom;
        }

        public string ApplySubstitutionItem(WholeSubstitution substitution, string word, ulong substChance)
        {
            return ForEachSubstitution(substitution, word, substChance, (f, r) => f + r);
        }

        public int CountSubstitution(WholeSubstitution substitution, string word)
        {
            int count = 0;
            ForEachSubstitution(substitution, word, 100, (f, r) =>
            {
                count++;
                return f;
            });
            return count;
        }

        private string ForEachSubstitution(WholeSubstitution substitution,
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
                if (cursorWord.Substring(i).StartsWith(replacePattern) &&
                        (substChance >= 100 || myCryptoRandom.GetRandomInRange(1, 100) <= substChance) )
                {
                    substitutedWord = operation(substitutedWord, substitution.With);
                    i += replacePattern.Length - 1;
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
