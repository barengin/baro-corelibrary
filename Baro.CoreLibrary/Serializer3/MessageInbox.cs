using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public class MessageInbox
    {
        public uint Inbox { get; set; }
        public DateTime ExpireDate { get; set; }

        public MessageInbox(uint inbox)
        {
            this.Inbox = inbox;
            this.ExpireDate = DateTime.Now.AddDays(7);
        }
    }
}
