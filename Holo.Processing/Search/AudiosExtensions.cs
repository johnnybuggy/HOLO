using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Holo.Core;
using HoloDB;

namespace Holo.Processing.Search
{
    public static class AudiosExtensions
    {
        public static IList<Audio> Search(this List<Audio> source, int referenceItemIndex, ISearchAlgorithm algorithm, object parameters = null)
        {
            return source.Search(source[referenceItemIndex], algorithm, parameters);
        }

        public static IList<Audio> Search(this List<Audio> source, Audio referenceItem, ISearchAlgorithm algorithm, object parameters = null)
        {
            algorithm.Search(source, referenceItem, parameters);

            return source;
        }

        public static IList<Audio> SearchBy<T>(this List<Audio> source, int referenceItemIndex, object parameters = null) where T : ISearchAlgorithm, new()
        {
            return source.SearchBy<T>(source[referenceItemIndex], parameters);
        }

        public static IList<Audio> SearchBy<T>(this List<Audio> source, Audio referenceItem, object parameters = null) where T : ISearchAlgorithm, new()
        {
            T Algorithm = new T();

            return source.Search(referenceItem, Algorithm, parameters);
        }
    }
}
