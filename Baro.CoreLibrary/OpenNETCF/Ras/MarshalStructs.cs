using System;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.Net
{
	/// <summary>
	/// Summary description for MarshalStructs.
	/// </summary>
	internal class MarshalStructs 
	{
		public MarshalStructs()
		{
			//
			// TODO: Add constructor logic here
			//
		}

	}


	internal class RASENTRYNAME : AdvancedMarshaler
	{
		public int dwSize;
		//CHAR szDeviceType[ RAS_MaxDeviceType + 1 ];
		[CustomMarshalAs(SizeConst=(RAS_MaxEntryName + 1)*2)]
		public string szEntryName;
		
		const int RAS_MaxEntryName = 20;
		

		public RASENTRYNAME()
		{
			dwSize = GetSize();
			this.data = new byte[dwSize];
			//Write the dwSize into the byte array
			//this.Serialize();
		}
	} 
}
