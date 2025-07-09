using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared
{
    public static class Base64Tools
    {        
        public static string EncodeString(string input)
        {
            byte[] encodedByte = System.Text.ASCIIEncoding.ASCII.GetBytes(input);
            string base64Encoded = Convert.ToBase64String(encodedByte);
            return base64Encoded;
        }

        public static string DecodeString(string input)
        {
            byte[] data = Convert.FromBase64String(input);
            string decodedString = System.Text.Encoding.UTF8.GetString(data);
            return decodedString;
        }        
    }
}