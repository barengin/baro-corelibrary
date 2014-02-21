using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAC.Fusions
{
    public class CompleteLinkage : Fusion
    {
        private const float INITIATL_LARGEST_DISTANCE = float.MinValue;

        public override float CalculateDistance(Cluster cluster1, Cluster cluster2)
        {
            float largestDistance = INITIATL_LARGEST_DISTANCE;
            // ConcurrentBag<float> list = new ConcurrentBag<float>();

            foreach (Element elementCluster1 in cluster1)
            {
                foreach (Element elementCluster2 in cluster2)
                {
                    var distance = metric.GetDistance(elementCluster1.GetDataPoints(), elementCluster2.GetDataPoints());

                    if (distance > largestDistance)
                        largestDistance = distance;
                }
            }

            return largestDistance;
        }
    }
}
