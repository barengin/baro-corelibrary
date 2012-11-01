using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Baro.CoreLibrary.Serializer2;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient3
    {
        private WaitHandle StartConnect()
        {
            if (Connected)
            {
                Disconnect().WaitOne();
            }

            DisposeSocket();

            _ackList.UnCompleted();
            // TODO: SendQueue

            AutoResetEvent connectedEvent = new AutoResetEvent(false);

            _socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.IP);
            _socket.BeginConnect(_settings.Address, new AsyncCallback(FinishConnect), new Tuple<Socket, AutoResetEvent>(_socket, connectedEvent));

            return connectedEvent;
        }

        private void FinishConnect(IAsyncResult r)
        {
            Tuple<Socket, AutoResetEvent> t = (Tuple<Socket, AutoResetEvent>)r.AsyncState;
            Socket s = t.Item1;
            AutoResetEvent e = t.Item2;

            try
            {
                s.EndConnect(r);
            }
            catch
            {
                DisposeSocket();                
                e.Set();
                return;
            }

            // StartReceive here!!!
            StartReceive();

            // Login here!!!
            SendAndWaitForAck(Message.Create(new MessageInfo(), _settings.Login, false, null));
            
            // Finish
            e.Set();
            FireOnConnect(new ConnectedEventArgs());
        }


        private WaitHandle StartDisconnect()
        {
            _ackList.Completed();
            // TODO: SendQueue

            DisposeSocket();
            return new AutoResetEvent(true);
        }
    }
}
