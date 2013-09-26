using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HOLO
{
    public class ClusterAnalysis
    {
        public struct Centroid
        {
            public int Num;
            public List<double> Vars;

            public Centroid(int num)
            {
                Vars = new List<double>();
                Num = num;
            }
        };

        public struct CentroidConfig
        {
            public int K;
            public int NVars;

            public List<Centroid> C;

            public CentroidConfig(double[,] c, int nvars, int k)
            {
                this.K = k;
                this.NVars = nvars;

                C = new List<Centroid>();

                for (int i = 0; i < K; i++)
                {
                    var cin = new Centroid(i);

                    for (int j = 0; j < NVars; j++)
                        cin.Vars.Add(c[j, i]);
                    C.Add(cin);
                }
            }

            public CentroidConfig(int nvars, int k)
            {
                this.K = k;
                this.NVars = nvars;
                C = new List<Centroid>();
            }
        };

        public static int KMeansAnalysis(ref List<List<double>> input, int cl_num, int restarts = 5)
        {
            int NPoints = input.Count;
            int NVars = input[0].Count;
            int K = cl_num;
            int Restarts = restarts;
            int Info = 0;

            var C = new double[NVars, K];
            var XYC = new int[NPoints];
            var XY = new double[NPoints, NVars];

            for (int i = 0; i < NPoints; i++)
            {
                for (int j = 0; j < NVars; j++)
                    XY[i, j] = input[i][j];
            }

            alglib.kmeansgenerate(
                XY,
                NPoints,
                NVars,
                K,
                Restarts,
                out Info,
                out C,
                out XYC);

            SaveCentroids(C, NVars, K, @"centroids.xml");

            return Info;
        }

        public static List<List<double>> PCAnalysis(ref List<List<double>> input, int cl_num, double threshold = 0.005)
        {
            int NPoints = input.Count;
            int NVars = input[0].Count;
            int Info = 0;

            var XY = new double[NPoints, NVars];
            var S2 = new double[NVars];
            var V = new double[NVars, NVars];

            for (int i = 0; i < NPoints; i++)
            {
                for (int j = 0; j < NVars; j++)
                    XY[i, j] = input[i][j];
            }

            alglib.pcabuildbasis(XY, NPoints, NVars, out Info, out S2, out V);

            var Vmatrix = new List<List<double>>();
            int S2thr = S2.Count(v => v > threshold * S2.Sum());
            for (int i = 0; i < S2thr; i++)
            {
                var Vrow = new List<double>();
                for (int j = 0; j < NVars; j++)
                    Vrow.Add(V[i, j]);
                Vmatrix.Add(Vrow);
            }

            Utilities.SaveToXML(Vmatrix, typeof(List<List<double>>), "vmatrix.xml");

            return PCAGetNewSpace(ref input, ref Vmatrix, S2thr);
        }

        public static List<List<double>> PCAGetNewSpace(ref List<List<double>> input, ref List<List<double>> vmatrix, int cutoff)
        {
            var ans = new List<List<double>>();

            for (int i = 0; i < input.Count; i++)
            {
                var vadd = new List<double>();
                                
                for (int j = 0; j < cutoff; j++)
                {
                    double x = 0;
                    for (int k = 0; k < input[0].Count; k++)
                    {
                        x += input[i][k] * vmatrix[j][k];
                    }
                    vadd.Add(-x);
                }
                ans.Add(vadd);
            }
            return ans;
        }

        public static List<double> PCAGetPointInSpace(ref List<List<double>> vmatrix, ref List<double> point)
        {
            var ans = new List<double>();

            for (int i = 0; i < vmatrix.Count; i++)
            {
                double x = 0;
                for (int j = 0; j < vmatrix[0].Count; j++)
                    x += point[j] * vmatrix[i][j];
                ans.Add(x);
            }
            return ans;
        }

        public static void GreedyAnalysis(ref List<List<double>> input, int cl_num, int restarts = 5, string centroid_search = "VolByCorrel")
        {
            int NPoints = input.Count;
            int NVars = input[0].Count;
            int K = cl_num;
            int Restarts = restarts;

            var C = new double[NVars, K];
            var XYC = new int[NPoints];
            var XY = new double[NPoints, NVars];

            var Cs = new List<int>();
            var rgen = new Random();
            double max_effect = 0, prev_max_effect = 0;

            for (int k = 0; k < Restarts; k++)
            {
                var Css = new List<int>();
                Css.Add(rgen.Next(0, NPoints));
                for (int i = 0; i < K ; i++)
                {
                    var next = GreedyChoose(ref Css, input, centroid_search);
                    if (next != -1)
                    {
                        Css.Add(next);
                        if (i == 0)
                            Css.RemoveAt(0);
                    }
                }
                max_effect = GreedyEffectiveness(ref Css, input, centroid_search);
                if (prev_max_effect < max_effect)
                {
                    Cs.Clear();
                    Cs.AddRange(Css);
                    prev_max_effect = max_effect;
                }
            }

            for (int i = 0; i < Cs.Count; i++)
                for (int j = 0; j < NVars; j++)
                    C[j, i] = input[Cs[i]][j];
                        
            SaveCentroids(C, NVars, K, @"centroids.xml");
        }

        private static int GreedyChoose(ref List<int> chosen, List<List<double>> choose_from, string centroid_search)
        {
            int next_chosen = -1;
            double sum_dist = 0, prev_sum_dist = 0;
                        
            for (int i = 0; i < choose_from.Count; i++)
            if (!chosen.Contains(i))
            //if (next_chosen != -1)
            {
                /* SumOfDist VolByCorrel MinDist */
                if (centroid_search == "SumOfDist")
                    sum_dist += chosen.AsParallel().Sum(v => Math.Sqrt(Utilities.VecDiff(choose_from[v], choose_from[i])) /** (1 - Utilities.CalcCorrel(choose_from[v], choose_from[i]))*/);
                else if (centroid_search == "VolByCorrel")
                    sum_dist += chosen.Sum(v => Math.Pow(choose_from[v].Average() - choose_from[i].Average(), 2) * (2 - Utilities.CalcCorrel(choose_from[v], choose_from[i])));
                else if (centroid_search == "MinDist")
                    sum_dist = chosen.AsParallel().Min(v => Math.Sqrt(Utilities.VecDiff(choose_from[v], choose_from[i])));
                
                if (prev_sum_dist < sum_dist)
                {
                    next_chosen = i;
                    prev_sum_dist = sum_dist;
                }
            }
            return next_chosen;
        }

        private static double GreedyEffectiveness(ref List<int> chosen, List<List<double>> choose_from, string centroid_search)
        {
            double sum_dist = 0;
            for (int i = 0; i < choose_from.Count; i++)
                if (!chosen.Contains(i))
                {
                    if (centroid_search == "SumOfDist")
                        sum_dist += chosen.AsParallel().Sum(v => Math.Sqrt(Utilities.VecDiff(choose_from[v], choose_from[i])));
                    else if (centroid_search == "VolByCorrel")
                        sum_dist += chosen.Sum(v => Math.Pow(choose_from[v].Average() - choose_from[i].Average(), 2) * (2 - Utilities.CalcCorrel(choose_from[v], choose_from[i])));
                    else if (centroid_search == "MinDist")
                        sum_dist = chosen.AsParallel().Min(v => Math.Sqrt(Utilities.VecDiff(choose_from[v], choose_from[i])));                    
                }
            return sum_dist;
        }

        public static int CleanupByEpsilon(List<List<double>> input, double eps, double min_eps_perc)
        {
            return input.RemoveAll(v => GetEpsilonCount(input, v, eps) < (min_eps_perc * input.Count));
        }

        public static double AverageDistance(List<List<double>> input)
        {
            var ans = input.AsParallel().Sum(v => input.Sum(w => Math.Sqrt(Utilities.VecDiff(v, w)))) / ((input.Count - 1) * input.Count);
            return ans;
        }

        private static int GetEpsilonCount(List<List<double>> input, List<double> chosen, double eps)
        {
            var ans = input.AsParallel().Count(v => Math.Sqrt(Utilities.VecDiff(v, chosen)) <= eps);
            return ans;
        }

        public static void SaveCentroids(double[,] c, int nvars, int k, string filename)
        {
            Utilities.SaveToXML(new CentroidConfig(c, nvars, k), typeof(CentroidConfig), filename);
            return;
        }

        public static CentroidConfig LoadCentroids(string filename)
        {
            var CC = new CentroidConfig(0, 0);

            CC = (CentroidConfig)Utilities.LoadFromXML(@"centroids.xml", typeof(CentroidConfig));

            return CC;
        }

        public static int ChooseCentroid(ref CentroidConfig cc, List<double> point, int fuzzy = 1)
        {
            int ans = -1;
            var pile = cc.C.AsParallel().OrderBy(c => Utilities.VecDiff(c.Vars, point)).Take(fuzzy).ToList();

            var r = new Random();
            ans = pile[r.Next(0, fuzzy)].Num;

            return ans;
        }

        public static List<double> CentroidDistances(ref CentroidConfig cc, List<double> point)
        {
            var ans = new List<double>();
            cc.C.ForEach(v => ans.Add(Utilities.VecDiff(v.Vars, point)));            
            return ans;
        }
    }
}
