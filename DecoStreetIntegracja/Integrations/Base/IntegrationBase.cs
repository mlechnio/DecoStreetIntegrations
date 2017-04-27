using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Integrations.Base
{
    public abstract class IntegrationBase : IIntegration, IDisposable
    {
        private NetworkCredential destinationCredentials = new NetworkCredential(ConfigurationManager.AppSettings["hosting_login"], ConfigurationManager.AppSettings["hosting_pass"]);

        internal MemoryStream sourceStream = new MemoryStream();
        internal MemoryStream destinationStream = new MemoryStream();
        internal string destinationFileName;
        internal string sourcePath;
        internal NetworkCredential sourceCredentials;

        public IntegrationBase()
        {
            Init();
            DownloadSourceFile();
            GenerateResult();
            UploadResultFile();
            Dispose();
        }

        public abstract void Init();
        public abstract void GenerateResult();

        private void DownloadSourceFile()
        {
            using (var webClient = new WebClient())
            {
                Console.WriteLine("Rozpoczecie pobierania pliku " + sourcePath);

                webClient.Credentials = sourceCredentials;

                sourceStream = new MemoryStream(webClient.DownloadData(sourcePath));

                Console.WriteLine("Pobrano");
            }
        }

        private void UploadResultFile()
        {
            Console.WriteLine("Rozpoczecie wysyłania pliku " + destinationFileName + " pod adres " + ConfigurationManager.AppSettings["hosting_address"] + destinationFileName);

            using (var webClient = new WebClient())
            {
                webClient.Credentials = destinationCredentials;
                webClient.UploadData(ConfigurationManager.AppSettings["hosting_address"] + destinationFileName, "STOR", destinationStream.ToArray());
            }

            Console.WriteLine("Zakończono wysyłanie pliku");
        }

        public virtual void Dispose()
        {
            if (sourceStream != null)
            {
                sourceStream.Dispose();
            }

            if (destinationStream != null)
            {
                destinationStream.Dispose();
            }
        }   
    }
}
