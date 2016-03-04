using Sequencer.Configuration.Model;

namespace Sequencer.ItemVisitor
{
    interface ISubstitutionVisitor<TSubstitution> where TSubstitution : BaseSubstitution
    {
        string ApplySubstitutionItem(TSubstitution substitution, string word, ulong substChance);
        int CountSubstitution(TSubstitution substitution, string word);
    }
}
