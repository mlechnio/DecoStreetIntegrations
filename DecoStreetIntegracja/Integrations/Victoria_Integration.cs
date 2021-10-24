using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Victoria_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "victoria_result_6.xml";

        internal override string SourcePath => "http://pikimar.cba.pl/prod/victoria_ceneo_src.xml";

        internal override string IdPrefix => "vicmeb";

        public override void GenerateResult()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//offers/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                sourceNode.Attributes["id"].InnerText = IdPrefix + sourceNode.Attributes["id"].InnerText;
                sourceNode.Attributes["price"].InnerText = Math.Ceiling(decimal.Parse(sourceNode.Attributes["price"].InnerText.Replace('.', ','))).ToString();
                var attrs = sourceNode["attrs"].ChildNodes;

                var desc = sourceNode["desc"].InnerText + "<br><br><strong>Parametry techniczne:</strong><br><br>";

                foreach (XmlNode attr in attrs)
                {
                    var name = attr.Attributes["name"].InnerText;

                    var val = attr.InnerText;

                    if (name != "Kod_producenta")
                    {
                        desc += "<strong>" + name + ":</strong> " + val + "<br>";
                    }
                }

                desc += "<br><strong>Czas realizacji:</strong> 14-21 dni roboczych";
                desc += "<br><strong>Kraj produkcji:</strong> Polska";

                sourceNode["desc"].InnerXml = string.Format(StringCostants.CDataFormat, desc);
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
