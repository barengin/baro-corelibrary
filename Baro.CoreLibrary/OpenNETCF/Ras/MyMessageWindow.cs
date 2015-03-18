using System;
using Microsoft.WindowsCE.Forms;

namespace OpenNETCF.Net
{
	/// <summary>
	/// Summary description for MyMessageWindow.
	/// </summary>
	internal class MyMessageWindow: MessageWindow
	{
		public MyMessageWindow()
		{
		}

		public delegate bool BeforeMessageEventHandler(ref Message msg);
		public delegate void AfterMessageEventHandler(ref Message msg);
		public event BeforeMessageEventHandler BeforeMessageEvent;
		public event AfterMessageEventHandler AfterMessageEvent;

		protected override void WndProc(ref Message m)
		{
			if ( BeforeMessageEvent != null && BeforeMessageEvent(ref m) )
				return;
			base.WndProc (ref m);
			AfterMessageEvent(ref m);
		}

	}
}
