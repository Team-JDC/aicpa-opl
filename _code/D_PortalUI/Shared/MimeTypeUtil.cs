using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AICPA.Destroyer.UI.Portal.Shared
{
	/// <summary>
	/// Utility class contains static method checkType to determine Mime Type
	/// </summary>
	public class MimeTypeUtil 
	{
		[DllImport(@"urlmon.dll", CharSet=CharSet.Auto)]
		private extern static System.UInt32 FindMimeFromData(
			System.UInt32 pBC,
			[MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
			[MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer, 
			System.UInt32 cbSize,
			[MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
			System.UInt32 dwMimeFlags,
			out System.UInt32 ppwzMimeOut,
			System.UInt32 dwReserved);
          
		public static string CheckType(string filePath) 
		{
			byte[] buffer = new byte[256];
			// grab the first 256 bytes on the file
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) 
			{
				if (fileStream.Length >=256) 
				{
					fileStream.Read(buffer,0,256);
				}
				else
				{
					fileStream.Read(buffer,0,(int)fileStream.Length);
				}
			}
			try 
			{
				System.UInt32 mimeType;
				System.UInt32 returnValue = FindMimeFromData( 0, null, buffer, 256, null, 0, out mimeType, 0);
				System.IntPtr mimeTypePointer = new IntPtr(mimeType);
				string retType = Marshal.PtrToStringUni(mimeTypePointer);
				if(retType == "application/octet-stream")
				{
					string contentType = string.Empty; 
					FileInfo fi = new FileInfo(filePath); 
					if(fi.Extension != null && fi.Extension.Length != 0) 
					{
						if (fi.Extension == ".doc")
						{
							contentType = "application/msword";
						}
						else if (fi.Extension == ".xls")
						{
							contentType = "application/msexcel";
						}		
						else 
						{
							RegistryKey ctrk = Registry.ClassesRoot.OpenSubKey(fi.Extension); 
							if(null != ctrk)
							{ 
								contentType = (string)ctrk.GetValue("Content Type"); 
							}
							
							ctrk.Close();
						}
						
                        if(contentType != string.Empty)
						{
							retType = contentType;
						}
					}
				}

				return retType;
			}
			catch(Exception ex) 
			{
				return ex.Message;
			}
		}    
	}
}