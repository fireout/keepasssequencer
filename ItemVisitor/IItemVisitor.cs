using WordSequence.Configuration;

namespace WordSequence.ItemVisitor
{
    internal interface IItemVisitor<TItem> where TItem : SequenceItem
    {
        string Visit(TItem item, string word, int seed);
    }
}