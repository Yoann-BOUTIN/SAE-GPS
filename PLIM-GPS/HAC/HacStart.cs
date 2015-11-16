using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAC.Fusions;
using HAC.Metrics;
using HAC.Algorithm;

namespace HAC
{
    public class HacStart
    {
        List<Element> elements = new List<Element>();
        ClusterPairs pairs = new ClusterPairs();
        Fusion fusion;
        IDistanceMetric metric;

        /// <summary>
        /// Creates a new HAC object that uses single-linkage as fusion function and the Jaccard index as distance metric 
        /// to cluster the specified elements.
        /// </summary>
        /// <param name="elements"></param>
        public HacStart(Element[] elements)
        {
            setElements(elements);
            this.fusion = new SingleLinkage();
            this.metric = new JaccardDistance();
            this.fusion.Metric = metric;
        }

        /// <summary>
        /// Creates a new HAC object to cluster the specified elements with the specified fusion and 
        /// metric function.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        /// <param name="fusion"></param>
        /// <param name="metric"></param>
        public HacStart(Element[] elements, Fusion fusion, IDistanceMetric metric)
        {
            setElements(elements);
            this.fusion = fusion;
            this.fusion.Metric = metric;
        }

        private void setElements(Element[] elements)
        {
            this.elements.AddRange(elements);
        }

        /// <summary>
        /// Creates countCluster clusters. 
        /// Uses the number criterion to stop the algorithm when the specified count of clusters is reached.
        /// </summary>
        /// <param name="countClusters"></param>
        /// <returns></returns>
        public Cluster[] Cluster(int countClusters)
        {
            AbstractCriterionAlgorithm ncAlgo = new NumberCriterionAlgorithm(countClusters);
            return ncAlgo.Cluster(elements, fusion, metric);
        }

        /// <summary>
        /// <para>Creates clusters until the lowest distance of two clusters is greater than maximumDistance (distance criterion).</para>
        /// <para>Hint: Dependent of the metric and the type of data you must have a roughly suggestion of 
        /// the possible values for maximumDistance to get reasonable results.</para>
        /// </summary>
        /// <param name="maximumDistance"></param>
        /// <param name="minCountClustersToCreate">The minimum count of clusters to create</param>
        /// <returns></returns>
        public Cluster[] Cluster(float maximumDistance, int minCountClustersToCreate)
        {
            AbstractCriterionAlgorithm dcAlgo = new DistanceCriterionAlgorithm(maximumDistance, minCountClustersToCreate);
            return dcAlgo.Cluster(elements, fusion, metric);
        }
    }
}
