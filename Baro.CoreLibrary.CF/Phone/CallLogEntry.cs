using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public class CallLogEntry : IDisposable
    {
        // Fields
        private bool disposed;
        private static BitVector32.Section fConnected = BitVector32.CreateSection(1, fOutgoing);
        private static BitVector32.Section fEnded = BitVector32.CreateSection(1, fConnected);
        private static BitVector32.Section fOutgoing = BitVector32.CreateSection(1);
        private static BitVector32.Section fRoam = BitVector32.CreateSection(1, fEnded);
        private byte[] m_data;

        // Methods
        internal CallLogEntry(byte[] data)
        {
            this.m_data = data;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }
        }

        ~CallLogEntry()
        {
            this.Dispose(false);
        }

        // Properties
        public CallerIDType CallerIDType
        {
            get
            {
                return (CallerIDType)BitConverter.ToInt32(this.m_data, 0x1c);
            }
        }

        public CallType CallType
        {
            get
            {
                return (CallType)BitConverter.ToInt32(this.m_data, 20);
            }
        }

        public bool Connected
        {
            get
            {
                BitVector32 vector = new BitVector32(BitConverter.ToInt32(this.m_data, 0x18));
                return Convert.ToBoolean(vector[fConnected]);
            }
        }

        public bool Ended
        {
            get
            {
                BitVector32 vector = new BitVector32(BitConverter.ToInt32(this.m_data, 0x18));
                return Convert.ToBoolean(vector[fEnded]);
            }
        }

        public DateTime EndTime
        {
            get
            {
                return DateTime.FromFileTime(BitConverter.ToInt64(this.m_data, 12));
            }
        }

        public string Name
        {
            get
            {
                return Marshal.PtrToStringUni((IntPtr)BitConverter.ToInt32(this.m_data, 0x24));
            }
        }

        public string NameType
        {
            get
            {
                return Marshal.PtrToStringUni((IntPtr)BitConverter.ToInt32(this.m_data, 40));
            }
        }

        public string Note
        {
            get
            {
                return Marshal.PtrToStringUni((IntPtr)BitConverter.ToInt32(this.m_data, 0x2c));
            }
        }

        public string Number
        {
            get
            {
                return Marshal.PtrToStringUni((IntPtr)BitConverter.ToInt32(this.m_data, 0x20));
            }
        }

        public bool Outgoing
        {
            get
            {
                BitVector32 vector = new BitVector32(BitConverter.ToInt32(this.m_data, 0x18));
                return Convert.ToBoolean(vector[fOutgoing]);
            }
        }

        public bool Roaming
        {
            get
            {
                BitVector32 vector = new BitVector32(BitConverter.ToInt32(this.m_data, 0x18));
                return Convert.ToBoolean(vector[fRoam]);
            }
        }

        public DateTime StartTime
        {
            get
            {
                return DateTime.FromFileTime(BitConverter.ToInt64(this.m_data, 4));
            }
        }
    }
#endif
}
