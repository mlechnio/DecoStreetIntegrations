using DecoStreetIntegracja.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;

namespace DecoStreetIntegracja.Integrations.Base
{
    public abstract class IntegrationBase : IDisposable
    {
        private readonly NetworkCredential destinationCredentials = new NetworkCredential(ConfigurationManager.AppSettings["hosting_login"], ConfigurationManager.AppSettings["hosting_pass"]);

        internal MemoryStream sourceStream = new MemoryStream();

        internal MemoryStream destinationStream = new MemoryStream();

        internal abstract string DestinationFileName { get; }

        internal abstract string SourcePath { get; }

        internal abstract string IdPrefix { get; }

        internal virtual NetworkCredential SourceCredentials { get; }

        public IntegrationBase()
        {
            DownloadSourceFile();
            //var fileStream = File.Create("local.xml");
            //sourceStream.Seek(0, SeekOrigin.Begin);
            //sourceStream.CopyTo(fileStream);
            //fileStream.Close();
            UploadSourceFileToFtp();
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            GenerateResult();
            UploadResultFile();
            Dispose();
        }

        private void UploadSourceFileToFtp()
        {
            Console.WriteLine("Backup pliku na FTP");
            using (var webClient = new WebClient())
            {
                webClient.Credentials = destinationCredentials;
                webClient.UploadData($@"{ConfigurationManager.AppSettings["hosting_address"]}/bak/{DestinationFileName}.{DateTime.Now.ToString("yyyyMMddTHHmm")}.xml", "STOR", sourceStream.ToArray());
            }
        }

        public virtual void GenerateResult()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                sourceNode.Attributes["id"].InnerText = IdPrefix + sourceNode.Attributes["id"].InnerText;
            }

            xmlDocument.Save(destinationStream);
        }

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
