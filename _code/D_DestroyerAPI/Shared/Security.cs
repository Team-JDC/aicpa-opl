using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Configuration;

namespace AICPA.Destroyer.Shared
{
    public static class Security
    {

        public static string GetEncryptionKey()
        {
            //EncryptionKey
            if (ConfigurationManager.AppSettings["EncryptionKey"] == null)
                throw new Exception("Invalid Settings.  EncryptionKey setting missing");
            return ConfigurationManager.AppSettings["EncryptionKey"];
        }

        //public static string EncodePassword(string originalPassword)
        //{
        //    //Declarations
        //    Byte[] originalBytes;
        //    Byte[] encodedBytes;
        //    MD5 md5;

        //    //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
        //    md5 = new MD5CryptoServiceProvider();
        //    originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
        //    encodedBytes = md5.ComputeHash(originalBytes);

        //    //Convert encoded bytes back to a 'readable' string
        //    return BitConverter.ToString(encodedBytes);
        //}


        public static string EncryptString(string toEncrypt, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            var encrypted = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            return encrypted;
        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        public static string DecryptString(string cipherString, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


    }
}
