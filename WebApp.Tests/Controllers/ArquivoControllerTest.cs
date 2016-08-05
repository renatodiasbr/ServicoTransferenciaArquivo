using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicoTransferenciaArquivo.WebApp.Tests.Controllers
{
    [TestClass]
    public class ArquivoControllerTest
    {
        [TestMethod]
        public void UploadTest()
        {
            WebClient client = new WebClient();
            client.QueryString["destinationPath"] = @"Arquivos";
            client.QueryString["fileName"] = "doc.pdf";
            var responseByte = client.UploadFile("http://localhost:11532/Arquivo/Upload", @".\Arquivos\ArquivoUpload.pdf");
            var responseString = client.Encoding.GetString(responseByte);
            Assert.IsTrue(Convert.ToBoolean(responseString));
        }

        [TestMethod]
        public void GetMd5HashTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = @"Arquivos\doc.pdf";
            var responseByte = client.DownloadData("http://localhost:11532/Arquivo/GetMd5Hash");
            var responseString = client.Encoding.GetString(responseByte);
            Assert.IsTrue(string.Equals(responseString, GetMd5Hash(@".\Arquivos\ArquivoUpload.pdf")));
        }

        [TestMethod]
        public void DownloadTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = @"Arquivos\doc.pdf";
            client.DownloadFile("http://localhost:11532/Arquivo/Download", @".\Arquivos\ArquivoDownload.pdf");
            Assert.AreEqual(GetMd5Hash(@".\Arquivos\ArquivoUpload.pdf"), GetMd5Hash(@".\Arquivos\ArquivoDownload.pdf"));
        }

        [TestMethod]
        public void EncryptTest()
        {
            try
            {
                //Create a UnicodeEncoder to convert between byte array and string.
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //Create byte arrays to hold original, encrypted, and decrypted data.
                byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
                byte[] encryptedData;
                byte[] decryptedData;

                //Create a new instance of RSACryptoServiceProvider to generate
                //public and private key data.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                }
            }
            catch (ArgumentNullException)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine("Encryption failed.");

            }
        }

        private byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    //RSA.ImportParameters(RSAKeyInfo);
                    RSA.FromXmlString("<RSAKeyValue><Modulus>6+NFDeI98ai5O/qvgcdY3LSk9FLR2Ou4J8gjiILwHnzPLTjWGJEpknzgZC424nnUC1olPYFJCMdm1h6pkSX3yVjpD9ufWuacpNjRp3t4QquKWxHTOHWVuZtcZRV5JXYHHSxeDIo+9u3oTYY1/dTcBpVUy68uzk3G06eT3sZz0hM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        private byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    //RSA.ImportParameters(RSAKeyInfo);
                    RSA.FromXmlString("<RSAKeyValue><Modulus>6+NFDeI98ai5O/qvgcdY3LSk9FLR2Ou4J8gjiILwHnzPLTjWGJEpknzgZC424nnUC1olPYFJCMdm1h6pkSX3yVjpD9ufWuacpNjRp3t4QquKWxHTOHWVuZtcZRV5JXYHHSxeDIo+9u3oTYY1/dTcBpVUy68uzk3G06eT3sZz0hM=</Modulus><Exponent>AQAB</Exponent><P>8zhFqMsKkvaQJJEgoYnrvGsz/zjQ9CAnqCs6Ja9LTLgN/6TRn3Fx0s7rWBzGZ1AFcPTlfkE3wM41fWNS6bKEnQ==</P><Q>+EhedgCV/u6YWFMk2IwwN9cszSgRHJ5R3/N1gWudbW81d9HA4YenYhOZKqRtdmtOH62LLkYhonDSJQ8hGUf6bw==</Q><DP>mbLa2nMNCJhFuMX5j/u/e/9nCYcXDN7xEKXYhg4DkMTTG9VFHvQq0OQv9yuf+ZOpDJvGFwYRvIbTaJyGveYxRQ==</DP><DQ>ZTY7K6d5ff8No5PhRVWAooLZBJj8wMnZXo4ErMmN13qqNToQgt+l4FlU6wk0hj2gD1HTlv2H4IxVXj4YpBGviQ==</DQ><InverseQ>lfGqdJH83ZUF+FUdd82zyHi6fzh9ztgyJHTCSYMFDDMRYeMNx0X7CL+84vkeLoo43E+b/dWZT3vNH6aMl78sLw==</InverseQ><D>QOYrhpFbd5RzOkRTa0JceYSd6bark6Hu7csLs7BKviiam5eKmAHATVRcLFPmt9LK+0CXBGAApxCtg3W2M4AdttmsJBA0tIxAW0+zdQZqabp9bCcVOR3PdwqULMmr1cve37f9A79vYHxaUh7IUBTVA0vkglOtnOY17HTw573WMHk=</D></RSAKeyValue>");

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

        private string GetMd5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
