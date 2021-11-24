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
    public class Aldex_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => @"C:\Users\mariu\Downloads\cennik2.xml";

        internal override string IdPrefix => "Aldex";

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("dupa", "http://www.cdn.com.pl/optima/dokument");
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//dupa:towar", nsmgr);

            var list = xmlNodeList.Cast<XmlNode>().ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list[i]);
            }
        }

        //internal override IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode)
        //{
        //    var productImages = new List<ProductImageForInsert>();

        //    productImages.Add(new ProductImageForInsert
        //    {
        //        product_id = product_id,
        //        url = sourceNode["zdjecie"].InnerText.Replace(" ", "%20"),
        //        name = GetNameFromNode(sourceNode),
        //        translations = new ProductTranslations { pl_PL = new Translation { name = GetNameFromNode(sourceNode) } }
        //    });

        //    return productImages;
        //}

        internal override int GetDeliveryId()
        {
            return 8;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return 
                $@"Wykończenie: {sourceNode["wykonczenie"].InnerText}<br>
                Rodzaj trzonka: {sourceNode["rodzaj_trzonka"].InnerText}<br>
                Max moc: {sourceNode["max._Wattage"].InnerText}<br>
                Szerokość: {sourceNode["szerokosc_lampy"].InnerText} cm<br>
                Wysokość: {sourceNode["wysokosc_lampy"].InnerText} cm<br>
                Głębokość: {sourceNode["glebokosc_lampy"].InnerText} cm<br>
                Średnica klosza: {sourceNode["srednica_klosz"].InnerText} cm<br>
                Waga: {sourceNode["waga_lampy"].InnerText} kg
                ";
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["twr_kod"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["nazwa"].InnerText;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["cena_brutto"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return 900;
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["waga_lampy"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            yield return sourceNode["zdjecie"].InnerText.Replace(" ", "%20");
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
