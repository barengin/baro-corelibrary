using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Collections;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient4
    {
        private ACKList _ackList;

        private ArraySegmentSeamlessBuffer<byte> _buffer = new ArraySegmentSeamlessBuffer<byte>();

        private readonly int MSG_ACK2 = MessageAttribute.GetMessageID(typeof(PredefinedCommands.Ack2));
        private readonly int KEEP_ALIVE = MessageAttribute.GetMessageID(typeof(PredefinedCommands.KeepAlive));

        private enum ProcessBufferResult
        {
            OK,
            MessageNotRegistered,
            MessageCRCError
        }

        private ProcessBufferResult ProcessMessage(int size, byte[] buffer)
        {
            MessageHeader header;

            try
            {
                header = Message.GetInternalHeader(buffer);
            }
            catch (MessageCrcException)
            {
                return ProcessBufferResult.MessageCRCError;
            }

            // Değilse normal komut kategorisinde devam et
            object obj = Message.Parse(buffer, header, null);

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

        private void MessageListener(MessageHeader header, object obj, out bool handled)
        {
            handled = false;

            if (header.CommandID == MSG_ACK2)
            {
                PredefinedCommands.Ack2 a = (PredefinedCommands.Ack2)obj;

                Log("ACK2 Received: " + a.CreateUniqueID().ToString());
                _ackList.Add(a);

                handled = true;
                return;
            }
        }

        private byte[] _lastCmd = new byte[16384];

        private ProcessBufferResult ProcessBufferList()
        {
            // todo : _buffer.BufferSize >= Message.MESSAGE_INTERNAL_HEADER_SIZE  bu kısım büyüktür idi,
            // bunun test edılmesı lazım

            // Process edilecek BufferSize eğer INTERNAL_HEADER'dan büyükse bir bilgi gelmiş olma ihtimali var.
            while (_buffer.BufferSize >= Message.MESSAGE_INTERNAL_HEADER_SIZE)
            {
                // Header içinde ilk 4 byte daima SIZE verir.
                _buffer.CopyTo(_lastCmd, 0, 0, 4);

                // Gelen ilk mesajın büyüklüğü
                int size = BitConverter.ToInt32(_lastCmd, 0);

                // Demek ki yeterince veri gelmiş. MQ'ya gönder.
                if (_buffer.BufferSize >= size)
                {
                    // İlk okunacak komut eğer gelmişse, BufferSize >= size, gelen komutu bir array'a kopyala,
                    // çünkü BufferList içinde ArraySegment'ler ile parçalanmış olabilir.
                    if (size > _lastCmd.Length)
                        Array.Resize<byte>(ref _lastCmd, size);

                    _buffer.CopyTo(_lastCmd, 4, 4, size - 4);
                    _buffer.RemoveFromStart(size);

                    // Process regular messages
                    ProcessBufferResult result = ProcessMessage(size, _lastCmd);

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

        private void _doReceiveJob()
        {
            bool quit = false;
            Log("Receive thread created...");

            while (!quit)
            {
                int readed = 0;

                byte[] _tsBuffer = new byte[4096];

                try
                {
                    // Log("Receiving...");
                    readed = _socket.Receive(_tsBuffer);
                    // Log("Received...");
                }
                catch (Exception ex)
                {
                    SocketException se = ex as SocketException;
                    string msg = (se != null) ? ((SocketError)se.ErrorCode).ToString() : ex.Message;

                    Log("Receive thread exception: " + msg);
                    FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                    
                    quit = true;
                    continue;
                }

                // Bağlantı kapatılmış.
                if (readed == 0)
                {
                    Log("Connection is closed by remote");
                    FireOnDisconnect(new DisconnectedEventArgs());
                    
                    quit = true;
                    continue;
                }

                Log(string.Format("Received {0} bytes", readed));

                _lastActivity.Reset();

                _buffer.Add(new ArraySegment<byte>(_tsBuffer, 0, readed));

                if (ProcessBufferList() != ProcessBufferResult.OK)
                {
                    TSDisposeSocket();
                    FireOnDisconnect(new DisconnectedEventArgs());

                    quit = true;
                    continue;
                }
            }
            
            Log("Receive thread disposed...");
        }

        private void CreateReceiveThread()
        {
            Thread t = new Thread(_doReceiveJob);
            t.IsBackground = true;
            t.Start();
        }
    }
}
