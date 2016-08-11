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
            var client = new ServiceClient();
            var target = client.Upload(@".\..\..\Arquivos\ArquivoUpload.pdf", Path.GetFullPath(@".\..\..\Arquivos"), "doc.pdf");
            Assert.IsTrue(target);
        }

        [TestMethod]
        public void GetMd5HashTest()
        {
            var client = new ServiceClient();
            var target = client.GetMd5Hash(Path.GetFullPath(@".\..\..\Arquivos\doc.pdf"));
            Assert.IsTrue(string.Equals(target, MD5Encryption.GetMd5Hash(@".\..\..\Arquivos\ArquivoUpload.pdf")));
        }

        [TestMethod]
        public void DownloadTest()
        {
            var client = new ServiceClient();
            client.Download(Path.GetFullPath(@".\..\..\Arquivos\doc.pdf"), @".\..\..\Arquivos\ArquivoDownload.pdf");
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
