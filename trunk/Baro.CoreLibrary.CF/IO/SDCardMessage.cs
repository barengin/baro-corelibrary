using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.IO
{
#if PocketPC || WindowsCE
    public sealed class SDCardMessage : Microsoft.WindowsCE.Forms.MessageWindow
    {
        private IntPtr WM_SD_INSERTION = new IntPtr(0x8000);
        private IntPtr WM_SD_REMOVAL = new IntPtr(0x8004);
        private const int WM_DEVICECHANGE = 0x219;

        public enum SDCardState
        {
            INSERTION,
            REMOVAL
        }

        public delegate void DelegateSDCardChange(SDCardState state);
        public event DelegateSDCardChange EventSDCardChange;

        protected override void WndProc(ref Microsoft.WindowsCE.Forms.Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                if (m.WParam == WM_SD_INSERTION)
                {
                    if (EventSDCardChange != null)
                    {
                        EventSDCardChange(SDCardState.INSERTION);
                    }
                }
                else if (m.WParam == WM_SD_REMOVAL)
                {
                    if (EventSDCardChange != null)
                    {
                        EventSDCardChange(SDCardState.REMOVAL);
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
#endif
}
