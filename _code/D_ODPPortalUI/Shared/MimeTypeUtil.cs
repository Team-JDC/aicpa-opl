using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace D_ODPPortalUI.Shared
{
    /// <summary>
    /// Utility class contains static method checkType to determine Mime Type
    /// </summary>
    public class MimeTypeUtil
    {
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private static extern UInt32 FindMimeFromData(
            UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed,
            UInt32 dwMimeFlags,
            out UInt32 ppwzMimeOut,
            UInt32 dwReserved);

        public static string CheckType(string filePath)
        {
            var buffer = new byte[256];
            // grab the first 256 bytes on the file
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                if (fileStream.Length >= 256)
                {
                    fileStream.Read(buffer, 0, 256);
                }
                else
                {
                    fileStream.Read(buffer, 0, (int) fileStream.Length);
                }
            }
            try
            {
                UInt32 mimeType;
                UInt32 returnValue = FindMimeFromData(0, null, buffer, 256, null, 0, out mimeType, 0);
                var mimeTypePointer = new IntPtr(mimeType);
                string retType = Marshal.PtrToStringUni(mimeTypePointer);
                if (retType == "application/octet-stream")
                {
                    string contentType = string.Empty;
                    var fi = new FileInfo(filePath);
                    if (fi.Extension != null && fi.Extension.Length != 0)
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
                            if (null != ctrk)
                            {
                                contentType = (string) ctrk.GetValue("Content Type");
                            }

                            ctrk.Close();
                        }

                        if (contentType != string.Empty)
                        {
                            retType = contentType;
                        }
                    }
                }

                return retType;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}