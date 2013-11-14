using System;
using System.Collections.Generic;
using Holo.Core;
using HoloDB;

namespace Holo.Processing.Search
{
    public class SearchByTempoDistribution : ISearchAlgorithm
    {
        public string DisplayName
        {
            get
            {
                return "Sort by tempo distribution";
            }
        }

        public void Search(List<Audio> source, int referenceItemIndex, object parameters = null)
        {
            Search(source, source[referenceItemIndex], parameters);
        }

        public void Search(List<Audio> source, Audio referenceItem, object parameters = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (referenceItem == null)
            {
                throw new ArgumentNullException("referenceItem");
            }

            Tempogram Descriptor = referenceItem.GetData<Tempogram>();

            if (Descriptor != null)
            {
                foreach (var Target in source)
                {
                    Tempogram Tempogram = Target.GetData<Tempogram>();
                    if (Tempogram == null)
                    {
                        Target.Tag = 10;
                        continue;
                    }

                    var d1 = Descriptor.LongTempogram.Distance(Tempogram.LongTempogram);
                    var d2 = Descriptor.ShortTempogram.Distance(Tempogram.ShortTempogram);
                    var d3 = Math.Abs(Descriptor.Intensity - Tempogram.Intensity);
                    Target.Tag = d1 + d2 * 0.0003f + d3 * 0.0002f;
                }
            }

            source.Sort((a1, a2) => a1.Tag.CompareTo(a2.Tag));
        }
    }
}
