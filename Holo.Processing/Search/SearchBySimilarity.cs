using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Holo.Core;
using HoloDB;

namespace Holo.Processing.Search
{
    public class SearchBySimilarity : ISearchAlgorithm
    {
        public string DisplayName
        {
            get
            {
                return "Default similarity search";
            }
        }

        public void Search(List<Audio> source, Audio referenceItem, object parameters = null)
        {
            SimilarityOptions Options = parameters as SimilarityOptions;

            if (Options == null)
            {
                throw new ArgumentException("Invalid options");
            }

            var ReferenceTempogram = referenceItem.GetData<Tempogram>();
            var ReferenceVolumDescriptor = referenceItem.GetData<VolumeDescriptor>();

            foreach (var Item in source)
            {
                var Distance = 0f;
                var Tempogram = Item.GetData<Tempogram>();
                if (Tempogram == null || ReferenceTempogram == null)
                    Distance += 1;
                else
                {
                    if (Options.LongRhythm)
                        Distance += ReferenceTempogram.LongTempogram.Distance(Tempogram.LongTempogram);
                    if (Options.ShortRhythm)
                        Distance += ReferenceTempogram.ShortTempogram.Distance(Tempogram.ShortTempogram) * 0.3f;
                    if (Options.Intensity)
                        Distance += Math.Abs(ReferenceTempogram.Intensity - Tempogram.Intensity) * 0.2f;
                }

                if (Options.VolumeDistr)
                {
                    var VolumeDescriptor = Item.GetData<VolumeDescriptor>();
                    if (VolumeDescriptor == null || ReferenceVolumDescriptor == null)
                        Distance += 1f;
                    else
                        Distance += ReferenceVolumDescriptor.Distance(VolumeDescriptor);
                }

                Item.Tag = Distance;
            }

            source.Sort((a1, a2) => a1.Tag.CompareTo(a2.Tag));
        }

        public void Search(List<Audio> source, int referenceItemIndex, object parameters = null)
        {
            Search(source, source[referenceItemIndex]);
        }
    }
}
