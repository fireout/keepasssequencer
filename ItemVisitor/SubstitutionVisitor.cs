using Sequencer.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sequencer.ItemVisitor
{
    class SubstitutionVisitor : ISubstitutionVisitor<BaseSubstitution>
    {

        public string ApplySubstitutionItem(BaseSubstitution substitution, string word)
        {
            var anySubstitution = substitution as AnySubstitution;
            if (anySubstitution != null)
            {
                return new AnySubstitutionVisitor().ApplySubstitutionItem(anySubstitution, word);
            }
            var wholeSubstitution = substitution as WholeSubstitution;
            if (wholeSubstitution != null)
            {
                return new WholeSubstitutionVisitor().ApplySubstitutionItem(wholeSubstitution, word);
            }
            return word;
        }

        public int CountSubstitution(BaseSubstitution substitution, string word)
        {
            var anySubstitution = substitution as AnySubstitution;
            if (anySubstitution != null)
            {
                return new AnySubstitutionVisitor().CountSubstitution(anySubstitution, word);
            }
            var wholeSubstitution = substitution as WholeSubstitution;
            if (wholeSubstitution != null)
            {
                return new WholeSubstitutionVisitor().CountSubstitution(wholeSubstitution, word);
            }
            return 0;
        }
    }
}
