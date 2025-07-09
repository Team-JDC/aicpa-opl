using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace OKTAAuth.Test
{
    [TestClass]
    public class AICPAEncryptionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string value = "watsonda+2015-04-14@knowlysis.com";
            AICPA.Destroyer.Shared.AICPAEncryption ae = new AICPA.Destroyer.Shared.AICPAEncryption();
            string encryptedstr = ae.Encrypt(value);
            Assert.AreNotEqual(value, encryptedstr);            
            string decrypt = ae.Decrypt(encryptedstr);
            Assert.AreEqual(decrypt, value);
        }

        [TestMethod]
        public void Test_Invalid_Key()
        {
            string encValue = "ejCW3kx863xAPLyMsYshSxVHvZBeqvo/nQEnMmb/K6BeRFL0eo1dsZk7YeI0qvZl";
            string value = "watsonda+2015-04-14@knowlysis.com";
            string key = ConfigurationManager.AppSettings["AicpaEncryptionKey"];
            string newkey = key.Substring(0, 3) + "A" + key.Substring(4);
            AICPA.Destroyer.Shared.AESEncryption enc = new AICPA.Destroyer.Shared.AESEncryption(newkey);
            try {
                string decValue = enc.Decrypt(encValue);
                Assert.AreNotEqual(decValue, value);
                Assert.Fail("Should have failed");
            } catch {
                //
            }

        }
        [TestMethod]
        public void Encrypt_Some()
        {
            //   string value = "AICPAResource"; //JM0fr5cO+dwwQz3FuRfAaA==
            string value = "tbtarter"; //TIFD2Zdh0+afYAF0bTCVpw==
            

            AICPA.Destroyer.Shared.AICPAEncryption ae = new AICPA.Destroyer.Shared.AICPAEncryption();
            string encryptedstr = ae.Encrypt(value);
            Assert.AreNotEqual(value, encryptedstr);
            string decrypt = ae.Decrypt(encryptedstr);
            Assert.AreEqual(decrypt, value);

        }
        
    }
}
