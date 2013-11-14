using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holo.Processing.Search
{
    public class SimilarityOptions
    {
        public bool AmpEnvelope;
        public bool Intensity;
        public bool LongRhythm;
        public bool ShortRhythm;
        public bool VolumeDistr;

        public override string ToString()
        {
            return string.Format("Volume: {0}, Intensity: {1}, LRhythm: {2}, SRhythm: {3}", 
                VolumeDistr, Intensity, LongRhythm, ShortRhythm);
        }
    }
}
