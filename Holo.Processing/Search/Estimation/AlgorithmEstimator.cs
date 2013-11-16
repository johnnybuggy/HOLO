using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holo.Core;
using HoloDB;
using MathNet.Numerics.Statistics;

namespace Holo.Processing.Search
{
    public sealed class AlgorithmEstimator
    {
        private readonly HoloCore Core;
        private readonly Func<int, int, int> TransformRankToScore;

        private readonly OrderedSet<SHA1Hash> HashMap;
        private readonly Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> ManualScores;

        public AlgorithmEstimator(HoloCore core, string hashMapPath, string scoresPath)
            : this(core, hashMapPath, scoresPath, DefaultRankToScore)
        {
        }

        public AlgorithmEstimator(HoloCore core, string hashMapPath, string scoresPath, Func<int, int, int> transformRankToScore)
        {
            if (core == null)
            {
                throw new ArgumentNullException("core");
            }

            if (transformRankToScore == null)
            {
                throw new ArgumentNullException("transformRankToScore");
            }

            Core = core;
            TransformRankToScore = transformRankToScore;

            HashMap = LoadHashes(hashMapPath);

            ManualScores = LoadScores(HashMap, scoresPath);
        }

        private static OrderedSet<SHA1Hash> LoadHashes(string fileName)
        {
            OrderedSet<SHA1Hash> Result = new OrderedSet<SHA1Hash>();

            string[] Lines = File.ReadAllLines(fileName);

            for (int i = 1; i < Lines.Length; i++)
            {
                string[] Parts = Lines[i].Split(',');

                SHA1Hash NewHash = SHA1Hash.FromString(Parts[1].Trim('"'));

                Result.Add(NewHash);
            }

            return Result;
        }

        private static Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> LoadScores(OrderedSet<SHA1Hash> hashes, string fileName)
        {
            Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> Result = new Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>>();

            string[] Lines = File.ReadAllLines(fileName);

            for (int i = 1; i < Lines.Length; i++)
            {
                string[] Parts = Lines[i].Split(',');

                int Audio1 = int.Parse(Parts[0]);
                int Audio2 = int.Parse(Parts[1]);
                int Score = int.Parse(Parts[2]);

                SHA1Hash Hash1 = hashes[Audio1 - 1];
                SHA1Hash Hash2 = hashes[Audio2 - 1];

                if (!Result.ContainsKey(Hash1))
                {
                    Result.Add(Hash1, new Dictionary<SHA1Hash, int>());
                }

                if (!Result[Hash1].ContainsKey(Hash2))
                {
                    Result[Hash1].Add(Hash2, Score);
                }
            }

            return Result;
        }

        private static int DefaultRankToScore(int rank, int total)
        {
            float score4 = 0.1f;
            float score3 = 0.4f;
            float score2 = 0.7f;
            float score1 = 1f;

            if (rank > score2 * total)
            {
                return 1;
            }

            if (rank > score3 * total)
            {
                return 2;
            }

            if (rank > score1 * total)
            {
                return 3;
            }

            return 4;
        }

        public EstimationResult EstimateAlgorithm<T>(object parameters = null) where T : ISearchAlgorithm, new()
        {
            return EstimateAlgorithm(new T(), parameters);
        }

        public EstimationResult EstimateAlgorithm(ISearchAlgorithm algorithm, object parameters = null)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }

            Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>> AutoScores = new Dictionary<SHA1Hash, Dictionary<SHA1Hash, int>>();

            List<Audio> Audios = Core.GetAudios().ToList();
            List<Audio> ReferenceList = Audios.ToList();

            foreach (Audio ReferenceAudio in ReferenceList)
            {
                IList<Audio> Series = Audios.Search(ReferenceAudio, algorithm, parameters)
                                            .Where(audio => HashMap.Contains(audio.GetHash()))
                                            .ToList();

                SHA1Hash ReferenceHash = ReferenceAudio.GetHash();

                AutoScores.Add(ReferenceHash, new Dictionary<SHA1Hash, int>());

                int Rank = 0;
                foreach (Audio TargetAudio in Series)
                {
                    AutoScores[ReferenceHash].Add(TargetAudio.GetHash(), TransformRankToScore(Rank++, Series.Count));
                }
            }

            List<double> Errors = new List<double>();
            List<double> ManualScoresList = new List<double>();
            List<double> ReducedAutoScoresList = new List<double>();

            foreach (SHA1Hash Reference in ManualScores.Keys)
            {
                foreach (SHA1Hash Target in ManualScores[Reference].Keys)
                {
                    if (AutoScores.ContainsKey(Reference) && AutoScores[Reference].ContainsKey(Target))
                    {
                        int ReferenceScore = ManualScores[Reference][Target];

                        int AutoScore = AutoScores[Reference][Target];

                        int Error = ReferenceScore - AutoScore;

                        Errors.Add(Error);
                        ManualScoresList.Add(ReferenceScore);
                        ReducedAutoScoresList.Add(AutoScore);
                    }
                }
            }

            double Mean = Errors.Mean();

            double StandardDeviation = Errors.StandardDeviation();

            double Covariance = ManualScoresList.Covariance(ReducedAutoScoresList);
            double PearsonCoeff = Correlation.Pearson(ManualScoresList, ReducedAutoScoresList);

            EstimationResult Result = new EstimationResult()
                {
                    AlgorithmName = algorithm.DisplayName,
                    Parameters = Convert.ToString(parameters),
                    Mean = Mean,
                    StandardDeviation = StandardDeviation,
                    Covariance = Covariance,
                    PearsonCoeff = PearsonCoeff,
                    Scores = AutoScores
                };

            return Result;
        }
    }
}
