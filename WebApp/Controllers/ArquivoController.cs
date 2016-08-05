using System;
using System.Collections.Generic;
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
        public bool Upload(string destinationPath, string fileName)
        {
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
            if (postedFile.ContentLength > 100 * 1024 * 1024)
            {
                throw new ArgumentException("Arquivo maior que o limite de 100MB.");
            }
            var serverPath = Server.MapPath("~/App_Data/");
            var combinedPath = Path.Combine(serverPath, destinationPath);
            if (!Directory.Exists(combinedPath))
            {
                Directory.CreateDirectory(combinedPath);
            }
            var filePath = Path.Combine(combinedPath, fileName);
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

        public string GetMd5Hash(string filePath)
        {
            var serverPath = Server.MapPath("~/App_Data/");
            var combinedPath = Path.Combine(serverPath, filePath);
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(combinedPath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }

        public FileStreamResult Download(string filePath)
        {
            var serverPath = Server.MapPath("~/App_Data/");
            var combinedPath = Path.Combine(serverPath, filePath);
            var file = System.IO.File.OpenRead(combinedPath);
            return File(file, "application/octet-stream");
        }
    }
}