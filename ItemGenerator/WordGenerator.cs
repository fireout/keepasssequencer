using System;
using System.Collections.Generic;
using Sequencer.Configuration.Model;
using Sequencer;

namespace Sequencer.ItemGenerator
{
    class WordItemGenerator : IItemGenerator<WordSequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public WordItemGenerator(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }

        public string Generate(WordSequenceItem item, CryptoRandomRange cryptoRandom)
        {
            string targetWord = string.Empty;
            {
                List<string> wordList = new List<string>();
                if (item.Words != null)
                    wordList.AddRange(item.Words);
                if (item.Words == null || !item.Words.Override)
                    wordList.AddRange(_globalConfiguration.DefaultWords);

                if (wordList.Count > 0)
                {
                    targetWord = wordList[(int)cryptoRandom.GetRandomInRange(0, (ulong)wordList.Count - 1)];
                }
            }
            return targetWord;
        }
    }
}

/* vim: set ts=4 sw=4 et: */
