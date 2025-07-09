using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

////This was provided by CPA.COM I put it here and changed Matt's AICPAEncryption.cs to use it
namespace AICPA.Destroyer.Shared
{

    interface IEncryption
    {
        string Encrypt(string cryptoKey);
        string Decrypt(string cryptoKey);
    }

    public class AESEncryption : IEncryption, IDisposable
    {
        RijndaelManaged rijndaelCipher;


        public AESEncryption(string key)
            : this(key, CipherMode.CBC, PaddingMode.PKCS7, 128, 128)
        {
        }

        public AESEncryption(string key, CipherMode cipherMode, PaddingMode paddingMode, int keySize, int blockSize)
        {
            rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = cipherMode;
            rijndaelCipher.Padding = paddingMode;
            rijndaelCipher.KeySize = keySize;
            rijndaelCipher.BlockSize = blockSize;

            byte[] RawKey = Encoding.UTF8.GetBytes(key);
            if (RawKey.Length > (keySize / 8))
            {
                byte[] ActualKey = new byte[(keySize / 8)];
                System.Array.Copy(RawKey, ActualKey, (keySize / 8));
                rijndaelCipher.Key = ActualKey;
                rijndaelCipher.IV = ActualKey;
            }
            else
            {
                rijndaelCipher.Key = RawKey;
                rijndaelCipher.IV = RawKey;
            }

            if (!rijndaelCipher.ValidKeySize(keySize))
                throw new ApplicationException("Invalid key size");
        }

        public string Encrypt(string plainText)
        {
            ICryptoTransform Transform = rijndaelCipher.CreateEncryptor();
            byte[] CiperBytes = Transform.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            Transform.Dispose();

            return Convert.ToBase64String(CiperBytes);
        }

        public string Decrypt(string encryptedText)
        {
            ICryptoTransform Transform = rijndaelCipher.CreateDecryptor();
            byte[] CiperBytes = Convert.FromBase64String(encryptedText);
            byte[] PlainTextInBytes = Transform.TransformFinalBlock(CiperBytes, 0, CiperBytes.Length);
            Transform.Dispose();

            return Encoding.UTF8.GetString(PlainTextInBytes);
        }

        public void Dispose()
        {
            if (rijndaelCipher != null)
                rijndaelCipher.Clear();
        }
    }

    public class TripleDESEncryption : IEncryption
    {
        private byte[] KeyInBytes;
        private byte[] IVInBytes;

        public TripleDESEncryption(string key, string IV)
        {
            KeyInBytes = ASCIIEncoding.ASCII.GetBytes(key);
            IVInBytes = ASCIIEncoding.ASCII.GetBytes(IV);
        }

        public string Encrypt(string clearText)
        {
            TripleDESCryptoServiceProvider TripleDesProv = new TripleDESCryptoServiceProvider();
            ICryptoTransform Encryptor = TripleDesProv.CreateEncryptor(KeyInBytes, IVInBytes);
            byte[] ClearTextInBytes = ASCIIEncoding.ASCII.GetBytes(clearText);
            byte[] EncryptedDataInBytes = Encryptor.TransformFinalBlock(ClearTextInBytes, 0, ClearTextInBytes.Length);

            return Convert.ToBase64String(EncryptedDataInBytes, 0, EncryptedDataInBytes.Length);
        }

        public string Decrypt(string encryptedText)
        {
            encryptedText = encryptedText.Replace(' ', '+');

            TripleDESCryptoServiceProvider TripleDesProv = new TripleDESCryptoServiceProvider();
            ICryptoTransform Decryptor = TripleDesProv.CreateDecryptor(KeyInBytes, IVInBytes);
            byte[] EncryptedTextInBytes = Convert.FromBase64String(encryptedText);
            byte[] DecryptedTextInBytes = Decryptor.TransformFinalBlock(EncryptedTextInBytes, 0, EncryptedTextInBytes.Length);

            return Encoding.ASCII.GetString(DecryptedTextInBytes, 0, DecryptedTextInBytes.Length);
        }
    }
}

