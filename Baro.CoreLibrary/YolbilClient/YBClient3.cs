using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using System.Net.Sockets;
using Baro.CoreLibrary.SockServer;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed partial class YBClient3: IYBClient
    {
        private object _synch = new object();
        private System.Windows.Forms.Control _synchContext;

        private ConnectionSettings _settings;
        private Timer _timer;

        #region Log
        private SequenceLog _logger;

        private void Log(string l)
        {
            if (_logger != null)
                _logger.Log(l);
        }

        #endregion

        #region Events
        public event EventHandler<ConnectedEventArgs> OnConnect;

        public event EventHandler<DisconnectedEventArgs> OnDisconnect;

        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        private void FireOnMessageReceived(MessageReceivedEventArgs m)
        {
            Log("Received: " + m.Header.GetMsgID().ToString());
            Log(DescriptionAttribute.ObjDebugString(m.Message));

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
            Log("Disconnected:");

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
            Log("Connected:");

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
            _logger = new SequenceLog(logCallback);
        }

        public YBClient3(ConnectionSettings settings, System.Windows.Forms.Control container)
            : this(settings)
        {
            _synchContext = container;
        }

        public YBClient3(ConnectionSettings settings)
        {
            _settings = settings;
            _queue = new SendQueue(settings.SentFolder);
        }

        #endregion

        public void DeleteFromServer(MessageHeader header)
        {
            PredefinedCommands.MsgDelete delete = new PredefinedCommands.MsgDelete() 
            { 
                MsgId = header.GetMsgID().ToString() 
            };

            Send(Message.Create(new MessageInfo(), delete, false, null));
        }

        public void Connect()
        {
            // Close the socket
            Disconnect();

            // Start timer
            lock (_synch)
            {
                // Start timer
                _timer = new Timer(new TimerCallback(timerLoop), null, 1000, 60000);
            }
        }

        public void Disconnect()
        {
            // Stop timer
            lock (_synch)
            {
                // Dispose timer
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
            
            // Close the socket
            DisconnectSocket();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
