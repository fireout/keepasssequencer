using System.Globalization;
using Sequencer.Configuration.Model;
using System;

namespace Sequencer.ItemVisitor
{
    class AnySubstitutionVisitor : ISubstitutionVisitor<AnySubstitution>
    {
        public string ApplySubstitutionItem(AnySubstitution substitution, string word)
        {
            return ForEachSubstitution(substitution, word, (f, r) => f + r);
        }

        public int CountSubstitution(AnySubstitution substitution, string word)
        {
            int count = 0;
            ForEachSubstitution(substitution, word, (f, r) =>
            {
                count++;
                return f;
            });
            return count;
        }

        private string ForEachSubstitution(AnySubstitution substitution, string word, Func<string, string, string> operation)
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
                if (replacePattern.Contains(cursorWord[i].ToString(CultureInfo.InvariantCulture)))
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
    }
}