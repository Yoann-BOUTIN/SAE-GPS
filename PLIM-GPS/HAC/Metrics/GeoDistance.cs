using HAC.Metrics;
using System;
using System.Diagnostics;

namespace PLIM_GPS.HAC.Metrics
{
    class GeoDistance : IDistanceMetric
    {
        public float GetDistance(object[] set1, object[] set2)
        {
            double lat1 = Convert.ToDouble(set1[1]);
            double lat2 = Convert.ToDouble(set2[1]);

            double long1 = Convert.ToDouble(set1[2]);
            double long2 = Convert.ToDouble(set2[2]);

            var pos1 = new Position() { Latitude = lat1, Longitude = long1 };
            var pos2 = new Position() { Latitude = lat2, Longitude = long2 };

            var distance = Haversine.Distance(pos1, pos2, DistanceType.Kilometers);

            distance *= 1000;

            //Debug.WriteLine("Distance " + distance + " m");

            return (float)distance;
        }
    }
}
