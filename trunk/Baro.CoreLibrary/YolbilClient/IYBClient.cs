﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.Serializer2;

namespace Baro.CoreLibrary.YolbilClient
{
    interface IYBClient
    {
        /// <summary>
        /// Sunucuya canlı bağlantı olup olmadığını gösterir.
        /// <remarks>Thread-Safe</remarks>
        /// </summary>
        bool Connected { get; }
        
        event EventHandler<ConnectedEventArgs> OnConnect;
        event EventHandler<DisconnectedEventArgs> OnDisconnect;
        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        void DeleteFromServer(MessageHeader header);
        void Send(Message msg);

        void Connect();
        void Disconnect();
    }
}
