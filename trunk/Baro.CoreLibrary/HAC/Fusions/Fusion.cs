﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAC.Metrics;

namespace HAC.Fusions
{
    public abstract class Fusion
    {
        protected IDistanceMetric metric;
        public IDistanceMetric Metric
        {
            set { metric = value; }
        }
        public abstract float CalculateDistance(Cluster cluster1, Cluster cluster2);
    }
}
