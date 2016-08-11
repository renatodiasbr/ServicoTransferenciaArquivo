using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServicoTransferenciaArquivo.Library
{
    public class ServiceClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            webRequest.Timeout = 1000 * 60 * 60;
            return webRequest;
        }

        public bool Upload(string sourceFilePath, string destinationPath, string destinationFileName)
        {
            QueryString.Clear();
            QueryString["destinationPath"] = destinationPath;
            QueryString["fileName"] = destinationFileName;
            QueryString["ticks"] = HttpUtility.UrlEncode(RSAEncryption.Encrypt(DateTime.Now.Ticks.ToString(), false));
            var responseByte = UploadFile(url + "/Arquivo/Upload", sourceFilePath);
            var responseString = Encoding.GetString(responseByte);
            return Convert.ToBoolean(responseString);
        }

        public void Download(string sourceFilePath, string destinationFilePath)
        {
            QueryString.Clear();
            QueryString["filePath"] = sourceFilePath;
            QueryString["ticks"] = HttpUtility.UrlEncode(RSAEncryption.Encrypt(DateTime.Now.Ticks.ToString(), false));
            DownloadFile(url + "/Arquivo/Download", destinationFilePath);
        }

        public string GetMd5Hash(string filePath)
        {
            QueryString.Clear();
            QueryString["filePath"] = filePath;
            QueryString["ticks"] = HttpUtility.UrlEncode(RSAEncryption.Encrypt(DateTime.Now.Ticks.ToString(), false));
            var responseByte = DownloadData(url + "/Arquivo/GetMd5Hash");
            return Encoding.GetString(responseByte);
        }

        private string url = ConfigurationManager.AppSettings["ServicoArquivoUrl"];
    }
}
