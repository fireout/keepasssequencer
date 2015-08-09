using System;
using System.Collections.Generic;
using WordSequence.Configuration;

namespace WordSequence.ItemGenerator
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
            string targetWord;
            {
                List<string> wordList = new List<string>();
                if (item.Words != null)
                    wordList.AddRange(item.Words);
                if (item.Words == null || !item.Words.Override)
                    wordList.AddRange(_globalConfiguration.DefaultWords);

                targetWord = wordList[cryptoRandom.GetRandomInRange(0, wordList.Count-1)];
            }

            return targetWord;
        }
    }
}

/* vim: set ts=4 sw=4 et: */
