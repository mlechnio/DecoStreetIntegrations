using DecoStreetIntegracja.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace DecoStreetIntegracja.Integrations.Base
{
    public abstract class IntegrationBase : IDisposable
    {
        private readonly NetworkCredential destinationCredentials = new NetworkCredential(ConfigurationManager.AppSettings["hosting_login"], ConfigurationManager.AppSettings["hosting_pass"]);

        internal MemoryStream sourceStream = new MemoryStream();

        internal MemoryStream destinationStream = new MemoryStream();

        internal abstract string DestinationFileName { get; }

        internal abstract string SourcePath { get; }

        internal virtual NetworkCredential SourceCredentials { get; }

        public IntegrationBase()
        {
            DownloadSourceFile();
            GenerateResult();
            UploadResultFile();
            Dispose();
        }

        public abstract void GenerateResult();

        private void DownloadSourceFile()
        {
            using (var webClient = new WebClient())
            {
                Console.WriteLine("Rozpoczecie pobierania pliku " + SourcePath);

                webClient.Credentials = SourceCredentials;

                sourceStream = new MemoryStream(webClient.DownloadData(SourcePath));

                Console.WriteLine("Pobrano");
            }
        }

        private void UploadResultFile()
        {
            Console.WriteLine("Rozpoczecie wysyłania pliku " + DestinationFileName + " pod adres " + ConfigurationManager.AppSettings["hosting_address"] + DestinationFileName);

            using (var webClient = new WebClient())
            {
                webClient.Credentials = destinationCredentials;
                webClient.UploadData(ConfigurationManager.AppSettings["hosting_address"] + DestinationFileName, "STOR", destinationStream.ToArray());
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
