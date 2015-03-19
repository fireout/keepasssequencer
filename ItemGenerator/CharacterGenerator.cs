using System;
using System.Collections.Generic;
using WordSequence.Configuration;

namespace WordSequence.ItemGenerator
{
    class CharacterItemGenerator : IItemGenerator<CharacterSequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public CharacterItemGenerator(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }

        public string Generate(CharacterSequenceItem item, int seed)
        {
            Random randomSeed = new Random(seed);

            string targetCharacterSet = string.Empty;
            List<char> characterList = null;
            int length = item.Length;
            if (item.LengthStrength != StrengthEnum.Full && randomSeed.Next(101) < (int)item.LengthStrength)
                length = randomSeed.Next(item.Length + 1);

            while (targetCharacterSet.Length < length)
            {
                if (characterList == null || characterList.Count == 0)
                {
                    characterList = new List<char>();
                    if (item.Characters != null)
                        characterList.AddRange(item.Characters);
                    if (item.Characters == null || !item.Characters.Override)
                        characterList.AddRange(_globalConfiguration.DefaultCharacters);
                }

                int charPos = randomSeed.Next(characterList.Count);
                targetCharacterSet += characterList[charPos];
                if (!item.AllowDuplicate)
                    characterList.RemoveAt(charPos);
            }

            return targetCharacterSet;

        }
    }
}