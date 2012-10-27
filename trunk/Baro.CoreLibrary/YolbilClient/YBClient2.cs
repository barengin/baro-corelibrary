using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Baro.CoreLibrary.Collections;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class YBClient2
    {
        private sealed class State
        {
            public byte[] _receiveBuffer = new byte[4096];
            public Socket _s;
        }

        private object _synch = new object();
        private SequenceLog _logger;

        private System.Threading.Timer _timer;
        private volatile Socket _socket;
        private System.Windows.Forms.Control _synchContext;

        private ArraySegmentSeamlessBuffer<byte> _buffer = new ArraySegmentSeamlessBuffer<byte>();
        private byte[] _tsBuffer = new byte[4096];

        private LastActivity _lastActivity = new LastActivity();
        private ConnectionSettings _settings;

        private void Log(string l)
        {
            if (_logger != null)
                _logger.Log(l);
        }

        #region Process Buffer
        private enum ProcessBufferResult
        {
            OK,
            MessageNotRegistered,
            MessageCRCError
        }

        private ProcessBufferResult ProcessMessage(int size)
        {
            MessageHeader header;

            try
            {
                header = Message.GetInternalHeader(_tsBuffer);
            }
            catch (MessageCrcException)
            {
                return ProcessBufferResult.MessageCRCError;
            }

            // Değilse normal komut kategorisinde devam et
            object obj = Message.Parse(_tsBuffer, header, null);

            // On Message Received
            FireOnMessageReceived(new MessageReceivedEventArgs() { Header = header, Message = obj });

            // No problem
            return ProcessBufferResult.OK;
        }

        private ProcessBufferResult ProcessBufferList()
        {
            // todo : _buffer.BufferSize >= Message.MESSAGE_INTERNAL_HEADER_SIZE  bu kısım büyüktür idi,
            // bunun test edılmesı lazım

            // Process edilecek BufferSize eğer INTERNAL_HEADER'dan büyükse bir bilgi gelmiş olma ihtimali var.
            while (_buffer.BufferSize >= Message.MESSAGE_INTERNAL_HEADER_SIZE)
            {
                // Header içinde ilk 4 byte daima SIZE verir.
                _buffer.CopyTo(_tsBuffer, 0, 0, 4);

                // Gelen ilk mesajın büyüklüğü
                int size = BitConverter.ToInt32(_tsBuffer, 0);

                // Demek ki yeterince veri gelmiş. MQ'ya gönder.
                if (_buffer.BufferSize >= size)
                {
                    // İlk okunacak komut eğer gelmişse, BufferSize >= size, gelen komutu bir array'a kopyala,
                    // çünkü BufferList içinde ArraySegment'ler ile parçalanmış olabilir.
                    if (size > _tsBuffer.Length)
                        Array.Resize<byte>(ref _tsBuffer, size);

                    _buffer.CopyTo(_tsBuffer, 4, 4, size - 4);
                    _buffer.RemoveFromStart(size);

                    // Process regular messages
                    ProcessBufferResult result = ProcessMessage(size);

                    // İşler yolunda gitmedi.
                    if (result != ProcessBufferResult.OK)
                    {
                        // TODO: Exceptions.MessageSocketException("result != ProcessBufferResult.OK - işler yolunda gitmedi ");
                        return result;
                    }
                }
                else
                {
                    // İçeride veri var ama henüz yeterince gelmemiş. Gelmesini bekleyeceğiz.
                    return ProcessBufferResult.OK;
                }
            }

            // Receive olayına devam. Durmak için bir sebep yok.
            return ProcessBufferResult.OK;
        }

        #endregion

        #region Events
        public event EventHandler<ConnectedEventArgs> OnConnect;
        public event EventHandler<DisconnectedEventArgs> OnDisconnect;
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        private void FireOnMessageReceived(MessageReceivedEventArgs m)
        {
            Log("Received:" + m.Header.GetMsgID().ToString());
            Log("ObjValue:" + DescriptionAttribute.ObjDebugString(m.Message));

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

        #region Constructors
        public YBClient2(ConnectionSettings settings, System.Windows.Forms.Control container, Action<string> logCallback)
            : this(settings, container)
        {
            _logger = new SequenceLog(logCallback);
        }

        public YBClient2(ConnectionSettings settings, System.Windows.Forms.Control container)
            : this(settings)
        {
            _synchContext = container;
        }

        public YBClient2(ConnectionSettings settings)
        {
            _timer = new System.Threading.Timer(new TimerCallback(checkState), null, 10000, 120000);
            _settings = settings;
        }

        #endregion

        // Başka bir thread tarafından çağırılıyor. Bu yüzden Timer.Dispose'dan sonra bile çalışmaya devam
        // edecektir.
        private void checkState(object s)
        {
            lock (_synch)
            {
                if (_socket == null)
                    return;

                // TODO:
            }
        }

        private void StartReceive()
        {
            Log("StartReceive()");

            if (_socket != null)
            {
                State state = new State() { _s = _socket };
                
                _socket.BeginReceive(state._receiveBuffer, 0, state._receiveBuffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
        }

        private void ReceiveCallback(IAsyncResult r)
        {
            Log("ReceiveCB()");

            State state = (State)r.AsyncState;
            int readed;

            try
            {
                readed = state._s.EndReceive(r);
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                return;
            }

            if (readed == 0) // Kapanmış
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new NotSupportedException("Unknown") });
                return;
            }

            _buffer.Add(new ArraySegment<byte>(state._receiveBuffer, 0, readed));

            if (ProcessBufferList() != ProcessBufferResult.OK)
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new InvalidOperationException("ProcessBufferException") });
                return;
            }

            _lastActivity.Reset();
            StartReceive();
        }

        public void Connect()
        {
            lock (_synch)
            {
                if (_socket != null)
                    throw new InvalidOperationException("Bu nesne ile daha önce bir bağlantı kurulmuş. Bu nesneyi dispose edip yeni bir YBClient yaratın.");

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            }

            try
            {
                _socket.Connect(_settings.Address);
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
            }

            FireOnConnect(new ConnectedEventArgs());
            StartReceive();
            
            Send(Message.Create(new MessageInfo(), _settings.Login, false));
        }

        public void Send(Message message)
        {
            Log("Send: " + message.GetMessageHeader().CommandID + "@" + message.GetMessageHeader().GetMsgID().ToString());
            _socket.Send(message.Data, message.Size, SocketFlags.None);
        }

        public void Disconnect()
        {
            _timer.Dispose();
            DisconnectInternal();
        }

        private void DisconnectInternal()
        {
            lock (_synch)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
            }
        }
    }
}
