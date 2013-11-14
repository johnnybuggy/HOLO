using System.Collections.Generic;
using Holo.Core;

namespace Holo.Processing.Search
{
    /// <summary>
    /// Represents an algorithm estimation process result.
    /// </summary>
    public class EstimationResult
    {
        public string AlgorithmName;

        public double Mean;

        public double StandardDeviation;

        public Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> Scores;
    }
}
