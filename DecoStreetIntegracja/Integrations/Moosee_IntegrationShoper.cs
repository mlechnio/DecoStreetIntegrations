using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Moosee_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "ftp://kingbath_plikixml@ftp.kingbath.nazwa.pl/kinghomexml.xml";

        internal override string IdPrefix => "Moosee";

        internal override NetworkCredential SourceCredentials => new NetworkCredential("kingbath_plikixml", "Formanowa1");

        internal override IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode)
        {
            var productImages = new List<ProductImageForInsert>();

            if (sourceNode["lista_zdjec"].ChildNodes.Count > 0)
            {
                productImages.Add(new ProductImageForInsert
                {
                    product_id = product_id,
                    url = sourceNode["lista_zdjec"].ChildNodes[0].InnerText,
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
                            url = sourceNode["lista_zdjec"].ChildNodes[i].InnerText,
                            name = sourceNode["nazwa"].InnerText + " " + i,
                            translations = new ProductTranslations { pl_PL = new Translation { name = sourceNode["nazwa"].InnerText + " " + i } }
                        });
                    }
                }
            }

            return productImages;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["opis"].InnerText;
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["numer"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["nazwa"].InnerText;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["cena_detaliczna_brutto_bez_waluty"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["stan"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["waga"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (sourceNode["producent"].InnerText == "Moosee")
                {
                    ProcessProduct(sourceNode);
                }
            }
        }
    }
}
