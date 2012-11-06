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
        private SendQueue _sendQueue;
        private ACKList _ackList = new ACKList();

        private bool LoginAndWaitForAck(Message message)
        {
            Log("Login: " + message.GetMessageHeader().GetMsgID().ToString());

            try
            {
                // Gönder
                _socket.Send(message.Data, message.Size, SocketFlags.None);
            }
            catch
            {
                DisposeSocket();
                return false;
            }

            // Bekle
            if (_ackList.WaitForAck2(PredefinedCommands.Ack2.CreateAck2(message.GetMessageHeader()), 60000))
            {
                return true;
            }
            else
            {
                Log("Login cevap gelmedi: timeout CommandID: " + message.GetMessageHeader().CommandID);
                DisposeSocket();
                return false;
            }
        }

        public void Send(Message msg)
        {
            Log("Send() " + msg.GetMessageHeader().GetMsgID().ToString());

            MessageHeader h = msg.GetMessageHeader();

            // Sunucu tarafı ise
            if (h.isServerSideCommand())
            {
                // Sunucu tarafı ama disk'e yaz işaretli.
                if (MessageAttribute.GetMessageAttribute(Message.GetTypeFromID(h.CommandID)).SaveToQueue)
                {
                    _sendQueue.Enqueue(msg, true);
                }
                else
                {
                    _sendQueue.Enqueue(msg, false);
                }
            }
            else
            {
                // Kullanıcı tarafı
                _sendQueue.Enqueue(msg, true);
            }
        }

        private void StartSend()
        {
            Log("StartSend() #THREAD");
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartSendInternal));
        }

        private void StartSendInternal(object s)
        {
            if (_sendQueue.isCompleted)
                return;

            Message m;

            if (_sendQueue.Peek(out m))
            {
                try
                {
                    Log("BeginSend()");
                    _socket.BeginSend(m.Data, 0, m.Size, SocketFlags.None, 
                                        new AsyncCallback(FinishSend), 
                                        new Tuple<Socket, Message>(_socket, m));
                }
                catch(Exception ex)
                {
                    DisconnectSocket(ex);
                    return;
                }

                // ACK2
                if (_ackList.WaitForAck2(PredefinedCommands.Ack2.CreateAck2(m.GetMessageHeader()), 60000))
                {
                    _sendQueue.Dequeue(out m);
                    StartSendInternal(null);
                }
                else
                {
                    DisconnectSocket(null);
                    return;
                }
            }
        }

        private void FinishSend(IAsyncResult r)
        {
            Tuple<Socket, Message> t = (Tuple<Socket, Message>)r.AsyncState;
            Socket s = t.Item1;
            Message m = t.Item2;

            try
            {
                Log("EndSend()");
                s.EndSend(r);
            }
            catch(Exception ex)
            {
                DisconnectSocket(ex);
                return;
            }

            Log("Sent: " + m.GetMessageHeader().GetMsgID().ToString());
        }
    }
}
