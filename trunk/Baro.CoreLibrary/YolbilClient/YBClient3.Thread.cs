using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient3
    {
        private volatile Socket _socket;

        public bool Connected
        {
            get { lock(_synch) { return _socket != null && _socket.Connected; } }
        }

        private bool DisposeSocket(Socket s)
        {
            if (s == null)
                return false;

            try
            {
                s.Shutdown(SocketShutdown.Both);
            }
            catch { }

            s.Close();

            return true;
        }

        private void ConnectSocket()
        {
            DisconnectSocket();

            lock (_synch)
            {
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
            bool r;

            lock (_synch)
            {
                r = DisposeSocket(_socket);
                _socket = null;
            }

            if (r)
                FireOnDisconnect(new DisconnectedEventArgs());
        }

        private void PauseTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void ResumeTimer()
        {
            _timer.Change(60000, 60000);
        }

        private void timerLoop(object s)
        {
            PauseTimer();

            if (Connected)
            {
                Send(Message.Create(new MessageInfo(), new PredefinedCommands.KeepAlive(), false, null));
            }
            else
            {
                ConnectSocket();

                if (Connected)
                {
                    StartReceive();
                    SendAndWaitForAck(Message.Create(new MessageInfo(), _settings.Login, false, null));
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartSend));
                }
            }

            ResumeTimer();
        }
    }
}
