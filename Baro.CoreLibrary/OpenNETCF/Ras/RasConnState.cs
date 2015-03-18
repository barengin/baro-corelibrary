using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Contains values that specify the states that may occur during a RAS connection operation. 
    /// </summary> 
    public enum RasConnState
    {
        /// <summary>
        /// The communication port is about to be opened. 
        /// </summary> 
        OpenPort = 0,
        /// <summary>
        /// The communication port has been opened successfully. 
        /// </summary> 
        PortOpened,
        /// <summary>
        /// A device is about to be connected. 
        /// </summary> 
        ConnectDevice,
        /// <summary>
        /// A device has connected successfully.  
        /// </summary> 
        DeviceConnected,
        /// <summary>
        /// All devices in the device chain have successfully connected. At this point, the physical link is established.  
        /// </summary> 
        AllDevicesConnected,
        /// <summary>
        /// The authentication process is starting.  
        /// </summary> 
        Authenticate,
        /// <summary>
        /// An authentication event has occurred.  
        /// </summary> 
        AuthNotify,
        /// <summary>
        /// The client has requested another validation attempt with a new user name/password/domain. 
        /// </summary> 
        AuthRetry,
        /// <summary>
        /// The remote access server has requested a callback number.  
        /// </summary> 
        AuthCallback,
        /// <summary>
        /// The client has requested to change the password on the account.  
        /// </summary> 
        AuthChangePassword,
        /// <summary>
        /// The projection phase is starting.  
        /// </summary> 
        AuthProject,
        /// <summary>
        /// The link-speed calculation phase is starting.   
        /// </summary> 
        AuthLinkSpeed,
        /// <summary>
        /// The link-speed calculation phase is starting.  
        /// </summary> 
        AuthAck,
        /// <summary>
        /// Reauthentication (after callback) is starting.   
        /// </summary> 
        ReAuthenticate,
        /// <summary>
        /// The client has successfully completed authentication. 
        /// </summary> 
        Authenticated,
        /// <summary>
        /// The line is about to disconnect in preparation for callback.  
        /// </summary> 
        PrepareForCallback,
        /// <summary>
        /// The client is delaying in order to give the modem time to reset itself in preparation for callback.  
        /// </summary> 
        WaitForModemReset,
        /// <summary>
        /// The client is waiting for an incoming call from the remote access server.   
        /// </summary> 
        WaitForCallback,
        /// <summary>
        /// It indicates that projection result data is available.  
        /// </summary> 
        Projected,
        /// <summary>
        /// Contains values that specify the states that may occur during a RAS connection operation. 
        /// </summary> 
        Interactive = 0x1000,
        /// <summary>
        /// This state corresponds to the retry authentication state.  
        /// </summary> 
        RetryAuthentication,
        /// <summary>
        /// This state corresponds to the callback state. 
        /// </summary> 
        CallbackSetByCaller,
        /// <summary>
        /// This state corresponds to the change password state. 
        /// </summary> 
        PasswordExpired,
        /// <summary>
        /// Successful connection.  
        /// </summary> 
        Connected = 0x2000,
        /// <summary>
        /// Disconnection or failed connection.  
        /// </summary> 
        Disconnected
    }
}
