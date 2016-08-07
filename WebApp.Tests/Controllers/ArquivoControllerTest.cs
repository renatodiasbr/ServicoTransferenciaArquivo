using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicoTransferenciaArquivo.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServicoTransferenciaArquivo.WebApp.Tests.Controllers
{
    [TestClass]
    public class ArquivoControllerTest
    {
        [TestMethod]
        public void UploadTest()
        {
            WebClient client = new WebClient();
            client.QueryString["destinationPath"] = Path.GetFullPath(@".\..\..\Arquivos");
            client.QueryString["fileName"] = "doc.pdf";
            var responseByte = client.UploadFile("http://localhost:11532/Arquivo/Upload", @".\..\..\Arquivos\ArquivoUpload.pdf");
            var responseString = client.Encoding.GetString(responseByte);
            Assert.IsTrue(Convert.ToBoolean(responseString));
        }

        [TestMethod]
        public void GetMd5HashTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = Path.GetFullPath(@".\..\..\Arquivos\doc.pdf");
            var responseByte = client.DownloadData("http://localhost:11532/Arquivo/GetMd5Hash");
            var responseString = client.Encoding.GetString(responseByte);
            Assert.IsTrue(string.Equals(responseString, MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoUpload.pdf")));
        }

        [TestMethod]
        public void DownloadTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = Path.GetFullPath(@".\..\..\Arquivos\doc.pdf");
            client.QueryString["ticks"] = HttpUtility.UrlEncode(RSAEncryption.Encrypt(DateTime.Now.Ticks.ToString(), false));
            client.DownloadFile("http://localhost:11532/Arquivo/Download", @".\..\..\Arquivos\ArquivoDownload.pdf");
            Assert.AreEqual(MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoUpload.pdf"), MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoDownload.pdf"));
        }

        [ExpectedException(typeof(Exception), AllowDerivedTypes=true)]
        [TestMethod]
        public void DownloadInvalidTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = Path.GetFullPath(@".\..\..\Arquivos\doc.pdf");
            client.QueryString["ticks"] = HttpUtility.UrlEncode(RSAEncryption.Encrypt(DateTime.Now.Ticks.ToString(), false));
            client.DownloadFile("http://localhost:11532/Arquivo/Download", @".\..\..\Arquivos\ArquivoDownload.pdf");
            client.DownloadFile("http://localhost:11532/Arquivo/Download", @".\..\..\Arquivos\ArquivoDownload.pdf");
            Assert.AreEqual(MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoUpload.pdf"), MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoDownload.pdf"));
        }

        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        [TestMethod]
        public void DownloadInvalidTicksTest()
        {
            WebClient client = new WebClient();
            client.QueryString["filePath"] = Path.GetFullPath(@".\..\..\Arquivos\doc.pdf");
            client.QueryString["ticks"] = "AaSsDdFgZxCc";
            client.DownloadFile("http://localhost:11532/Arquivo/Download", @".\..\..\Arquivos\ArquivoDownload.pdf");
            Assert.AreEqual(MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoUpload.pdf"), MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoDownload.pdf"));
        }
    }
}
