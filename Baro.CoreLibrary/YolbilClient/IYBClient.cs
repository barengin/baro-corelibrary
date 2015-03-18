using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    interface IYBClient: IDisposable
    {
        /// <summary>
        /// Sunucuya canlı bağlantı olup olmadığını gösterir.
        /// </summary>
        bool Connected { get; }
        
        event EventHandler<ConnectedEventArgs> OnConnect;
        event EventHandler<DisconnectedEventArgs> OnDisconnect;
        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        void DeleteFromServer(MessageHeader header);
        void Send(Message msg);

        WaitHandle Connect();
        WaitHandle Disconnect();
    }
}
