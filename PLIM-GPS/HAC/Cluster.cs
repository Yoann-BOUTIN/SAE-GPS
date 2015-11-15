using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAC.Fusions;

namespace HAC
{
    public class Cluster : IEnumerable
    {
        HashSet<Element> elements = new HashSet<Element>();
        Fusion fusion;
        public String Name { get; set; }

        public Cluster(Fusion fusion)
        {
            this.fusion = fusion;
            
        }

        internal void AddElement(Element element)
        {
            this.elements.Add(element);
        }

        internal void AddElements(Element[] elements)
        {
            foreach (Element e in elements)
                this.elements.Add(e);
        }

        internal Element[] GetElements()
        {
            return elements.ToArray<Element>();
        }

        internal float CalculateDistance(Cluster otherCluster)
        {
            return fusion.CalculateDistance(this, otherCluster);
        }

        #region IEnumerable Member

        public IEnumerator GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        #endregion
    }
}
