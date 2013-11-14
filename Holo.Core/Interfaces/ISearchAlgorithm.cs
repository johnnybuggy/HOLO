using System.Collections.Generic;
using HoloDB;

namespace Holo.Core
{
    public interface ISearchAlgorithm
    {
        string DisplayName
        {
            get;
        }

        void Search(List<Audio> source, Audio referenceItem, object parameters = null);

        void Search(List<Audio> source, int referenceItemIndex, object parameters = null);
    }
}
