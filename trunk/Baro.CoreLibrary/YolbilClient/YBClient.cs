using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.Collections;
using System.Threading;
using Baro.CoreLibrary.Core;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class YBClient
    {
        private sealed class State
        {
            public byte[] receiveBuffer = new byte[2048];
        }

        private object _lock = new object();
        private volatile ConnectionSettings _settings;
        private volatile Socket _socket;

        private PCQ<Message> _list;
        private ArraySegmentSeamlessBuffer<byte> _buffer = new ArraySegmentSeamlessBuffer<byte>();

        private AutoResetEvent _loginEvent = null;

        private Timer _timer;
        private DateTime lastKeepAlive = SystemTime.GetLocalTime().ToDateTime();

        public event Action<YBClient> OnConnected;
        public event Action<YBClient, Exception> OnDisconnected;
        public event Action<YBClient, MessageInternalHeader, object> OnMessageReceived;

        public YBClient(ConnectionSettings settings)
        {
            this.Settings = settings;
        }

        public bool Connected
        {
            get { return _socket != null && _socket.Connected; }
        }

        public ConnectionSettings Settings
        {
            get { return _settings; }
            private set
            {
                _settings = value;
            }
        }

        public void RemoveMsgFromServer(MessageInternalHeader header)
        {
            PredefinedCommands.MsgDelete delete = new PredefinedCommands.MsgDelete() { MsgId = header.GetMsgID().ToString() };
            Message msg_delete = Message.Create(new MessageInfo(), delete, false, null);
            Send(msg_delete);
        }

        public void Send(Message msg)
        {
            AddToSendQueue(msg, true);
        }

        private void AddToSendQueue(Message msg, bool writeToDisk)
        {
            MessageInternalHeader header = msg.GetMessageHeader();
            UniqueID uid = header.GetMsgID();
            string filename = Path.Combine(_settings.SentFolder, uid.ToString() + ".msg");

            // Sunucu tarafını diske yazma.
            if (writeToDisk && msg.GetMessageHeader().CommandID >= 1024)
            {
                FileStream fs = File.Create(filename);
                fs.Write(msg.Data, 0, msg.Size);
                fs.Close();
            }

            _list.Enqueue(msg);
        }

        private void StartSend()
        {
            Message msg;

            if (!_list.Dequeue(out msg))
            {
                return;
            }

            _socket.BeginSend(msg.Data, 0, msg.Size, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }

        private void SendCallback(IAsyncResult r)
        {
            try
            {
                _socket.EndSend(r);
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                if (OnDisconnected != null) OnDisconnected(this, ex);
                return;
            }

            StartSend();
        }

        public void Connect()
        {
            Exception ex = null;
            bool error = false;

            lock (_lock)
            {
                if (_socket != null)
                    return;

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    _socket.Connect(_settings.Address);
                }
                catch (Exception e)
                {
                    error = true;
                    ex = e;
                }

                if (_timer == null)
                    _timer = new Timer(new TimerCallback(SentFolderTimer), null, 5000, 60000);

                // Hata yok.
                if (!error)
                {
                    lastKeepAlive = SystemTime.GetLocalTime().ToDateTime();

                    _buffer.Clear();
                    _list = new PCQ<Message>();

                    StartReceive();

                    // Login
                    Send(Message.Create(new MessageInfo(), _settings.Login, false, null));

                    // DİKKAT: İlk send() methodu StartSend'den önce olmalı.
                    StartSend();

                    // Login for MSG_LIST
                    _loginEvent = new AutoResetEvent(false);

                    // MSG_LIST
                    Send(Message.Create(new MessageInfo(),
                         new PredefinedCommands.MsgList() { Query = "" },
                         false, null));

                    // MSG_LIST için 30sn bekle
                    bool signalled = _loginEvent.WaitOne(30000, false);

                    _loginEvent.Close();
                    _loginEvent = null;

                    if (signalled)
                    {
                        // signal
                    }
                    else
                    {
                        // timeout
                        error = true;
                        ex = new Exception("Timeout");
                    }
                }
            }

            // Hata var.
            if (error)
            {
                DisconnectInternal();
                if (OnDisconnected != null) OnDisconnected(this, ex);
                return;
            }

            // Hata yok.
            if (OnConnected != null) OnConnected(this);
        }

        private void SentFolderTimer(object state)
        {
            if (SystemTime.GetLocalTime().ToDateTime() - lastKeepAlive > new TimeSpan(0, 2, 0))
            {
                // RECONNECT();
                DisconnectInternal();

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                {
                    Thread.Sleep(3000);
                    Connect();
                }));

                return;
            }

            if (_socket == null || !_socket.Connected)
                return;

            Send(Message.Create(new MessageInfo(), new PredefinedCommands.KeepAlive(), false, null));

            string[] files = Directory.GetFiles(_settings.SentFolder, "*.msg");

            foreach (var item in files)
            {
                DateTime dt = File.GetCreationTime(item);
                DateTime st = SystemTime.GetLocalTime().ToDateTime();

                if (st - dt > new TimeSpan(0, 2, 0))
                {
                    Message m = Message.FromFile(item);
                    AddToSendQueue(m, false);
                }
            }
        }

        private void DisconnectInternal()
        {
            lock (_lock)
            {
                if (_socket != null)
                {
                    if (_list != null)
                        _list.Completed();

                    try
                    {
                        _socket.Close();
                    }
                    catch { }

                    _socket = null;
                }
            }
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                DisconnectInternal();

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        private void StartReceive()
        {
            State state = new State();

            _socket.BeginReceive(state.receiveBuffer, 0, state.receiveBuffer.Length,
                SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
        }

        private void ReceiveCallback(IAsyncResult r)
        {
            State state = (State)r.AsyncState;
            int readed;

            try
            {
                readed = _socket.EndReceive(r);
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                if (OnDisconnected != null) OnDisconnected(this, ex);
                return;
            }

            _buffer.Add(new ArraySegment<byte>(state.receiveBuffer, 0, readed));

            if (ProcessBufferList() != ProcessBufferResult.OK)
            {
                DisconnectInternal();
                if (OnDisconnected != null) OnDisconnected(this, new Exception("ProcessBufferResult OK değil"));
                return;
            }

            StartReceive();
        }

        private void SaveToReceivedMessages(MessageInternalHeader header, object obj)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(_settings.ReceivedFolder, header.GetMsgID().ToString() + ".msg"), true))
            {
                sw.WriteLine(DescriptionAttribute.ObjDebugString(obj));
            }
        }

        #region ProcessBuffer
        byte[] _tsBuffer = new byte[2048];

        private enum ProcessBufferResult
        {
            OK,
            MessageNotRegistered,
            MessageCRCError
        }

        private ProcessBufferResult ProcessBufferList()
        {
            // todo : _buffer.BufferSize >= Message.MESSAGE_INTERNAL_HEADER_SIZE  bu kısım büyüktür idi, bunun test edılmesı lazım

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

        private ProcessBufferResult ProcessMessage(int size)
        {
            MessageInternalHeader header;

            try
            {
                header = Message.GetInternalHeader(_tsBuffer);
            }
            catch (MessageCrcException)
            {
                return ProcessBufferResult.MessageCRCError;
            }

            {   // Değilse normal komut kategorisinde devam et

                object obj = Message.Parse(_tsBuffer, header, null);

#if DEBUG
                SaveToReceivedMessages(header, obj);
#endif

                // Sunucu tarafı
                if (header.CommandID < 1024)
                {
                    if (header.CommandID == MessageAttribute.GetMessageID(typeof(PredefinedCommands.KeepAlive)))
                    {
                        lastKeepAlive = SystemTime.GetLocalTime().ToDateTime();
                        return ProcessBufferResult.OK;
                    }

                    if (header.CommandID == MessageAttribute.GetMessageID(typeof(PredefinedCommands.Ack)))
                    {
                        PredefinedCommands.Ack a = (PredefinedCommands.Ack)obj;
                        string filename = Path.Combine(_settings.SentFolder, a.MsgId + ".msg");

                        if (File.Exists(filename))
                            File.Delete(filename);

                        return ProcessBufferResult.OK;
                    }

                    if (header.CommandID == MessageAttribute.GetMessageID(typeof(PredefinedCommands.MsgListResult)))
                    {
                        PredefinedCommands.MsgListResult r = (PredefinedCommands.MsgListResult)obj;
                        string[] list = r.Result.Split(',');

                        foreach (var item in list)
                        {
                            if (string.IsNullOrEmpty(item))
                                continue;

                            PredefinedCommands.MsgGet g = new PredefinedCommands.MsgGet() { MsgId = item };
                            Message m = Message.Create(new MessageInfo(), g, false, null);
                            Send(m);
                        }

                        _loginEvent.Set();
                        return ProcessBufferResult.OK;
                    }
                }

                // User tarafı
                if (OnMessageReceived != null) OnMessageReceived(this, header, obj);

                // No problem
                return ProcessBufferResult.OK;
            }
        }

        #endregion
    }
}
