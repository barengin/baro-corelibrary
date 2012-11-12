using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Serializer2;
using System.Net.Sockets;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient4
    {
        private void _doSendJob()
        {
            bool quit = false;
            Log("Send thread created...");

            while (!quit)
            {
                if (_sendQueue.isCompleted)
                {
                    Log("SendQueue is marked to quit...");
                    quit = true;
                    continue;
                }

                Message m;

                if (_sendQueue.Peek(out m))
                {
                    try
                    {
                        Log("Sending...");
                        _socket.Send(m.Data, 0, m.Size, SocketFlags.None);
                        Log("Sent...");
                    }
                    catch(Exception ex)
                    {
                        TSDisposeSocket();
                        FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
                        quit = true;
                        continue;
                    }

                    // ACK2
                    if (_ackList.WaitForAck2(PredefinedCommands.Ack2.CreateAck2(m.GetMessageHeader()), 60000))
                    {
                        _sendQueue.Dequeue(out m);
                    }
                    else
                    {
                        TSDisposeSocket();
                        FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new Exception("ACK Timeout") });
                        quit = true;
                        continue;
                    }
                }
            }

            Log("Send thread disposed...");
        }

        private void CreateSendThread()
        {
            Thread t = new Thread(_doSendJob);
            t.IsBackground = true;
            t.Start();
        }
    }
}
