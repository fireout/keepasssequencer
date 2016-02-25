using System;
using System.Collections.Generic;
using Sequencer.Configuration.Model;
namespace Sequencer.ItemVisitor
{
    class SubstitutionsVisitor : IItemVisitor<WordSequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public SubstitutionsVisitor(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }
        public string Visit(WordSequenceItem item, string word, CryptoRandomRange cryptoRandom)
        {

            if (item.Substitution > PercentEnum.Never)
            {
                List<BaseSubstitution> applicableSubstitution = new List<BaseSubstitution>();
                if (item.Substitutions != null)
                    applicableSubstitution.AddRange(item.Substitutions);
                if (item.Substitutions == null || !item.Substitutions.Override)
                    applicableSubstitution.AddRange(_globalConfiguration.DefaultSubstitutions);
                foreach (BaseSubstitution substitution in applicableSubstitution)
                    if (cryptoRandom.GetRandomInRange(1, 100) <= (ulong)item.Substitution)
                        word = new SubstitutionVisitor().ApplySubstitutionItem(substitution, word);
            }
            return word;
        }
    }
}
