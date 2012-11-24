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
        private volatile Socket _socket;

        public bool Connected
        {
            get { lock (_synch) { return _socket != null && _socket.Connected; } }
        }

        private bool DisposeSocket()
        {
            // Log("DisposeSocket()");

            lock (_synch)
            {
                _ackList.Completed();
                _sendQueue.Completed();

                if (_socket == null)
                    return false;

                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch { }

                _socket.Close();
                _socket = null;

                return true;
            }
        }

        //private void DisconnectSocket(Exception ex)
        //{
        //    Log("DisconnectSocket()");

        //    if (DisposeSocket())
        //        FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = ex });
        //}
    }
}
