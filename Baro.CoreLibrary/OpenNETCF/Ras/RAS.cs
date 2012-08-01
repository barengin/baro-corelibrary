using System;
using System.Text;
using System.Collections;
using System.Reflection;

namespace OpenNETCF.Net
{

    /// <summary>
    /// Class that allows a user to gain remote access to the network server by means of a modem. 
    /// </summary>
    public class Ras
    {
        private const int RAS_MaxEntryName = 20;

        private Ras() { }

        private static string[] EnumEntryNames()
        {
            ArrayList al = new ArrayList();

            int cb = 0, cEntries;

            byte[] entries = new byte[RasEntryName.GetSize()];

            // set the dwSize member of the RASENTRYNAME per the docs
            Buffer.BlockCopy(BitConverter.GetBytes(entries.Length), 0, entries, 0, 4); 

            if (Native.RasEnumEntries(null, null, null, ref cb, out cEntries) != 0)
            {
                // if we have more than one, we need to reallocate with the size passed back
                entries = new byte[cb];

                // set the dwSize member of the RASENTRYNAME per the docs
                Buffer.BlockCopy(BitConverter.GetBytes(entries.Length), 0, entries, 0, 4);

                Native.RasEnumEntries(null, null, entries, ref cb, out cEntries);
            }

            for (int i = 0; i < cEntries; i++)
            {
                RasEntryName en = RasEntryName.FromData(entries, RasEntryName.GetSize() * i);
                al.Add(en.ToString());
            }
            return (string[])al.ToArray(typeof(string));
        }

        /// <summary>
        /// Returns array of RasConnection objects
        /// </summary>
        public static RasConnection[] ActiveConnections
        {
            get
            {
                int cb = 0, cConn = 0;

                RasConnection[] rasConn = null;
                RASCONN conn = new RASCONN();
                cb = RASCONN.GetSize();
                int ret = Native.RasEnumConnections(conn.Data, ref cb, ref cConn);

                if (cb == 0)
                    return new RasConnection[0];

                RASCONN bConn = new RASCONN(cb);

                Native.RasEnumConnections(bConn.Data, ref cb, ref cConn);
                rasConn = new RasConnection[cConn];
                for (int i = 0; i < cConn; i++)
                {
                    conn = RASCONN.FromData(bConn.Data, RASCONN.GetSize() * i);
                    rasConn[i] = new RasConnection();
                    rasConn[i].hConnection = conn.hRasConn;
                    rasConn[i].Name = conn.szEntryName;
                }

                return rasConn;
            }
        }

        /// <summary>
        /// Terminates an existing remote access connection
        /// </summary> 
        public static void Hangup(RasConnection conn)
        {
            if (conn != null)
                if (conn.hConnection != 0)
                    Native.RasHangUp((IntPtr)conn.hConnection);
        }


        /// <summary>
        /// Returns array of RasEntry objects
        /// </summary>
        public static RasEntryCollection Entries
        {
            get
            {
                RasEntryCollection entries = new RasEntryCollection();

                string retValue = new string(' ', 255);
                string[] strArr;
                byte[] DevCfg = null;
                int cbEntry = 0, cbDevCfg = 0;
                strArr = EnumEntryNames();
                if (strArr.Length > 0)
                {
                    Native.Error ret = Native.Error.Success;

                    for (int i = 0; i < strArr.Length; i++)
                    {
                        Native.RasGetEntryProperties(null, strArr[i].Trim(), null, ref cbEntry, DevCfg, ref cbDevCfg);
                        RasEntry entry = new RasEntry(cbEntry);
                        entry.Name = strArr[i].Trim();
                        DevCfg = new byte[cbDevCfg];
                        ret = Native.RasGetEntryProperties(null, strArr[i].Trim(), entry.Data, ref cbEntry, DevCfg, ref cbDevCfg);
                        DevCfg = new byte[cbDevCfg];
                        ret = Native.RasGetEntryProperties(null, strArr[i].Trim(), entry.Data, ref cbEntry, DevCfg, ref cbDevCfg);

                        entries.Add(entry);
                    }
                }

                return entries;
            }
        }


