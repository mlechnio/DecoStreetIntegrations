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
        internal override string SourcePath => "https://sklep.emibig.com.pl/products2.xml";

        internal override string IdPrefix => "emi";

        internal override int GetDeliveryId()
        {
            return 14;
        }

        internal override int? GetProducerId()
        {
            return 442;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            var textStripped = sourceNode["content"].InnerText.Replace("&nbsp;", "")
                .Replace("\t", "")
                // .Replace("\r\n", "<br>")
                .Replace("Dowód zakupu: Faktura / paragon\r\n", "");

            var bredText = Regex.Replace(textStripped, @"[(\r\n)]{2,}", "<br>");

            var descSplit = bredText.Split(new string[] { "<br>" }, StringSplitOptions.None);

            for (int i = 0; i < descSplit.Length; i++)
            {
                descSplit[i] = "<p>" + descSplit[i].Replace("\r\n", "</p><p>") + "</p>";
            }

            return string.Join("", descSplit);
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["id"].InnerText;
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            yield return sourceNode["img"].InnerText.Replace(" ", "%20");

            foreach (XmlNode item in sourceNode["gallery"].ChildNodes)
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
            return sourceNode["title"].InnerText;
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["price"].InnerText, CultureInfo.InvariantCulture);
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
            var xmlNodeList = xmlDocument.SelectNodes("//products/item");

            var list = xmlNodeList.Cast<XmlNode>().ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list[i]);
            }
        }
    }
}
