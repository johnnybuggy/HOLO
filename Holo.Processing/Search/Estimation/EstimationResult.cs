using System.Collections.Generic;
using Holo.Core;

namespace Holo.Processing.Search
{
    /// <summary>
    /// Represents an algorithm estimation process result.
    /// </summary>
    public class EstimationResult
    {
        public string AlgorithmName
        {
            get;
            set;
        }

        public double Mean
        {
            get;
            set;
        }

        public double StandardDeviation
        {
            get;
            set;
        }

        public Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> Scores
        {
            get;
            set;
        }
    }
}
