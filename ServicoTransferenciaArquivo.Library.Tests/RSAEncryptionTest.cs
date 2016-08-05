using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServicoTransferenciaArquivo.Library.Tests
{
    [TestClass]
    public class RSAEncryptionTest
    {
        [TestMethod]
        public void EncryptTest()
        {
            var data = DateTime.Now.Ticks.ToString();
            var dataEncrypted = RSAEncryption.Encrypt(data, false);
            var dataDecrypted = RSAEncryption.Decrypt(dataEncrypted, false);
            Assert.AreEqual(data, dataDecrypted);
        }
    }
}
