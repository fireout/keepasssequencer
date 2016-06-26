using Sequencer.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sequencer.Sequence
{
    class SequenceFactory
    {
        public string Generate(PasswordSequenceConfiguration sequence, CryptoRandomRange cryptoRandom)
        {
            if (sequence == null)
                return string.Empty;
            string targetSequence = string.Empty;
            foreach (SequenceItem sequenceItem in sequence.Sequence)
            {
                targetSequence += new ItemGenerator.ItemGenerator(sequence)
                                                   .Generate(sequenceItem, cryptoRandom);
            }
            return targetSequence;
        }

    }
}
