using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOLO
{
    class StatsAnalysis
    {
        public static Dictionary<string, double> ProcessIntSequence(List<int> seq, int order, bool normalize)
        {
            var ans = new Dictionary<string, double>();

            for (int i = 0; i < seq.Count - order; i++)
            {
                string nextkey = "v" + string.Join("_", seq.Skip(i).Take(order).Select(s => s.ToString()).ToArray());

                if (ans.ContainsKey(nextkey))
                {
                    if (normalize)
                        ans[nextkey] += (double)100 / (double)seq.Count;
                    else
                        ans[nextkey]++;
                }
                else
                {
                    if (normalize)
                        ans.Add(nextkey, (double)100 / (double)seq.Count);
                    else
                        ans.Add(nextkey, 1);
                }
            }

            if (normalize)
                ans = ans.Where(p => p.Value >= 200 / (double)seq.Count).ToDictionary(p => p.Key, p => p.Value);
            else
                ans = ans.Where(p => p.Value >= 2).ToDictionary(p => p.Key, p => p.Value);

            return ans;
        }

        
    }
}
