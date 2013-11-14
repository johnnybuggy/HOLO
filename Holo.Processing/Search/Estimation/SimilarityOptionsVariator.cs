using System.Collections.Generic;

namespace Holo.Processing.Search
{
    public class SimilarityOptionsVariator
    {
        public IEnumerable<SimilarityOptions> GetNextOptions()
        {
            for (ushort i = 0; i < 16; i++)
            {
                yield return new SimilarityOptions()
                                 {
                                     VolumeDistr = (i & 1) == 1,
                                     Intensity = (i >> 1 & 1) == 1,
                                     LongRhythm = (i >> 2 & 1) == 1,
                                     ShortRhythm = (i >> 3 & 1) == 1
                                 };
            }
        }
    }
}
