using Sequencer.Configuration.Model;
using System;
namespace Sequencer.ItemVisitor
{
    class WholeSubstitutionVisitor : ISubstitutionVisitor<WholeSubstitution>
    {
        public string ApplySubstitutionItem(WholeSubstitution substitution, string word)
        {
            return ForEachSubstitution(substitution, word, (f, r) => f + r);
        }

        public int CountSubstitution(WholeSubstitution substitution, string word)
        {
            int count = 0;
            ForEachSubstitution(substitution, word, (f, r) =>
            {
                count++;
                return f;
            });
            return count;
        }

        private string ForEachSubstitution(WholeSubstitution substitution, string word, Func<string, string, string> operation)
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

    }
}