using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using ClusterAnalysis;

namespace HOLO
{
    public class HOLOProperties
    {
        public struct HarvestProps
        {
            public int SnapCount;
            public int SnapSize;
            public int DownScaleRate;
            public string StatsCalcMethod;
            public string AmplitudeFunc;
            public float FreqCutoffRate;
            public int ClustersFuzz;
            public int OverlapRate;
            public int DoFullNormalize;

            public ClusterProps ClusteringProperties;
            public string CentroidConfigFile;
        };

        public struct ClusterProps
        {
            public int ClustersCount;
            public string ClustersMethod;
            public int CleanupOutliers;
            public float NeigborhoodDistance;
            public float MinNeighborhoodPerc;
            public string CentroidSearchMethod;
        };
    }
}
