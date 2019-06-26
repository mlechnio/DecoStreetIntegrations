using DecoStreetIntegracja.Integrations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Adansonia2_Integration : IntegrationBase
    {
        public override void Init()
        {
            destinationFileName = "adansonia2_result.xml";
            sourcePath = "http://admin.adansonia.pl/cennik/b2b.xml?format=ceneo&category=263&p=802,803,804,805,806,807,808,809,810,811,812,813,814,815,816,817,818,819,820,821,822,823,825,864,869,1011,1012,1013,1014,1016,1017,1028,1029,1070,1165,1166,1431,1432,1433,1434,1435,1436,1437,1438,1439,1440,1441,1447,1448,1484,1750,1912,1913,2056";
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
                sourceNode.Attributes["price"].InnerText = sourceNode.Attributes["suggestedpricebrutto"].InnerText;
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
