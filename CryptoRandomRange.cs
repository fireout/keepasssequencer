using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;

namespace Sequencer
{
    /* This class exists to restrict the 64-bit random provided by the KeePass
     * library to a range, without bias. Probably this should be built into
     * KeePass...but it's not for now.
     *
     * Note, the naïve approach would be to do min + (rand % max), but if the
     * maximum value of the random does not evenly divide by max, this will cause
     * biases toward the smaller numbers.
     */
    public class CryptoRandomRange 
    {
        public CryptoRandomRange(CryptoRandomStream randStream)
        {
            this.BaseCryptoRandom = randStream;
        }

        /* returns a random number without bias from within a range */
        public ulong GetRandomInRange(ulong min, ulong max)
        {
            if (min > max)
            {
                System.ArgumentException argEx =
                    new System.ArgumentException("Random number range: max smaller than min");
                throw argEx;
            }
            else if (max == min)
            {
                /* Needed for init time because for some reason this function gets
                 * called before any actual limits are set up, or something, and max and
                 * min are both zero.
                 */
                return min;
            }

            ulong maxOffset    = max - min;
            ulong numIntervals = maxOffset+1;
            ulong maxValid     = System.UInt64.MaxValue - (System.UInt64.MaxValue % numIntervals);
            ulong intervalSize = maxValid / numIntervals;
            ulong trialNum;

            /* keep trying random numbers until we get one within the range that won't
             * causes biases
             */
            do
            {
                trialNum = this.BaseCryptoRandom.GetRandomUInt64();
            } while(trialNum > maxValid);
            return min + (trialNum / intervalSize);
        }

        private CryptoRandomStream BaseCryptoRandom;
    }
}

/* vim: set ts=4 sw=4 et: */
