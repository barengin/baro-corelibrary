﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.HAC.Metrics
{
    public class JaccardDistance : IDistanceMetric
    {
        public float GetDistance(object[] set1, object[] set2)
        {
            var interSect = set1.Intersect<object>(set2);

            if (interSect.Count() == 0)
                return 1f / float.Epsilon;

            var unionSect = set1.Union<object>(set2);

            return 1f / (((float)interSect.Count() / unionSect.Count()) + float.Epsilon);
        }
    }
}
