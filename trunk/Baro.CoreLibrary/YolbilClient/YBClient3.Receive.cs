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
    partial class YBClient3
    {
        private ArraySegmentSeamlessBuffer<byte> _buffer = new ArraySegmentSeamlessBuffer<byte>();
        private byte[] _tsBuffer = new byte[4096];

        private readonly int MSG_ACK2 = MessageAttribute.GetMessageID(typeof(PredefinedCommands.Ack2));
        private readonly int KEEP_ALIVE = MessageAttribute.GetMessageID(typeof(PredefinedCommands.KeepAlive));

        private enum ProcessBufferResult
        {
            OK,
            MessageNotRegistered,
            MessageCRCError
        }

        private sealed class State
        {
            public byte[] _receiveBuffer = new byte[4096];
            public Socket _s;
        }

        private void StartReceive()
        {
            State state = new State() { _s = _socket };

            try
            {
                _socket.BeginReceive(state._receiveBuffer, 0, state._receiveBuffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                DisconnectSocket(ex);
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
                DisconnectSocket(ex);
                return;
            }

            if (readed == 0) // Kapanmış
            {
                DisconnectSocket(null);
                return;
            }

            _buffer.Add(new ArraySegment<byte>(state._receiveBuffer, 0, readed));

            if (ProcessBufferList() != ProcessBufferResult.OK)
            {
                DisconnectSocket(null);
                return;
            }

            StartReceive();
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

        private void MessageListener(MessageHeader header, object obj, out bool handled)
        {
            handled = false;

            if (header.CommandID == MSG_ACK2)
            {
                PredefinedCommands.Ack2 a = (PredefinedCommands.Ack2)obj;

                _ackList.Add(a);

                handled = true;
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
    }
}
