using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed partial class YBClient3: IYBClient
    {
        private System.Windows.Forms.Control _synchContext;
        private object _synch = new object();

        private ConnectionSettings _settings;

        #region Log
        private int _logSequence = 0;
        private Action<string> _logCB;

        private void Log(string l)
        {
            if (_logCB != null)
            {
                Interlocked.Increment(ref _logSequence);
                _logCB(string.Format("{0,3}- {1}", _logSequence, l));
            }
        }

        #endregion

        #region Events
        public event EventHandler<ConnectedEventArgs> OnConnect;

        public event EventHandler<DisconnectedEventArgs> OnDisconnect;

        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        private void FireOnMessageReceived(MessageReceivedEventArgs m)
        {
            Log("EVENT Receive:" + m.Header.CommandID);

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
            Log("EVENT Disconnected:");

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
            Log("EVENT Connected:");

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

        #region CTors
        public YBClient3(ConnectionSettings settings, System.Windows.Forms.Control container, Action<string> logCallback)
            : this(settings, container)
        {
            _logCB = logCallback;
        }

        public YBClient3(ConnectionSettings settings, System.Windows.Forms.Control container)
            : this(settings)
        {
            _synchContext = container;
        }

        public YBClient3(ConnectionSettings settings)
        {
            _settings = settings;
            _sendQueue = new SendQueue(settings.SentFolder);
        }

        #endregion

        public void DeleteFromServer(MessageHeader header)
        {
            PredefinedCommands.MsgDelete delete = new PredefinedCommands.MsgDelete() 
            { 
                MsgId = header.GetMsgID().ToString() 
            };

            Log("Delete from server: " + header.GetMsgID().ToString());
            Send(Message.Create(new MessageInfo(), delete, false, null));
        }

        public WaitHandle Connect()
        {
            return StartConnect();
        }

        public WaitHandle Disconnect()
        {
            return StartDisconnect();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
