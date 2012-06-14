using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace Baro.CoreLibrary.Gps.Serial
{
    public delegate void DisconnectHandler(Exception reason);
    public delegate void ReadHandler(string data);

    public class SerialPort
    {
        #region Fields
        private int m_baudRate;
        private string m_portName;
        private volatile bool m_threadIsWorking;
        private Thread m_thread;

        #endregion

        #region Events
        public event DisconnectHandler OnDisconnect;
        public event ReadHandler OnRead;

        protected virtual void FireOnDisconnect(Exception reason)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(reason);
            }
        }

        protected virtual void FireOnRead(string data)
        {
            if (OnRead != null)
            {
                OnRead(data);
            }
        }

        #endregion

        public string PortName
        {
            get { return m_portName; }
        }

        public int BaudRate
        {
            get { return m_baudRate; }
        }

        public void Open(string portName, int baudRate)
        {
            Close();

            m_threadIsWorking = true;
            m_baudRate = baudRate;
            m_portName = portName;
            m_thread = new Thread(new ThreadStart(threadProc));
            m_thread.IsBackground = true;
            m_thread.Name = "Serial Port Internal Thread";
            m_thread.Start();
        }

        public void Close()
        {
            if (m_thread != null)
            {
                m_threadIsWorking = false;
                
                if (!m_thread.Join(2500))
                {
                    m_thread.Abort();
                }

                m_thread = null;
            }
        }

        private void threadProc()
        {
            SerialStream stream = null;

            try
            {
                stream = new SerialStream(m_portName, m_baudRate, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            }
            catch (Win32Exception ex)
            {
                FireOnDisconnect(ex);
                m_threadIsWorking = false;
                return;
            }

            StreamReader sr = new StreamReader(stream, System.Text.Encoding.ASCII);

            while (m_threadIsWorking)
            {
                string d = sr.ReadLine();

                if (string.IsNullOrEmpty(d))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    FireOnRead(d);
                }
            }

            stream.Close();
            stream = null;
            FireOnDisconnect(null);
            m_threadIsWorking = false;
        }
    }
}
