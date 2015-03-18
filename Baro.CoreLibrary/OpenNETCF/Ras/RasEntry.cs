using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using Microsoft.WindowsCE.Forms;
using OpenNETCF.Diagnostics;

namespace OpenNETCF.Net
{
	/// <summary>
	/// Represents the method that will handle notifications for the connection status.
	/// </summary> 
	public delegate void RasNotificationHandler(int hConn, RasConnState State, RasError ErrorCode);

    /// <summary>
    /// Represents the method that handle the event when any errors occur with the internal Ras API
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="ErrorCode"></param>
    public delegate void RasErrorHandler(RasEntry entry, RasError ErrorCode);
    
    /// <summary>
	/// This class describes a phone-book entry. 
	/// </summary>
    public class RasEntry : IDisposable
    {
        private string name;
        private bool bDialing;
        private int m_connectionHandle = 0;
        private RASENTRY rasEntry;
        private MessageHook m_callbackWindow;

        /// <summary>
        /// Occurs when a status of the connection changes. 
        /// </summary>
        public event RasNotificationHandler RasStatus;

        /// <summary>
        /// Occurs when an error has occured with the Ras API
        /// </summary>
        public event RasErrorHandler Error;

        internal RasEntry(int size)
        {
            rasEntry = new RASENTRY(size);
        }

        internal RasEntry(byte[] entryData)
        {
            rasEntry = new RASENTRY(entryData);
        }

        internal IntPtr CallbackWindowHandle
        {
            get { return m_callbackWindow.Hwnd; }
        }

        /// <summary>
        /// Returns the RasDialParams of a phone-book entry. 
        /// </summary>
        public RasDialParams GetDialParams()
        {
            // create the params
            RasDialParams dialParams = new RasDialParams(this.Name);
            // fill 
            dialParams.GetData();
            return dialParams;
        }

        /// <summary>
        /// Changes the RasDialParams of a phone-book entry. 
        /// </summary>
        public void SetDialParams(RasDialParams dParams)
        {
            dParams.EntryName = this.Name;
            dParams.Update();
        }

        /// <summary>
        /// Raises the RasStatus event.
        /// </summary>
        public virtual void OnRasStatus(int hConn, RasConnState state, RasError error)
        {
            LogFile.TimeStampLines = true;
            LogFile.WriteLine(string.Format("RAS Status change: state='{0}' error='{1}'", 
                state.ToString(), error.ToString()));

            if (RasStatus != null)
            {
                RasStatus(m_connectionHandle, state, error);
            }
        }

        internal virtual void OnRasError(RasError error)
        {
            if (Error != null) Error(this, error);
        }

        /// <summary>
        /// Gets or sets the name of a phone-book entry. 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets connection options. It can be one or more of the ConnectionOptions enum values.
        /// </summary>
        public int Options
        {
            get { return rasEntry.dwOptions; }
            set { rasEntry.dwOptions = value; }
        }

        internal byte[] Data
        {
            get { return rasEntry.Data; }
        }

        /// <summary>
        /// Gets or sets string that contains a telephone number. The way RAS uses this string depends on if the Options member specifies the ConnectionOptions.UseCountryAndAreaCodes flag. If the flag is set, RAS combines PhoneNumber with the country and area codes specified by the CountryCode, and AreaCode members. If the flag is not set or if the ConnectionOptions.DialAsLocalCall flag is set, RAS uses the PhoneNumber string as the entire phone number. 
        /// </summary>
        public string PhoneNumber
        {
            get { return rasEntry.szLocalPhoneNumber; }
            set { rasEntry.szLocalPhoneNumber = value; }
        }

        /// <summary>
        /// Gets or sets country code portion of the phone number. This member is ignored unless Options specifies the ConnectionOptions.UseCountryAndAreaCodes flag. 
        /// </summary>
        public int CountryCode
        {
            get { return rasEntry.dwCountryCode; }
            set { rasEntry.dwCountryCode = value; }
        }

        /// <summary>
        /// Gets or sets string that contains the area code. This member is ignored unless Options specifies the ConnectionOptions.UseCountryAndAreaCodes flag. 
        /// </summary>
        public string AreaCode
        {
            get { return rasEntry.szAreaCode; }
            set { rasEntry.szAreaCode = value; }
        }

        /// <summary>
        /// Gets or sets string that contains the name of a TAPI device to use with this phone-book entry.
        /// </summary>
        public string DeviceName
        {
            get { return rasEntry.szDeviceName; }
            set { rasEntry.szDeviceName = value; }
        }

        /// <summary>
        /// Gets or sets string that contains the name of a TAPI device to use with this phone-book entry.
        /// </summary>
        public string DeviceType
        {
            get { return rasEntry.szDeviceType; }
            set { rasEntry.szDeviceType = value; }
        }

        /// <summary>
        /// Gets or sets the IP address to be used while this connection is active.
        /// </summary>
        public string IPAddress
        {
            get { return rasEntry.IPAddress; }
            set { rasEntry.IPAddress = value; }
        }

        /// <summary>
        /// Gets or sets the IP address of the DNS server to be used while this connection is active. 
        /// </summary>
        public string IPAddressDns
        {
            get { return rasEntry.IPAddressDns; }
            set { rasEntry.IPAddressDns = value; }
        }

