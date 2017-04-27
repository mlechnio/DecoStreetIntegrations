using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Durbas_Integration : IntegrationBase
    {
        public override void Init()
        {
            destinationFileName = "durbas_result.xml";
            sourcePath = "http://umeblujmieszkanie.pl/products/xml/ceneo";
        }

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                sourceNode.Attributes["id"].InnerText = "Durbas" + sourceNode.Attributes["id"].InnerText;
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
