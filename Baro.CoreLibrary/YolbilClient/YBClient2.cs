using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Baro.CoreLibrary.Collections;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;
using Baro.CoreLibrary.Core;

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
        private System.Windows.Forms.Control _synchContext;

        private SequenceLog _logger;

        private volatile Socket _socket;

        private ArraySegmentSeamlessBuffer<byte> _buffer = new ArraySegmentSeamlessBuffer<byte>();
        private byte[] _tsBuffer = new byte[4096];

        private LastActivity _lastActivity = new LastActivity();
        private ConnectionSettings _settings;
        private SendQueue _queue;
        private Thread _sendThread;

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

            bool handled;
            MessageListener(header, obj, out handled);

            if (!handled)
            {
                // On Message Received
                FireOnMessageReceived(new MessageReceivedEventArgs() { Header = header, Message = obj });
            }

            // No problem
            return ProcessBufferResult.OK;
        }

        private readonly int MSG_ACK2 = MessageAttribute.GetMessageID(typeof(PredefinedCommands.Ack2));
        private readonly int KEEP_ALIVE = MessageAttribute.GetMessageID(typeof(PredefinedCommands.KeepAlive));

        private AutoResetEvent _waitForEvent = new AutoResetEvent(false);
        private object _waitFor;

        private void MessageListener(MessageHeader header, object obj, out bool handled)
        {
            handled = false;

            // Henüz işlenmemiş ise kapat
            if (_waitFor == null)
                return;

            if (_waitFor is PredefinedCommands.Ack2 && header.CommandID == MSG_ACK2)
            {
                PredefinedCommands.Ack2 a = (PredefinedCommands.Ack2)obj;
                PredefinedCommands.Ack2 w = (PredefinedCommands.Ack2)_waitFor;

                if (a.CreateUniqueID() == w.CreateUniqueID())
                {
                    Log("Ack2");
                    _waitForEvent.Set();
                }

                handled = true;
                _waitFor = null;

                return;
            }

            if (_waitFor is PredefinedCommands.KeepAlive && header.CommandID == KEEP_ALIVE)
            {
                Log("Ack2: Keep_alive");
                _waitForEvent.Set();

                handled = true;
                _waitFor = null;

                return;
            }
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
            _settings = settings;
            _queue = new SendQueue(settings.SentFolder);
        }

        #endregion

        private void _send()
        {
            while (true)
            {
                if (_socket != null && _socket.Connected)
                {
                    _queue.WaitForEvent.WaitOne();

                    // Kuyruk kapatılmış. Yani Client artık kapalı.
                    if (_queue.Closed)
                    {
                        // Çık git, thread kapansın.
                        return;
                    }

                    // Messsage send process
                    Message m;

                    while (!_queue.Closed && _queue.Peek(out m))
                    {
                        if (SendAndWaitForAck(m))
                        {
                            // Herşey yolunda göndermeye devam et
                            _queue.Dequeue(out m);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void StartReceive()
        {
            // Log("StartReceive()");

            if (_socket != null)
            {
                State state = new State() { _s = _socket };

                _socket.BeginReceive(state._receiveBuffer, 0, state._receiveBuffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
        }

        private void ReceiveCallback(IAsyncResult r)
        {
            // Log("ReceiveCB()");

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
                return;
            }

            FireOnConnect(new ConnectedEventArgs());
            StartReceive();

            if (SendAndWaitForAck(Message.Create(new MessageInfo(), _settings.Login, false)))
            {
                _queue.Open();

                _sendThread = new Thread(new ThreadStart(_send));
                _sendThread.IsBackground = true;
                _sendThread.Name = "YBClient Send Thread";
                _sendThread.Start();
            }
            else
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                return;
            }
        }

        private bool SendAndWaitForAck(Message message)
        {
            Log("SendAndWaitForACK2: " + message.GetMessageHeader().CommandID + "," + message.GetMessageHeader().GetMsgID().ToString());

            // Gönderilen mesaj keep-alive ise onu bekle, aksi her durumda ACK2 bekle
            if (message.GetMessageHeader().CommandID == KEEP_ALIVE)
            {
                _waitFor = new PredefinedCommands.KeepAlive();
            }
            else
            {
                _waitFor = PredefinedCommands.Ack2.CreateAck2(message.GetMessageHeader());
            }

            try
            {
                // Gönder
                _socket.Send(message.Data, message.Size, SocketFlags.None);
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                return false;
            }

            // Bekle
            if (_waitForEvent.WaitOne(120000))
            {
                return true;
            }
            else
            {
                DisconnectInternal();
                FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new TimeoutException("Timeout for ACK") });
                return false;
            }
        }

        public void Send(Message m)
        {
            MessageHeader h = m.GetMessageHeader();

            // Sunucu tarafı ise
            if (h.isServerSideCommand())
            {
                // Sunucu tarafı ama disk'e yaz işaretli.
                if (MessageAttribute.GetMessageAttribute(Message.GetTypeFromID(h.CommandID)).SaveToQueue)
                {
                    _queue.Enqueue(m, true);
                }
                else
                {
                    _queue.Enqueue(m, false);
                }
            }
            else
            {
                // Kullanıcı tarafı
                _queue.Enqueue(m, true);
            }
        }

        public void Disconnect()
        {
            DisconnectInternal();
        }

        private void DisconnectInternal()
        {
            lock (_synch)
            {
                _queue.Close();
                
                if (_sendThread != null)
                    _sendThread.Join();

                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
            }
        }
    }
}
