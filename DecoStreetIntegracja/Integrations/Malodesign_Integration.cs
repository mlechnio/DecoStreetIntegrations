using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Malodesign_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "malodesign_result.xml";

        internal override string SourcePath => "https://malodesign.pl/modules/pricewars2/service.php?id_xml=7";

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                sourceNode.Attributes["id"].InnerText = "Malodesign" + sourceNode.Attributes["id"].InnerText;
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
