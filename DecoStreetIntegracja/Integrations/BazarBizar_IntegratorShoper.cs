using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class BazarBizar_IntegratorShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "https://www.bazarbizar.be/en/feed/custom/28043/";

        internal override string IdPrefix => "BazarBizar";

        internal override void Process()
        {
            var allowedProductSkus = new CsvUtils().GetBazarBizarDecostreetSkus("BazarBizarDecostreetSkus");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//channel/item");

            var list = xmlNodeList.Cast<XmlNode>().Where(x => allowedProductSkus.Contains(x["g:mpn"].InnerText.Trim())).ToList();
            //var listSkus = xmlNodeList.Cast<XmlNode>().Select(x => x["g:mpn"].InnerText.Trim()).ToList();

           // var select = allowedProductSkus.Except(listSkus).ToList();
           // var select2 = listSkus.Except(allowedProductSkus).ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list, i, list.Count);
            }
        }

        internal override int? GetProducerId()
        {
            return 480;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["g:price"].InnerText.Replace(",", ".").Replace(" EUR", ""), CultureInfo.InvariantCulture) * 10;
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["g:stock_level"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), CultureInfo.InvariantCulture);
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return 0;
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["g:id"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["title"].InnerText;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["description"].InnerText;
        }

        internal override int GetDeliveryId()
        {
            return 9;
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            var xmlDoc = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("g", "http://base.google.com/ns/1.0");

            if (sourceNode["g:image_link"] != null)
            {
                yield return sourceNode["g:image_link"].InnerText.Replace(" ", "%20");
            }

            var images = sourceNode.SelectNodes("g:additional_image_link", nsmgr);

            foreach (XmlNode image in images)
            {
                yield return image.InnerText.Replace(" ", "%20");
            }
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            return 0;
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return false;
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            return null;
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            return null;
        }
    }
}
