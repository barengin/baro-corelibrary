using Baro.CoreLibrary.HAC.Fusions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.HAC
{
    public class Cluster : IEnumerable
    {
        HashSet<Element> elements = new HashSet<Element>();
        Fusion fusion;

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

        public Element[] GetElements()
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
