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
    public class Emibig_IntegrationShoperUpdateImages : IntegratorShoperBase
    {
        internal override string SourcePath => @"C:\Users\mariu\Downloads\produkty_xml_3_28-04-2024_19 13 08_pl.xml";

        internal override string IdPrefix => "emix";

        internal override int GetDeliveryId()
        {
            return 14;
        }

        internal override int? GetProducerId()
        {
            return 467;
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
                ProcessProduct(list, i, list.Count, insertNew: false, updateImages: true);
            }
        }
    }
}
