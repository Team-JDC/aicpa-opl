using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace ExamsCandidate.Shared
{
    /// <summary>
    /// Built
    /// </summary>
    public class AICPAEncryption
    {
        static string AICPA_ENCRYPTION_KEY = "AicpaEncryptionKey";
        
        AESEncryption _AESEncryption;
        
        public AESEncryption Encryption { 
            get {
                if (_AESEncryption == null)
                {
                    _AESEncryption = new AESEncryption(ConfigurationManager.AppSettings[AICPA_ENCRYPTION_KEY]);
                }
                return _AESEncryption;
            }
        }

        ~AICPAEncryption()
        {
            _AESEncryption = null;
        }

        public string Encrypt(string plainText)
        {
            return Encryption.Encrypt(plainText);
        }

        public string Decrypt(string encryptedText)
        {
            try
            {
                return Encryption.Decrypt(encryptedText);
            }
            catch (Exception e)
            {
                return "EcncryptedPassswordInputIsInvalid";
            }
        }

    }
}

