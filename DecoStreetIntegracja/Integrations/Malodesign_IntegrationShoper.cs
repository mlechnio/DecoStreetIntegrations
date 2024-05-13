using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Malodesign_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "https://malodesign.pl/modules/pricewars2/service.php?id_xml=7";

        internal override string IdPrefix => "Malodesign";

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            var list = xmlNodeList.Cast<XmlNode>().ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list, i, list.Count);
            }
        }

        internal override int GetDeliveryId()
        {
            return 11;
        }

        internal override int? GetProducerId()
        {
            return 254;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["desc"].InnerText;
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode.Attributes["id"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["name"].InnerText;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode.Attributes["price"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            var quantity = decimal.Parse(sourceNode.Attributes["stock"].InnerText, CultureInfo.InvariantCulture);
            return quantity >= 0 ? quantity : 0;
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode.Attributes["weight"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            foreach (XmlNode item in sourceNode["imgs"].ChildNodes)
            {
                yield return item.Attributes["url"].InnerText.Replace(" ", "%20");
            }
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return false;
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }
    }
}
