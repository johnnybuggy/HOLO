using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Holo.Core;
using HoloDB;

namespace Holo.Processing.Search
{
    public class SearchByRandom : ISearchAlgorithm
    {
        #region Implementation of ISearchAlgorithm

        public string DisplayName
        {
            get
            {
                return "Random";
            }
        }

        public void Search(List<Audio> source, Audio referenceItem, object parameters = null)
        {
            Random Rnd = new Random();

            foreach (Audio Item in source)
            {
                Item.Tag = Rnd.Next();
            }

            source.Sort((a1, a2) => a1.Tag.CompareTo(a2.Tag));
        }

        public void Search(List<Audio> source, int referenceItemIndex, object parameters = null)
        {
            Search(source, source[referenceItemIndex], parameters);
        }

        #endregion
    }
}
