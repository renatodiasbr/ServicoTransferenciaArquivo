using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicoTransferenciaArquivo.Library
{
    public class RSAEncryption
    {
        public static byte[] Encrypt(byte[] data, bool fOAEP)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(File.ReadAllText(ConfigurationManager.AppSettings["RSAKeyInfo"]));
                    encryptedData = RSA.Encrypt(data, fOAEP);
                }
                return encryptedData;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public static string Encrypt(string data, bool fOAEP)
        {
            try
            {
                byte[] encryptedData = Encrypt(Encoding.UTF8.GetBytes(data), fOAEP);
                return Convert.ToBase64String(encryptedData);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public static byte[] Decrypt(byte[] data, bool fOAEP)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(File.ReadAllText(ConfigurationManager.AppSettings["RSAKeyInfo"]));
                    decryptedData = RSA.Decrypt(data, fOAEP);
                }
                return decryptedData;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public static string Decrypt(string DataToDecrypt, bool fOAEP)
        {
            try
            {
                byte[] decryptedData = Decrypt(Convert.FromBase64String(DataToDecrypt), fOAEP);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}
