using WordSequence.Configuration;

namespace WordSequence.ItemVisitor
{
    class WholeSubstitutionVisitor : ISubstitutionVisitor<WholeSubstitution>
    {
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
    }
}