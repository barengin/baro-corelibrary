using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsCE.Forms;
using System.Diagnostics;

namespace OpenNETCF.Net
{
    internal class MessageHook : MessageWindow
    {        
        private RasEntry entry;

        public MessageHook(RasEntry entry)
        {
            this.entry = entry;
        }

        protected override void WndProc(ref Microsoft.WindowsCE.Forms.Message msg)
        {
            RasConnState state = (RasConnState)msg.WParam;
            RasError error = (RasError)msg.LParam;

            entry.OnRasStatus(msg.Msg, state, error);
            entry.OnRasError(error);

            // Debug.WriteLine(string.Format("RAS Message: state='{0}' error='{1}'", state.ToString(), error.ToString()));
        }
    }
}
