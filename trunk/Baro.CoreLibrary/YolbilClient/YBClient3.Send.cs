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
        private ACKList _ackList = new ACKList();

        private bool SendAndWaitForAck(Message message)
        {
            Log("SendAndWaitForACK2: " + message.GetMessageHeader().CommandID + "," + message.GetMessageHeader().GetMsgID().ToString());

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
        }
    }
}
