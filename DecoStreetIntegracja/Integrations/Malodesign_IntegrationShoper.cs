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
                ProcessProduct(list[i]);
            }
        }

        internal override IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode)
        {
            var productImages = new List<ProductImageForInsert>();

            var count = 0;
            foreach (XmlNode item in sourceNode["imgs"].ChildNodes)
            {
                productImages.Add(new ProductImageForInsert
                {
                    product_id = product_id,
                    url = item.Attributes["url"].InnerText,
                    name = sourceNode["name"].InnerText + (count > 0 ? " " + count : ""),
                    translations = new ProductTranslations { pl_PL = new Translation { name = sourceNode["name"].InnerText + (count > 0 ? " " + count : "") } }
                });
                count++;
            }

            return productImages;
        }

        internal override int GetDeliveryId()
        {
            return 11;
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
    }
}
