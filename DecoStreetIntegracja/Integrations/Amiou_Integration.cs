using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Amiou_Integration : IntegrationBase 
    {
        public override void Init()
        {
            destinationFileName = "amiou_result.xml";
            sourcePath = "http://85.128.135.34/~amiou/produkty/baza.xml";
        }

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//offers/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (!EnabledData.enabledAmiouCategories.Contains(sourceNode["cat"].InnerText))
                {
                    sourceNode.ParentNode.RemoveChild(sourceNode);
                }
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
