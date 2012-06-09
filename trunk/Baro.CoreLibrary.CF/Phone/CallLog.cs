using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public class CallLog : IList, ICollection, IEnumerable, IDisposable
    {
        // Fields
        private int m_count;
        private int m_handle = 0;

        // Methods
        public CallLog()
        {
            if (PhoneOpenCallLog(ref this.m_handle) != 0)
            {
                throw new ExternalException("Error opening Call Log");
            }
            this.m_count = this.Seek(CallLogSeek.End, 0) + 1;
            this.Seek(CallLogSeek.Beginning, 0);
        }

        public void Close()
        {
            if (this.m_handle != 0)
            {
                if (PhoneCloseCallLog(this.m_handle) != 0)
                {
                    throw new ExternalException("Error closing Call Log");
                }
                this.m_handle = 0;
            }
        }

        public void Dispose()
        {
            this.Close();
        }

        ~CallLog()
        {
            this.Close();
        }

        public CallLogEntry GetEntry()
        {
            if (this.m_handle == 0)
            {
                throw new ObjectDisposedException("CallLog closed");
            }
            byte[] array = new byte[0x30];
            BitConverter.GetBytes(array.Length).CopyTo(array, 0);
            if (PhoneGetCallLogEntry(this.m_handle, array) != 0)
            {
                return null;
            }
            return new CallLogEntry(array);
        }

        [DllImport("phone.dll", SetLastError = true)]
        private static extern int PhoneCloseCallLog(int handle);
        
        [DllImport("phone.dll", SetLastError = true)]
        private static extern int PhoneGetCallLogEntry(int handle, byte[] pentry);
        
        [DllImport("phone.dll", SetLastError = true)]
        private static extern int PhoneOpenCallLog(ref int handle);
        
        [DllImport("phone.dll", SetLastError = true)]
        private static extern int PhoneSeekCallLog(int handle, CallLogSeek seek, int iRecord, ref int piRecord);
        
        public int Seek(CallLogSeek seek, int iRecord)
        {
            if (this.m_handle == 0)
            {
                throw new ObjectDisposedException("Call Log closed");
            }
            int piRecord = 0;
            PhoneSeekCallLog(this.m_handle, seek, iRecord, ref piRecord);
            return piRecord;
        }

        void ICollection.CopyTo(Array array, int index)
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CallLogEnumerator(this);
        }

        int IList.Add(object value)
        {
            return 0;
        }

        void IList.Clear()
        {
        }

        bool IList.Contains(object value)
        {
            return false;
        }

        int IList.IndexOf(object value)
        {
            return 0;
        }

        void IList.Insert(int index, object value)
        {
        }

        void IList.Remove(object value)
        {
        }

        void IList.RemoveAt(int index)
        {
        }

        // Properties
        public int Count
        {
            get
            {
                return this.m_count;
            }
        }

        public CallLogEntry this[int index]
        {
            get
            {
                if (this.Seek(CallLogSeek.Beginning, index) == index)
                {
                    return this.GetEntry();
                }
                return null;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
            }
        }

        // Nested Types
        private class CallLogEnumerator : IEnumerator
        {
            // Fields
            private CallLogEntry m_entry;
            private int m_index = -1;
            private CallLog m_parent;

            // Methods
            internal CallLogEnumerator(CallLog parent)
            {
                this.m_parent = parent;
            }

            public bool MoveNext()
            {
                this.m_index++;
                this.m_entry = this.m_parent.GetEntry();
                return (this.m_entry != null);
            }

            public void Reset()
            {
                this.m_parent.Seek(CallLogSeek.Beginning, 0);
                this.m_index = -1;
            }

            // Properties
            public object Current
            {
                get
                {
                    return this.m_entry;
                }
            }
        }
    }
#endif
}