        /// <summary>
        /// Gets or sets the IP address of a secondary or backup DNS server to be used while this connection is active. 
        /// </summary>
        public string IPAddressDnsAlt
        {
            get { return rasEntry.IPAddressDnsAlt; }
            set { rasEntry.IPAddressDnsAlt = value; }
        }



        /// <summary>
        /// Gets or sets the IP address of the WINS server to be used while this connection is active. 
        /// </summary>
        public string IPAddressWins
        {
            get { return rasEntry.IPAddressWins; }
            set { rasEntry.IPAddressWins = value; }
        }

        /// <summary>
        /// Gets or sets the IP address of a secondary WINS server to be used while this connection is active. 
        /// </summary>
        public string IPAddressWinsAlt
        {
            get { return rasEntry.IPAddressWinsAlt; }
            set { rasEntry.IPAddressWinsAlt = value; }
        }

        /// <summary>
        /// Default constructor of the RasEntry
        /// </summary>
        public RasEntry()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();
            name = "";
            rasEntry = new RASENTRY(3276);
        }

        /// <summary>
        /// Dials the RAS connection
        /// </summary> 
        /// <param name="bAsync">Specifies how-to dial entry synchronously or asynchronously.</param>
        /// <returns> RasError.Success if there was success.</returns>
        public RasError Dial(bool bAsync)
        {
            return Dial(bAsync, null);
        }

        /// <summary>
        /// Dials the RAS connection.
        /// </summary> 
        /// <param name="rasDialParams">Intelliprog.Ras.RasDialParams that includes all values for the RAS connection to dial.</param>
        /// <param name="bAsync">Specifies how-to dial connection synchronously or asynchronously.</param>
        /// <returns> RasError.Success if there was success.</returns>
        public RasError Dial(bool bAsync, RasDialParams rasDialParams)
        {

            uint flag = 0xFFFFFFFF;

            if (bDialing) //don't allow to dial if we are already dialing 
                return RasError.DialAlreadyInProgress;

            if (rasDialParams == null)
            {
                rasDialParams = new RasDialParams(this.Name);
            }

            m_connectionHandle = 0;
            RasError ret = 0;

            bDialing = true;

            if (bAsync)
            {
                m_callbackWindow = new MessageHook(this);
                ret = Native.RasDial(IntPtr.Zero, null, rasDialParams.Data, flag, m_callbackWindow.Hwnd, ref m_connectionHandle);
                bDialing = false;
            }
            else
            {
                ret = Native.RasDial(IntPtr.Zero, null, rasDialParams.Data, 0, IntPtr.Zero, ref m_connectionHandle);
                bDialing = false;
            }

            return ret;
        }

        /// <summary>
        /// Asynchronously terminates a remote access connection
        /// </summary> 
        public void BeginHangup()
        {
            // see if a connection is active
            if (this.Status.State != RasConnState.Disconnected)
            {
                Native.RasHangUp((IntPtr)m_connectionHandle);
            }
        }

        /// <summary>
        /// Synchronously terminates a remote access connection
        /// </summary>
        public void Hangup()
        {
            BeginHangup();

            int timeout = 300;  // 300 * 100 == 30 seconds
            while(this.Status.State != RasConnState.Disconnected)
            {
                Thread.Sleep(100);
                
                if (timeout-- <= 0)
                {
                    throw new TimeoutException("Hangup timed out");
                }
            }
        }

        /// <summary>
        /// A native handle for the connection if it is open
        /// </summary>
        public IntPtr ConnectionHandle
        {
            get { return new IntPtr(m_connectionHandle); }
        }

        /// <summary>
        /// Changes the name of an entry in the phone book. 
        /// </summary> 
        public bool Rename(string newName)
        {
            Native.Error dwError;            // Return code from functions 

            dwError = Native.RasValidateEntryName(null, newName);

            if (dwError != Native.Error.AlreadyExists)
            {
                if (dwError != Native.Error.Success)
                {
                    throw new Exception("Invalid EntryName format.");

                }
            }

            dwError = Native.RasRenameEntry(null, this.Name, newName);

            if (dwError != Native.Error.Success)
            {
                throw new Exception("Unable to rename entry.");
            }

            return true;
        }

        /// <summary>
        /// Returns the connection status. 
        /// </summary> 
        public RasConnectionStatus Status
        {
            get
            {
                RasConnectionStatus status = new RasConnectionStatus();
                if ((m_connectionHandle == 0) || (m_connectionHandle == -1))
                {
                    status.State = RasConnState.Disconnected;
                }
                else
                {
                    RASCONNSTATUS rasStat = new RASCONNSTATUS();
                    if (Native.RasGetConnectStatus((IntPtr)m_connectionHandle, rasStat.Data) == Native.Error.InvalidHandle)
                    {
                        m_connectionHandle = 0;
                        status.State = RasConnState.Disconnected;
                        status.RasError = RasError.Success;
                    }
                    else
                    {
                        status.State = (RasConnState)rasStat.dwRasConnState;
                        status.RasError = (RasError)rasStat.dwError;
                    }
                }
                return status;
            }
        }

        /// <summary>
        /// Overridden.  Returns the name of the RasEntry object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~RasEntry()
        {
            this.Dispose(true);
        }

        #region IDisposable Members

        private bool disposed = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                if (m_connectionHandle != 0)
                    this.Hangup();

            }
            disposed = true;
        }

        #endregion
    }	
}
