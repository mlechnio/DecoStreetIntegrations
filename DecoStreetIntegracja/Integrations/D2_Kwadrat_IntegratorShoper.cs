using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class D2_Kwadrat_IntegratorShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "ftp://thyone.iq.pl/updatexml.xml";

        internal override NetworkCredential SourceCredentials => new NetworkCredential("dkwadrat_UD2", "zhz907h");

        internal override string IdPrefix => "DK";

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            var list = xmlNodeList.Cast<XmlNode>().Where(x => EnabledData.enabledD2Categories.Contains(x["kategoria_glowna"].InnerText)).ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list[i]);
            }
        }

        internal override IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode)
        {
            var productImages = new List<ProductImageForInsert>();

            if (sourceNode["lista_zdjec"].ChildNodes.Count > 0)
            {
                productImages.Add(new ProductImageForInsert
                {
                    product_id = product_id,
                    url = sourceNode["lista_zdjec"].ChildNodes[0].InnerText.Replace(" ", "%20"),
                    name = sourceNode["nazwa"].InnerText,
                    translations = new ProductTranslations { pl_PL = new Translation { name = sourceNode["nazwa"].InnerText } }
                });

                if (sourceNode["lista_zdjec"].ChildNodes.Count > 1)
                {
                    for (var i = 1; i < sourceNode["lista_zdjec"].ChildNodes.Count; ++i)
                    {
                        productImages.Add(new ProductImageForInsert
                        {
                            product_id = product_id,
                            url = sourceNode["lista_zdjec"].ChildNodes[i].InnerText.Replace(" ", "%20"),
                            name = sourceNode["nazwa"].InnerText + " " + i,
                            translations = new ProductTranslations { pl_PL = new Translation { name = sourceNode["nazwa"].InnerText + " " + i } }
                        });
                    }
                }
            }

            return productImages;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["cena_brutto"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), CultureInfo.InvariantCulture);
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["waga"].InnerText.Replace(",", ".").Replace(".000", ".00"), CultureInfo.InvariantCulture);
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["numer"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["nazwa"].InnerText;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["opis_tekstowy"].InnerText;
        }

        internal override int GetDeliveryId()
        {
            return 11;
        }
    }
}
