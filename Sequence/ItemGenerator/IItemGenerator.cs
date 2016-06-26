using Sequencer.Configuration.Model;

namespace Sequencer.ItemGenerator
{
    internal interface IItemGenerator<TPart> where TPart : SequenceItem
    {
        string Generate(TPart item, CryptoRandomRange cryptoRandom);
    }
}