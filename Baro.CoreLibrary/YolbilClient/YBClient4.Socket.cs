using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
	partial class YBClient4
	{
        private Socket _socket;
        private LastActivity _lastActivity = new LastActivity();

        private volatile bool _exitControlThread = false;

        private void _doControlJob()
        {
            Log("Control thread started...");
            byte[] b = new byte[1];
            TimeSpan TO = new TimeSpan(0, 1, 0);

            while (!_exitControlThread)
            {
                if (_lastActivity.Peek() > TO)
                {
                    TSDisposeSocket();
                    FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new Exception("Timeout") });
                    _exitControlThread = true;
                }
                else
                {
                    Log("Control thread SLEEP..." + _lastActivity.Peek().ToString());
                    Thread.Sleep(3000);
                }
            }
            
            Log("Control thread disposed...");
        }

        private void CloseControlThread()
        {
            _exitControlThread = true;
        }

        private void CreateControlThread()
        {
            _exitControlThread = false;

            Thread t = new Thread(_doControlJob);
            t.IsBackground = true;
            t.Start();
        }

        private bool TSSocketIsAvailable()
        {
            lock (_synch)
            {
                return _socket != null;
            }
        }

        private void TSCreateSocket()
        {
            lock (_synch)
            {
                if (_socket != null)
                    throw new InvalidOperationException("Socket zaten yaratılmış");

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                _sendQueue.MakeUnCompleted();
            }
        }

        private void TSDisposeSocket()
        {
            lock (_synch)
            {
                if (_socket != null)
                {
                    _exitControlThread = true;
                    _sendQueue.Completed();

                    try { _socket.Shutdown(SocketShutdown.Both); }
                    catch { }

                    try { _socket.Close(); }
                    catch { }

                    _socket = null;
                }
                else
                {
                }
            }
        }
	}
}
