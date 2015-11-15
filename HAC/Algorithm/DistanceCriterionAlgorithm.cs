using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAC.Algorithm
{
    /// <summary>
    /// Implements the cluster algorithm by stopping it when the lowest distance of two clusters is greater 
    /// than a given maximum distance (distance criterion).
    /// </summary>
    internal class DistanceCriterionAlgorithm : AbstractCriterionAlgorithm
    {
        float maximumDistance;
        int minCountClustersToCreate;

        /// <summary>
        /// Creates a new instance of the DistanceCriterionAlgorithm.
        /// </summary>
        /// <param name="maximumDistance">The maximum distance to merge two clusters. 
        /// The algorithm stops if the distance of all clusters is greater than maximumDistance.</param>
        /// <param name="minCountClustersToCreate">The minimum count of clusters to create</param>
        internal DistanceCriterionAlgorithm(float maximumDistance, int minCountClustersToCreate)
        {
            this.maximumDistance = maximumDistance;
            this.minCountClustersToCreate = minCountClustersToCreate;
        }

        protected override bool isFinished(ICollection<Cluster> currentClusters, ClusterPair lowestDistancePair)
        {
            return currentClusters.Count == minCountClustersToCreate || lowestDistancePair.Distance > maximumDistance;
        }
    }
}
