using System.Globalization;
using WordSequence.Configuration;

namespace WordSequence.ItemVisitor
{
    class AnySubstitutionVisitor : ISubstitutionVisitor<AnySubstitution> 
    {
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
                if (replacePattern.Contains(cursorWord[i].ToString(CultureInfo.InvariantCulture)))
                    substitutedWord += substitution.With;
                else
                    substitutedWord += word[i];
            }
            return substitutedWord;

        }
    }
}