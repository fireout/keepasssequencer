using Sequencer.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sequencer.ItemGenerator
{
    class ItemGenerator : IItemGenerator<SequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public ItemGenerator(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }

        public string Generate(SequenceItem item, CryptoRandomRange cryptoRandom)
        {
            if (item.Probability > PercentEnum.Never &&
                (int)cryptoRandom.GetRandomInRange(1, 100) <= (int)item.Probability)
            {
                var characters = item as CharacterSequenceItem;
                if (characters != null)
                {
                    return new CharacterItemGenerator(_globalConfiguration)
                                .Generate(characters, cryptoRandom);

                }
                var words = item as WordSequenceItem;
                if (words != null)
                {
                    string word = string.Empty;
                    word = new WordItemGenerator(_globalConfiguration)
                                .Generate(words, cryptoRandom);

                    word = new ItemVisitor.CapitalizeVisitor().Visit(words, word, cryptoRandom);

                    word = new ItemVisitor.SubstitutionsVisitor(_globalConfiguration)
                                .Visit(words, word, cryptoRandom);

                    return word;
                }

            }
            return string.Empty;
        }
    }
}
