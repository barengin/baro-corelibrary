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
    partial class YBClient3
    {
        private SendQueue _queue;

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
            catch
            {
                DisconnectSocket();
                return false;
            }

            // Bekle
            if (_waitForEvent.WaitOne(60000, false))
            {
                return true;
            }
            else
            {
                DisconnectSocket();
                return false;
            }
        }

        public void Send(Message msg)
        {
            MessageHeader h = msg.GetMessageHeader();

            // Sunucu tarafı ise
            if (h.isServerSideCommand())
            {
                // Sunucu tarafı ama disk'e yaz işaretli.
                if (MessageAttribute.GetMessageAttribute(Message.GetTypeFromID(h.CommandID)).SaveToQueue)
                {
                    _queue.Enqueue(msg, true);
                }
                else
                {
                    _queue.Enqueue(msg, false);
                }
            }
            else
            {
                // Kullanıcı tarafı
                _queue.Enqueue(msg, true);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(StartSend));
        }

        private object _sendSynch = new object();
        private volatile bool _sendRunning = false;

        private void StartSend(object state)
        {
            if (_sendRunning)
                return;

            if (!Connected)
                return;

            lock (_sendSynch)
            {
                _sendRunning = true;
                Message m;

                while (_queue.Peek(out m))
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

                _sendRunning = false;
            }
        }
    }
}
