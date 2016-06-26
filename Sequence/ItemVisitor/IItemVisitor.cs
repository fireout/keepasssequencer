using Sequencer.Configuration.Model;

namespace Sequencer.ItemVisitor
{
    internal interface IItemVisitor<TItem> where TItem : SequenceItem
    {
        string Visit(TItem item, string word, CryptoRandomRange cryptoRandom);
    }
}