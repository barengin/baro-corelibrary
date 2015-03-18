using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Provides data about a remote access connection. 
    /// </summary>
    public class RasConnection
    {

        private string name;
        private int hconnection;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RasConnection()
        {
            Name = "";
            hconnection = 0;
        }

        /// <summary>
        /// String that contains name of a phone-book entry.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Handle to the remote access connection. 
        /// </summary>
        public int hConnection
        {
            get { return hconnection; }
            set { hconnection = value; }
        }
    }
}
