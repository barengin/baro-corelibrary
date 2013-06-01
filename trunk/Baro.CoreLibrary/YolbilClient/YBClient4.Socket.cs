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
        private volatile bool _readyToConnect = true;

        public bool ReadyToConnect
        {
            get { return _readyToConnect; }
        }

        private void _doControlJob()
        {
            _readyToConnect = false;

            Log("Control thread started...");
            byte[] b = new byte[1];
            TimeSpan TO = new TimeSpan(0, 1, 0);

            while (!_exitControlThread)
            {
                if (_lastActivity.Peek() > TO)
                {
                    try
                    {
                        _socket.Send(b, 0, 0, SocketFlags.None);
                    }
                    catch 
                    {
                        TSDisposeSocket();
                        FireOnDisconnect(new DisconnectedEventArgs() { DisconnectReason = new Exception("Timeout") });
                        _exitControlThread = true;
                        continue;
                    }

                    _lastActivity.Reset();
                }
                else
                {
                    // Log("Control thread SLEEP ... " + _lastActivity.Peek().ToString());
                    Thread.Sleep(3000);
                }
            }

            _readyToConnect = true;
            Log("Control thread disposed...");
        }

        private void CloseControlThread()
        {
            _exitControlThread = true;
        }

        private void CreateControlThread()
        {
            _exitControlThread = false;

            Thread _controlThread = new Thread(_doControlJob);
            _controlThread.IsBackground = true;
            _controlThread.Start();
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
                _ackList = new ACKList();

                _buffer.Clear();
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
                    _ackList.Completed();

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
