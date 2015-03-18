using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Specifies connection options.
    /// </summary>
    public enum ConnectionOptions
    {
        /// <summary>
        /// If this flag is set, the CountryCode, and AreaCode members of the RasEntry are used to construct the phone number. If this flag is not set, these members are ignored. 
        /// </summary>
        UseCountryAndAreaCodes = 0x00000001,
        /// <summary>
        /// If this flag is set, RAS tries to use the IP address specified by RasEntry.IPAddress as the IP address for the dial-up connection. If this flag is not set, the value of the ipaddr member is ignored. 
        /// </summary>
        SpecificIpAddr = 0x00000002,
        /// <summary>
        /// If this flag is set, RAS uses the RasEntry.IPAddressDns, RasEntry.IPAddressDnsAlt, RasEntry.IPAddressWins, and RasEntry.IPAddressWinsAlt members to specify the name server addresses for the dial-up connection. If this flag is not set, RAS ignores these members. 
        /// </summary>
        SpecificNameServers = 0x00000004,
        /// <summary>
        /// If this flag is set, RAS disables the PPP LCP extensions defined in RFC 1570. This may be necessary to connect to certain older PPP implementations, but interferes with features such as server callback. Do not set this flag unless specifically required.
        /// </summary>
        DisableLcpExtensions = 0x00000020,
        /// <summary>
        /// Specifies connection options.
        /// </summary>
        TerminalBeforeDial = 0x00000040,
        /// <summary>
        /// Specifies connection options.
        /// </summary>
        TerminalAfterDial = 0x00000080,
        /// <summary>
        /// If this flag is set, RAS uses the user name, password, and domain of the currently logged-on user when dialing this entry. 
        /// </summary>
        UseLogonCredentials = 0x00004000,
        /// <summary>
        /// Specifies connection options.
        /// </summary>
        DialAsLocalCall = 0x00020000
    }
}
