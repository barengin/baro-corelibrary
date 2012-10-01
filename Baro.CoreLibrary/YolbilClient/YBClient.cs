using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    public enum ClientState
    {
        Disconnected,
        Connecting,
        Connected
    }

    public sealed class YBClient
    {
        private object _lock = new object();
        private volatile ClientState _state = ClientState.Disconnected;
        private volatile ConnectionSettings _settings;
        private volatile Socket _socket;

        public event Action<YBClient> OnConnected;
        public event Action<YBClient, Exception> OnDisconnected;
        public event Action<YBClient, object> OnMessageReceived;
        public event Action<YBClient, DateTime> OnServerTime;

        public YBClient(ConnectionSettings settings)
        {
            this.Settings = settings;
        }

        public ConnectionSettings Settings { 
            get { return _settings; }
            private set
            {
                _settings = value;
            }
        }

        public void Connect()
        {
        }

        public void Disconnect()
        {
        }
    }
}
