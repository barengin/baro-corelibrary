using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;
using System.Net.Sockets;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient3
    {
        private SendQueue _queue;

        private AutoResetEvent _sendEvent = new AutoResetEvent(false);

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

        private void SendCallback(IAsyncResult r)
        {
            State state = (State)r.AsyncState;

            try
            {
                state._s.EndSend(r);
            }
            catch
            {
                DisconnectSocket();
                return;
            }

            Message m;
            _queue.Dequeue(out m);

            StartSend(null);
        }

        private void StartSend(object s)
        {
            Message m;

            if (Connected)
            {
                if (_queue.Peek(out m))
                {
                    State state = new State() { _s = _socket };

                    try
                    {
                        _socket.BeginSend(m.Data, 0, m.Size, SocketFlags.None,
                            new AsyncCallback(SendCallback), state);
                    }
                    catch
                    {
                        DisconnectSocket();
                    }
                }
                else
                {
                    _sendEvent.WaitOne();
                }
            }
        }
    }
}
