using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Contains values that specify the RasConnState and RasError codes of the connection.  
    /// </summary> 
    public struct RasConnectionStatus
    {
        /// <summary>
        /// The connection state for the Ras entry
        /// </summary>
        public RasConnState State;
        /// <summary>
        /// The error for the connection state.  RasError.Success signifies no error.
        /// </summary>
        public RasError RasError;
    }
}
