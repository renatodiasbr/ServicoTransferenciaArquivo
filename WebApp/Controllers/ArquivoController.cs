using ServicoTransferenciaArquivo.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ServicoTransferenciaArquivo.WebApp.Controllers
{
    public class ArquivoController : Controller
    {
        [HttpPost]
        public bool Upload(string destinationPath, string fileName, string ticks)
        {
            ValidarTicks(ticks);

            if (Request.Files.Count == 0)
            {
                throw new ArgumentNullException("Arquivo não foi informado.");
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var postedFile = Request.Files[0];

            if (postedFile.ContentLength > Convert.ToInt32(ConfigurationManager.AppSettings["MaxFileSize"]))
            {
                throw new ArgumentException("Arquivo maior que o limite de 100MB.");
            }

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            var filePath = Path.Combine(destinationPath, fileName);
            var backupFilePath = filePath +".bkp";

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Copy(filePath, backupFilePath);
                    System.IO.File.Delete(filePath);
                }
                
                postedFile.SaveAs(filePath);

                if (System.IO.File.Exists(backupFilePath))
                {
                    System.IO.File.Delete(backupFilePath);
                }
            }
            catch (Exception)
            {
                if (!System.IO.File.Exists(filePath) && System.IO.File.Exists(backupFilePath))
                {
                    System.IO.File.Copy(backupFilePath, filePath);
                    System.IO.File.Delete(backupFilePath);
                }

                throw;
            }

            return true;
        }

        public string GetMd5Hash(string filePath, string ticks)
        {
            ValidarTicks(ticks);
            return MD5Encryption.GetMd5Hash(filePath);
        }

        public FileStreamResult Download(string filePath, string ticks)
        {
            ValidarTicks(ticks);
            var file = System.IO.File.OpenRead(filePath);
            return File(file, "application/octet-stream");
        }

        private void ValidarTicks(string ticks)
        {
            if (!ValidaAcesso.ValidarTicks(ticks))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}