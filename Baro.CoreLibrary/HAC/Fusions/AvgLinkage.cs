using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAC.Fusions
{
    public class AvgLinkage : Fusion
    {
        public override float CalculateDistance(Cluster cluster1, Cluster cluster2)
        {
            int count = 0;
            float distance = 0;

            foreach (Element elementCluster1 in cluster1)
            {
                foreach (Element elementCluster2 in cluster2)
                {
                    distance += metric.GetDistance(elementCluster1.GetDataPoints(), elementCluster2.GetDataPoints());
                    count++;
                }
            }

            return distance / count;
        }
    }
}
