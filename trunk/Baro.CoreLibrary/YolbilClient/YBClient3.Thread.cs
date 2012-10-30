using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient3
    {
        private volatile Socket _socket;

        public bool Connected
        {
            get { return _socket != null && _socket.Connected; }
        }

        private void DisposeSocket(Socket s)
        {
            if (s == null)
                return;

            try
            {
                s.Shutdown(SocketShutdown.Both);
            }
            catch { }

            s.Close();
        }

        private void ConnectSocket()
        {
            lock (_synch)
            {
                DisconnectSocket();

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                try
                {
                    _socket.Connect(_settings.Address);
                }
                catch
                {
                    DisconnectSocket();
                    return;
                }
            }

            FireOnConnect(new ConnectedEventArgs());
        }

        private void DisconnectSocket()
        {
            lock (_synch)
            {
                DisposeSocket(_socket);
            }
            
            FireOnDisconnect(new DisconnectedEventArgs());
        }

        private void timerLoop(object s)
        {

        }
    }
}
