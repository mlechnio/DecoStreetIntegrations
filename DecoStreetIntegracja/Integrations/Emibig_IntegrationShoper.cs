using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Emibig_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => @"C:\Users\mariu\Downloads\produkty_xml_3_23-06-2024_13_46_57_pl.xml";

        internal override string IdPrefix => "emix";

        private List<string> ImportNames = new List<string> { "TECNO 1XXL WHITE oprawa oświetleniowa", "TECNO 1XL WHITE oprawa oświetleniowa", "TECNO 1L WHITE oprawa oświetleniowa", "TECNO 1S WHITE oprawa oświetleniowa", "TECNO 1XXL BLACK oprawa oświetleniowa", "TECNO 1XL BLACK oprawa oświetleniowa", "TECNO 1L BLACK oprawa oświetleniowa", "TECNO 1M BLACK oprawa oświetleniowa", "TECNO 1S BLACK oprawa oświetleniowa", };

        internal override int GetDeliveryId()
        {
            return 9;
        }

        internal override int? GetProducerId()
        {
            return 472;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["desc"].InnerText;
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["id"].InnerText;
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            //yield return sourceNode["photos"].InnerText.Replace(" ", "%20");

            foreach (XmlNode item in sourceNode["photos"].ChildNodes)
            {
                yield return item.InnerText.Replace(" ", "%20");
            }
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return false;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["name"].InnerText;
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["retailPriceGross"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return 1000;
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return 30;
        }

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//products/product");

            var list = xmlNodeList.Cast<XmlNode>().ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                if (ImportNames.Contains(GetNameFromNode(list[i])))
                {
                    ProcessProduct(list, i, list.Count);
                }
            }
        }
    }
}
