using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Baro.CoreLibrary.Serializer2;

namespace Baro.CoreLibrary.YolbilClient
{
    public partial class YolbilCF : Component
    {
        public event EventHandler<ConnectedEventArgs> OnConnected;
        public event EventHandler<DisconnectedEventArgs> OnDisconnected;
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        public YolbilCF()
        {
            InitializeComponent();
        }

        public YolbilCF(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected void FireOnConnected()
        {
            if (OnConnected != null)
            {
                OnConnected(this, new ConnectedEventArgs());
            }
        }

        protected void FireOnDisconnected(Exception e)
        {
            if (OnDisconnected != null)
            {
                OnDisconnected(this, new DisconnectedEventArgs() { DisconnectReason = e });
            }
        }

        protected void FireOnMessageReceived(MessageInternalHeader h, object message)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(this, new MessageReceivedEventArgs() { Header = h, Message = message });
            }
        }

        public bool Connected { get; set; }
    }
}
