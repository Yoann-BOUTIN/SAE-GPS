using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAC.Metrics
{
    public interface IDistanceMetric
    {
        float GetDistance(object[] set1, object[] set2);
    }
}
