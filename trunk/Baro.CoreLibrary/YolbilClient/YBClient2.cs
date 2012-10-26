using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class YBClient2 : IDisposable
    {
        private sealed class ReceiveState
        {
            public byte[] _receiveBuffer = new byte[2048];
            public Socket _socket;
        }

        private System.Threading.Timer _timer;
        private Socket _socket;
        private Control _synchContext;

        #region Events
        public event EventHandler<ConnectedEventArgs> OnConnect;
        public event EventHandler<DisconnectedEventArgs> OnDisconnect;
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        private void FireOnMessageReceived(MessageReceivedEventArgs m)
        {
            if (OnMessageReceived != null)
            {
                if (_synchContext == null)
                {
                    OnMessageReceived(this, m);
                }
                else
                {
                    _synchContext.BeginInvoke(new EventHandler<MessageReceivedEventArgs>(OnMessageReceived), this, m);
                }
            }
        }

        private void FireOnDisconnect(DisconnectedEventArgs d)
        {
            if (OnDisconnect != null)
            {
                if (_synchContext == null)
                {
                    OnDisconnect(this, d);
                }
                else
                {
                    _synchContext.BeginInvoke(new EventHandler<DisconnectedEventArgs>(OnDisconnect), this, d);
                }
            }
        }

        private void FireOnConnect(ConnectedEventArgs c)
        {
            if (OnConnect != null)
            {
                if (_synchContext == null)
                {
                    OnConnect(this, c);
                }
                else
                {
                    _synchContext.BeginInvoke(new EventHandler<ConnectedEventArgs>(OnConnect), this, c);
                }
            }
        }

        #endregion

        public YBClient2(Control container)
            : this()
        {
            _synchContext = container;
        }

        public YBClient2()
        {
            _timer = new System.Threading.Timer(new TimerCallback(checkState), null, 10000, 60000);
        }

        private void checkState(object s)
        {
        }

        public void Connect(ConnectionSettings s)
        {
        }

        public void Disconnect()
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();
        }

        #endregion
    }
}
