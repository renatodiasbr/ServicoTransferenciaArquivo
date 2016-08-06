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
                    RSA.FromXmlString(File.ReadAllText(GetKeyInfoPath()));
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
            catch (ArgumentNullException)
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
                    RSA.FromXmlString(File.ReadAllText(GetKeyInfoPath()));
                    decryptedData = RSA.Decrypt(data, fOAEP);
                }
                return decryptedData;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public static string Decrypt(string data, bool fOAEP)
        {
            try
            {
                byte[] decryptedData = Decrypt(Convert.FromBase64String(data), fOAEP);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        private static string GetKeyInfoPath()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RSAKeyInfo:VirtualWebPath"]))
            {
                return ConfigurationManager.AppSettings["RSAKeyInfo"];
            }
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["RSAKeyInfo:VirtualWebPath"]))
            {
                return System.Web.HttpContext.Current.Server.MapPath((ConfigurationManager.AppSettings["RSAKeyInfo"]));
            }
            else
            {
                return ConfigurationManager.AppSettings["RSAKeyInfo"];
            }
        }
    }
}
