using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Adansonia_Integration : IntegrationBase
    {
        public override void Init()
        {
            destinationFileName = "adansonia_result.xml";
            sourcePath = "http://admin.adansonia.pl/service/cennik.xml?name=ghost&producer=&category=";
        }

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                sourceNode.Attributes["id"].InnerText = "Adansonia" + sourceNode.Attributes["id"].InnerText;
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
