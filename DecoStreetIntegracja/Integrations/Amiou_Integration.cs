using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Amiou_Integration : IntegrationBase 
    {
        internal override string DestinationFileName => "amiou_result.xml";

        internal override string SourcePath => "http://85.128.135.34/~amiou/produkty/baza.xml";

        internal override string IdPrefix => throw new NotImplementedException();

        public override void GenerateResult()
        {
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
