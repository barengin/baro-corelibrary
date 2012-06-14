using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Baro.CoreLibrary
{
#if PocketPC || WindowsCE
    using Microsoft.WindowsCE.Forms;
    using System.Windows.Forms;

    public sealed class NotifyIcon : IDisposable
    {
        // Fields
        private NOTIFYICONDATA data;
        private bool doubleclick;
        private Icon icon;
        private static uint id = 9;
        private NotifyIconMessageWindow messageWindow;
        private bool visible;

        // Events
        public event EventHandler Click;

        public event EventHandler DoubleClick;

        public event MouseEventHandler MouseDown;

        public event MouseEventHandler MouseUp;

        // Methods
        public NotifyIcon()
        {
            this.messageWindow = new NotifyIconMessageWindow(this);
            this.data = new NOTIFYICONDATA();
            this.data.cbSize = Marshal.SizeOf(this.data);
            this.data.uID = id++;
            this.data.uCallbackMessage = 0x4e;
            this.data.hWnd = this.messageWindow.Hwnd;
            this.data.uFlags = NIF.MESSAGE;
        }

        public void Dispose()
        {
            this.Visible = false;
            
            if (this.icon != null)
            {
                this.icon.Dispose();
                this.icon = null;
            }
        }

        private void OnClick(EventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }

        private void OnDoubleClick(EventArgs e)
        {
            if (this.DoubleClick != null)
            {
                this.DoubleClick(this, e);
            }
        }

        private void OnMouseDown(MouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }

        private void OnMouseUp(MouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                this.MouseUp(this, e);
            }
            if (!this.doubleclick)
            {
                this.OnClick(new EventArgs());
            }
            this.doubleclick = false;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool Shell_NotifyIcon(NIM dwMessage, ref NOTIFYICONDATA lpData);

        // Properties
        public Icon Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
                this.data.hIcon = this.icon.Handle;
                this.data.uFlags |= NIF.ICON;
                if (this.visible)
                {
                    Shell_NotifyIcon(NIM.MODIFY, ref this.data);
                }
            }
        }

        public string Text
        {
            get
            {
                return this.data.szTip;
            }
            set
            {
                if (value.Length > 0x40)
                {
                    throw new ArgumentException("value", "Text must be 64 characters or less");
                }
                this.data.szTip = value;
                this.data.uFlags |= NIF.TIP;
                if (this.visible)
                {
                    Shell_NotifyIcon(NIM.MODIFY, ref this.data);
                }
            }
        }

        public bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                if (this.visible != value)
                {
                    if (value)
                    {
                        if (!Shell_NotifyIcon(NIM.ADD, ref this.data))
                        {
                            throw new ExternalException("Error adding NotifyIcon");
                        }
                        this.visible = true;
                    }
                    else
                    {
                        if (!Shell_NotifyIcon(NIM.DELETE, ref this.data))
                        {
                            throw new ExternalException("Error deleting NotifyIcon");
                        }
                        this.visible = false;
                    }
                }
            }
        }

        // Nested Types
        [Flags]
        private enum NIF : uint
        {
            ICON = 2,
            MESSAGE = 1,
            TIP = 4
        }

        private enum NIM : uint
        {
            ADD = 0,
            DELETE = 2,
            MODIFY = 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uID;
            public NotifyIcon.NIF uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string szTip;
        }

        private sealed class NotifyIconMessageWindow : MessageWindow
        {
            // Fields
            private NotifyIcon parent;

            // Methods
            internal NotifyIconMessageWindow(NotifyIcon parent)
            {
                this.parent = parent;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x4e)
                {
                    switch (((int)m.LParam))
                    {
                        case 0x201:
                            this.parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                            break;

                        case 0x202:
                            this.parent.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                            break;

                        case 0x203:
                            this.parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
                            this.parent.doubleclick = true;
                            this.parent.OnDoubleClick(new EventArgs());
                            break;

                        case 0x204:
                            this.parent.OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
                            break;

                        case 0x205:
                            this.parent.OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
                            break;
                    }
                }
                base.WndProc(ref m);
            }
        }
    }
#endif
}
