using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.Shared;

namespace MainUI.Shared
{
    /// <summary>
    /// Utility class contains static method checkType to determine Mime Type
    /// </summary>
    public class MimeTypeUtil
    {
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(
            UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed,
            UInt32 dwMimeFlags,
            out IntPtr ppwzMimeOut,
            UInt32 dwReserved);

        public static string CheckType(string filePath)
        {
            byte[] buffer = new byte[256];
            // grab the first 256 bytes on the file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                if (fileStream.Length >= 256)
                {
                    fileStream.Read(buffer, 0, 256);
                }
                else
                {
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                }
            }
            try
            {
                IntPtr mimeType;
                UInt32 result = FindMimeFromData(0, null, buffer, 256, null, 0, out mimeType, 0);
                string retType = string.Empty;
                retType = Marshal.PtrToStringUni(mimeType);
                if (retType == "application/octet-stream")
                {
                    string contentType = string.Empty;
                    FileInfo fi = new FileInfo(filePath);
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
                                contentType = (string)ctrk.GetValue("Content Type");
                            }

                            ctrk.Close();
                        }

                        if (contentType != string.Empty)
                        {
                            retType = contentType;
                        }
                    }
                }
                // (07/01/10) Benjamin Bytheway - This was a fix for firefox css requests.  It was using
                // Handlers/GetResource.ashx to grab the css files for the pages that it needed, but for 
                // some reason it was returning "text/plain" as the contentType.  That isn't good because
                // firefox needs a mime type of "text/css" in order to parse it as css.  So, I added this
                // else if statement to get that to work.
                else if (retType == "text/plain")
                {
                    string contentType = "text/plain";
                    FileInfo fi = new FileInfo(filePath);
                    if (fi.Extension != null && fi.Extension.Length != 0)
                    {
                        if (fi.Extension == ".css")
                        {
                            contentType = "text/css";
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