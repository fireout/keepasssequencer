using WordSequence.Configuration;

namespace WordSequence.ItemVisitor
{
    interface ISubstitutionVisitor<TSubstitution> where TSubstitution:BaseSubstitution
    {
        string ApplySubstitutionItem(TSubstitution substitution, string word);
    }
}
