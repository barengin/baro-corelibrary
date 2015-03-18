using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAC
{
    /// <summary>
    /// An element consists of an array of data points. To use single values (e.g. to cluster numbers) add that value 
    /// as the sole data point to an element object.
    /// </summary>
    public class Element
    {
        HashSet<object> dataPoints = new HashSet<object>();
        string id;

        /// <summary>
        /// Returns the id of the element.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        public Element() { }

        public Element(object[] dataPoints)
        {
            this.AddDataPoints(dataPoints);
        }

        public Element(string id)
        {
            this.id = id;
        }

        public Element(string id, object[] dataPoints)
        {
            this.id = id;
            this.AddDataPoints(dataPoints);
        }

        public void AddDataPoint(object dataPoint)
        {
            dataPoints.Add(dataPoint);
        }

        public void AddDataPoints(object[] dataPoints)
        {
            foreach(object point in dataPoints)
                this.dataPoints.Add(point);
        }

        public object[] GetDataPoints()
        {
            return dataPoints.ToArray<object>();
        }
    }
}
