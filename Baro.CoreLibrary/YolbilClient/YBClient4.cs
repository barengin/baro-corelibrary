using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using System.Threading;
using Baro.CoreLibrary.SockServer;
using System.Net.Sockets;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed partial class YBClient4
    {
        private System.Windows.Forms.Control _synchContext;
        private object _synch = new object();

        private ConnectionSettings _settings;
        private SendQueue _sendQueue;

        #region Log
        private int _logSequence = 0;
        private Action<string> _logCB;

        public void Log(string l)
        {
            if (_logCB != null)
            {
                Interlocked.Increment(ref _logSequence);
                _logCB(string.Format("{0,3}- {1}", _logSequence, l));
                // _logCB(l);
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

        private const int FALSE = 0;
        private const int TRUE = 1;

        private int _disconnectEventFired = TRUE;

        private void FireOnDisconnect(DisconnectedEventArgs d)
        {
            if (Interlocked.CompareExchange(ref _disconnectEventFired, TRUE, FALSE) == FALSE)
            {
                if (d.DisconnectReason != null)
                {
                    SocketException se = d.DisconnectReason as SocketException;
                    string msg = (se != null) ? ((SocketError)se.ErrorCode).ToString() : d.DisconnectReason.Message;
                    Log("EVENT Disconnected: " + msg);
                }
                else
                {
                    Log("EVENT Disconnected: " + (d.DisconnectReason != null ? d.DisconnectReason.Message : ""));
                }

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
            else
            {
                Log("EVENT Disconnected NOT FIRED!");
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
        public YBClient4(ConnectionSettings settings, System.Windows.Forms.Control container, Action<string> logCallback)
            : this(settings, container)
        {
            _logCB = logCallback;
        }

        public YBClient4(ConnectionSettings settings, System.Windows.Forms.Control container)
            : this(settings)
        {
            _synchContext = container;
        }

        public YBClient4(ConnectionSettings settings)
        {
            _settings = settings;
            _sendQueue = new SendQueue(settings.SentFolder);
        }

        public YBClient4(ConnectionSettings settings, Action<string> logCallback)
        {
            _logCB = logCallback;
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

        public void Send(Message m)
        {
            Log("Send() " + m.GetMessageHeader().GetMsgID().ToString());

            MessageHeader h = m.GetMessageHeader();

            // Sunucu tarafı ise
            if (h.isServerSideCommand())
            {
                // Sunucu tarafı ama disk'e yaz işaretli.
                if (MessageAttribute.GetMessageAttribute(Message.GetTypeFromID(h.CommandID)).SaveToQueue)
                {
                    _sendQueue.Enqueue(m, true);
                }
                else
                {
                    _sendQueue.Enqueue(m, false);
                }
            }
            else
            {
                // Kullanıcı tarafı
                _sendQueue.Enqueue(m, true);
            }
        }

        public void Connect()
        {
            if (!_readyToConnect)
            {
                throw new Exception("Sistem yeni bir bağlantıya hazır değil");
            }

            TSCreateSocket();

            Interlocked.CompareExchange(ref _disconnectEventFired, FALSE, TRUE);

            try
            {
                _socket.Connect(_settings.Address);
            }
            catch (Exception ex)
            {
                TSDisposeSocket();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                return;
            }

            Log("Connection is starting...");

            CreateReceiveThread();

            Message loginMessage = Message.Create(new MessageInfo(), _settings.Login, false, null);
            
            try
            {
                _socket.Send(loginMessage.Data, loginMessage.Size, SocketFlags.None);
            }
            catch (Exception ex)
            {
                TSDisposeSocket();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                return;
            }

            PredefinedCommands.Ack2 loginAck = PredefinedCommands.Ack2.CreateAck2(loginMessage.GetMessageHeader());

            if (!_ackList.WaitForAck2(loginAck, 60000))
            {
                Log("Login ACK cant received in 1 min.");
                TSDisposeSocket();
                FireOnDisconnect(new DisconnectedEventArgs());
                return;
            }
            else
            {
                Log("Login ACK Ok.");
            }

            CreateSendThread();
            CreateControlThread();

            FireOnConnect(new ConnectedEventArgs());
        }

        public void Disconnect()
        {
            Log("Disconnecting...");
            TSDisposeSocket();
            FireOnDisconnect(new DisconnectedEventArgs());
        }
    }
}