        /// <summary>
        /// Returns array of DeviceInfo objects
        /// </summary>
        public static DeviceInfo[] Devices
        {
            get
            {
                int cb = 0;
                int szBuff = 0;
                int res = 0;

                cb = RASDEVINFO.GetSize();

                RASDEVINFO enFirst = new RASDEVINFO();
                res = Native.RasEnumDevices(null, ref cb, ref szBuff);

                RASDEVINFO entries = new RASDEVINFO(cb);

                res = Native.RasEnumDevices(entries.Data, ref cb, ref szBuff);
                DeviceInfo[] devinfo = new DeviceInfo[szBuff];
                for (int i = 0; i < szBuff; i++)
                {
                    RASDEVINFO en = RASDEVINFO.FromData(entries.Data, RASDEVINFO.GetSize() * i);
                    devinfo[i] = new DeviceInfo();
                    devinfo[i].DeviceName = en.szDeviceName;
                    devinfo[i].DeviceType = en.szDeviceType;
                }

                return devinfo;
            }
        }


        /// <summary>
        /// Creates a new RasEntry (phone-book entry).
        /// </summary>
        /// <param name="Name">The name of the RasEntry</param>
        /// <param name="devInfo">valid DeviceInfo structure</param>
        public static RasEntry CreateEntry(string Name, DeviceInfo devInfo)
        {
            RasEntry newEntry = new RasEntry();
            newEntry.Name = Name;
            newEntry.DeviceName = devInfo.DeviceName;
            newEntry.DeviceType = devInfo.DeviceType;

            return CreateEntry(newEntry, null);

        }

        /// <summary>
        /// Creates a new RasEntry (phone-book entry).
        /// </summary>
        /// <param name="newEntry">RasEntry that contains all phone-book entry information.</param>
        /// <param name="dialParams">RasDialParams that contains dial parameters.</param>
        public static RasEntry CreateEntry(RasEntry newEntry, RasDialParams dialParams)
        {
            Native.Error ret = Native.Error.Success;
            int cbEntry = 0, cbDevCfg = 0;

            if (newEntry.DeviceType == "" || newEntry.DeviceName == "")
            {
                throw new Exception("DeviceName and DeviceType must be populated in the RasEntry.");
            }
            //validate entry first
            ret = Native.RasValidateEntryName(null, newEntry.Name);
            if (ret != Native.Error.AlreadyExists)
            {
                if (ret != 0)
                {
                    throw new Exception("Invalid EntryName format.");

                }
            }

            //get the size for a RASENTRY
            ret = Native.RasGetEntryProperties(null, string.Empty, null, ref cbEntry, null, ref cbDevCfg);
            byte[] buffer = new byte[cbEntry];

            // get a default set of properties
            ret = Native.RasGetEntryProperties(null, string.Empty, buffer, ref cbEntry, null, ref cbDevCfg);

            RasEntry entry = new RasEntry(buffer);


            entry.CountryCode = newEntry.CountryCode;
            entry.Name = newEntry.Name;
            entry.Options = newEntry.Options;
            entry.DeviceType = newEntry.DeviceType;
            entry.DeviceName = newEntry.DeviceName;
            entry.AreaCode = newEntry.AreaCode;
            entry.PhoneNumber = newEntry.PhoneNumber;
            entry.IPAddress = newEntry.IPAddress;
            entry.IPAddressDns = newEntry.IPAddressDns;
            entry.IPAddressDnsAlt = newEntry.IPAddressDnsAlt;
            entry.IPAddressWins = newEntry.IPAddressWins;
            entry.IPAddressWinsAlt = newEntry.IPAddressWinsAlt;

            ret = Native.RasSetEntryProperties(null, entry.Name, entry.Data, cbEntry, null, 0);
            if (ret == Native.Error.Success)
            {
                if (dialParams != null)
                {
                    entry.SetDialParams(dialParams);
                }
                return newEntry;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Deletes a RasEntry (phone-book entry).
        /// </summary>
        /// <param name="entry">RasEntry structure.</param>
        public static bool DeleteEntry(RasEntry entry)
        {
            Native.RasDeleteEntry(null, entry.Name);
            return true;
        }
    }
}
