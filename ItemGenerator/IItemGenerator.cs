using WordSequence.Configuration;

namespace WordSequence.ItemGenerator
{
    internal interface IItemGenerator<TPart> where TPart : SequenceItem
    {
        string Generate(TPart item, int seed);
    }
}