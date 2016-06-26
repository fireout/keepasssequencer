using System;
using System.Collections.Generic;
using Sequencer.Configuration.Model;

namespace Sequencer.ItemGenerator
{
    class CharacterItemGenerator : IItemGenerator<CharacterSequenceItem>
    {
        private readonly PasswordSequenceConfiguration _globalConfiguration;
        public CharacterItemGenerator(PasswordSequenceConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;
        }

        public string Generate(CharacterSequenceItem item, CryptoRandomRange cryptoRandom)
        {
            string targetCharacterSet = string.Empty;
            List<char> characterList = null;
            uint length = item.Length;
            if (length > 0 &&
                item.LengthStrength != StrengthEnum.Full &&
                (int)cryptoRandom.GetRandomInRange(1, 100) <= (uint)item.LengthStrength)
            {
                length = (uint)cryptoRandom.GetRandomInRange(0, item.Length - 1);
            }

            while (targetCharacterSet.Length < length &&
                   (null == characterList || characterList.Count > 0))
            {
                if (characterList == null)
                {
                    characterList = new List<char>();
                    if (item.Characters != null)
                        characterList.AddRange(item.Characters);
                    if (item.Characters == null || !item.Characters.Override)
                        characterList.AddRange(_globalConfiguration.DefaultCharacters);
                }

                if (characterList.Count > 0)
                {
                    int charPos = (int)cryptoRandom.GetRandomInRange(0, (ulong)characterList.Count - 1);
                    targetCharacterSet += characterList[charPos];
                    if (!item.AllowDuplicate)
                        characterList.RemoveAt(charPos);
                }
            }

            return targetCharacterSet;
        }
    }
}
