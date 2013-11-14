using System;
using System.Collections.Generic;
using System.Text;

namespace Holo.Processing.Search
{
    public class SimpleReportFormatter
    {
        public static string FormatShort(ICollection<EstimationResult> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            StringBuilder Output = new StringBuilder();

            foreach (EstimationResult DataItem in data)
            {
                PrintSeparator(Output);
                Output.AppendLine();
                Output.AppendFormat("Algorithm: {0}\n\n", DataItem.AlgorithmName);
                Output.AppendFormat("Mean: {0}\n", DataItem.Mean);
                Output.AppendFormat("Standard deviation: {0}\n\n", DataItem.StandardDeviation);
            }

            return Output.ToString();
        }

        private static void PrintSeparator(StringBuilder output)
        {
            output.AppendLine("===========================================");
        }
    }
}
