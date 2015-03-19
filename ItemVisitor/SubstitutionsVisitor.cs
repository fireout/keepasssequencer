using System;
using System.Collections.Generic;
using WordSequence.Configuration;

namespace WordSequence.ItemVisitor
{
    class SubstitutionsVisitor : IItemVisitor<WordSequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public SubstitutionsVisitor(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }
        public string Visit(WordSequenceItem item, string word, int seed)
        {

            Random randomSeed = new Random(seed);
            if (item.Substitution > PercentEnum.Never)
            {
                List<BaseSubstitution> applicableSubstitution = new List<BaseSubstitution>();
                if (item.Substitutions != null)
                    applicableSubstitution.AddRange(item.Substitutions);
                if (item.Substitutions == null || !item.Substitutions.Override)
                    applicableSubstitution.AddRange(_globalConfiguration.DefaultSubstitutions);
                foreach (BaseSubstitution substitution in applicableSubstitution)
                    if (randomSeed.Next(101) <= (int)item.Substitution)
                        word = ApplySubstitutionItem(substitution, word);
            }
            return word;
        }

        private string ApplySubstitutionItem(BaseSubstitution substitution, string word)
        {
            throw new NotImplementedException();
        }
    }
}
