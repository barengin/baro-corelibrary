using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Baro.CoreLibrary.IO.NativeFile
{
    public unsafe sealed class SimpleFile : IDisposable
    {
        public const int BUFFER_SIZE = 512;

        private NativeFileReader m_file;
        
        private bool m_disposed;
        private int m_position;

        private byte[] m_buffer;

        private int m_bufferPage = -1;
        private int m_currentPage = -2;

        public SimpleFile(NativeFileReader file)
        {
            m_file = file;
            m_buffer = new byte[BUFFER_SIZE];
            Position = 0;
        }

        private void ReadPage()
        {
            m_currentPage = (m_position / BUFFER_SIZE);

            if (m_currentPage != m_bufferPage)
            {
                m_bufferPage = m_currentPage;
                m_file.Position = m_currentPage * BUFFER_SIZE;
                m_file.Read(m_buffer, BUFFER_SIZE);
            }
        }

        public byte Val
        {
            get
            {
                return m_buffer[m_position % BUFFER_SIZE];
            }
        }

        public byte this[int position]
        {
            get
            {
                this.Position = position;
                return this.Val;
            }
        }

        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
                ReadPage();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                m_file.Dispose();
            }

            m_disposed = true;
            GC.SuppressFinalize(this);
        }

        ~SimpleFile()
        {
            Dispose(false);
        }
        #endregion
    }
}
