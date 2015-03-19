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

        public string Generate(WordSequenceItem item, int seed)
        {
            Random randomSeed = new Random(seed);
            string targetWord;
            {
                List<string> wordList = new List<string>();
                if (item.Words != null)
                    wordList.AddRange(item.Words);
                if (item.Words == null || !item.Words.Override)
                    wordList.AddRange(_globalConfiguration.DefaultWords);

                targetWord = wordList[randomSeed.Next(wordList.Count)];
            }


            return targetWord;

        }
    }
}
