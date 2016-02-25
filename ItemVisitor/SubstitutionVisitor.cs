using Sequencer.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sequencer.ItemVisitor
{
    class SubstitutionVisitor : ISubstitutionVisitor<BaseSubstitution>
    {
        public SubstitutionVisitor(CryptoRandomRange cryptoRandom) 
        {
            myCryptoRandom = cryptoRandom;
        }

        public string ApplySubstitutionItem(BaseSubstitution substitution, string word, ulong substChance)
        {
            var anySubstitution = substitution as AnySubstitution;
            if (anySubstitution != null)
            {
                return new AnySubstitutionVisitor(myCryptoRandom).ApplySubstitutionItem(anySubstitution, word, substChance);
            }
            var wholeSubstitution = substitution as WholeSubstitution;
            if (wholeSubstitution != null)
            {
                return new WholeSubstitutionVisitor(myCryptoRandom).ApplySubstitutionItem(wholeSubstitution, word, substChance);
            }
            return word;
        }

        public int CountSubstitution(BaseSubstitution substitution, string word)
        {
            var anySubstitution = substitution as AnySubstitution;
            if (anySubstitution != null)
            {
                return new AnySubstitutionVisitor(myCryptoRandom).CountSubstitution(anySubstitution, word);
            }
            var wholeSubstitution = substitution as WholeSubstitution;
            if (wholeSubstitution != null)
            {
                return new WholeSubstitutionVisitor(myCryptoRandom).CountSubstitution(wholeSubstitution, word);
            }
            return 0;
        }

        private CryptoRandomRange myCryptoRandom;
    }
}
