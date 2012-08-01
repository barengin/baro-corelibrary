using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenNETCF.Net
{
	/// <summary>
	/// Summary description for Native.
	/// </summary>
	internal class Native
	{
        public enum Error
        {
            Success = 0,
            InvalidHandle = 6,
            AlreadyExists = 183
        }

		#region API functions Declarations

		internal const int RASBASE = 600;
		
		[DllImport("coredll.dll")]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("coredll.dll")]
		internal static extern IntPtr LoadLibrary(string lpModuleName);

//		[DllImport("rasconnect.dll", EntryPoint="EnumEntries")]
//		internal static extern int RasEnumEntries(string value, ref int count);
//			
//		[DllImport("rasconnect.dll", EntryPoint="Dial")]
//		internal static extern int RasDialCust(string sName, ref int connection);
//
//		[DllImport("rasconnect.dll", EntryPoint="DialUser")]
//		internal static extern int RasDialUser(string sName, string sUserName, string sPassword, string sPhoneNumber, string sDomain, ref int connection);
//
//		[DllImport("rasconnect.dll", EntryPoint="HangUp")]
//		internal static extern int	RasHangUp(int connection);
//
//		[DllImport("rasconnect.dll", EntryPoint="GetConnectStatus")]
//		internal static extern int RasGetConnectStatus(int connection);
//			
//		[DllImport("rasconnect.dll", EntryPoint="CreateRasEntry")]
//		internal static extern int CreateRasEntry(string lpszName, 
//			string lpszLogin, 
//			string lpszPassword, 
//			string lpszPhoneNumber,
//			int dwCountryCode,
//			string lpszAreaCode,
//			string lpszDeviceName,
//			string lpszIPAddress,
//			string lpszDNS);
//			
//		[DllImport("rasconnect.dll", EntryPoint="EnumDev")]
//		internal static extern int RasEnumDevicesCust(string lpstrDevices, ref int count);

		#endregion

		#region Interop stuff

		[DllImport("coredll")]
		internal extern static RasError RasDial(
			IntPtr dialExtensions, 
			string phoneBookPath , 
			byte[] rasDialParam , 
			uint NotifierType, 
			IntPtr notifier, 
			ref int pRasConn );


		[DllImport("coredll")]
		internal extern static int RasHangUp(
			IntPtr Session );

		[DllImport("coredll")]
		internal extern static Error RasRenameEntry(
			string lpszPhonebook, 
			string lpszOldEntry, 
			string lpszNewEntry );

		[DllImport("coredll")]
		internal extern static int RasDeleteEntry(
			string lpszPhonebook, 
			string lpszEntry );


		[DllImport("coredll")]
		internal extern static Error RasValidateEntryName(
			string lpszPhonebook, 
			string lpszEntry );


		[DllImport("coredll")]
		internal extern static int RasEnumEntries(string sReserved, string sPhoneBook,  byte[] pEntries, ref int lpcb, out int lpcEntries);

		[DllImport("coredll")]
		internal extern static int RasEnumConnections(
			byte[] lprasconn, 
			ref int lpcb, 
			ref int lpcConnections );

		[DllImport("coredll")]
		internal extern static int RasGetEntryDialParams(
			string lpszPhoneBook, 
			byte[] lpRasDialParams, 
			ref int lpfPassword );

		[DllImport("coredll")]
		internal extern static int RasSetEntryDialParams(
			string lpszPhoneBook, 
			byte[] lpRasDialParams, 
			int fRemovePassword );


		[DllImport("coredll")]
		internal extern static int RasEnumDevices( 
			byte[] lpRasDevinfo, 
			ref int lpcb, 
			ref int lpcDevices );

		[DllImport("coredll")]
		internal extern static Error RasGetEntryProperties(
			string lpszPhoneBook, 
			string szEntry, 
			byte[] lpbEntry, 
			ref int lpdwEntrySize, 
			byte[] DevCfg, 
			ref int lpdwSize );

		[DllImport("coredll")]
		internal extern static Error RasSetEntryProperties(
			string lpszPhoneBook, 
			string szEntry , 
			byte[] lpbEntry, 
			int dwEntrySize, 
			byte[] lpb, 
			int dwSize );

		[DllImport("coredll")]
		internal extern static int RasDevConfigDialogEdit(
			string szDeviceName,
			string szDeviceType,
			IntPtr hWndOwner,
			byte[] lpDeviceConfigIn,
			int dwSize,
			byte[] lpDeviceConfigOut
			);

		[DllImport("RasConnect.dll")]
		internal extern static int GetDialParamsSize(string name);

		[DllImport("coredll")]
		internal extern static Error RasGetConnectStatus(
			IntPtr rasconn, 
			byte[] lprasconnstatus );


		#endregion

	}
}
