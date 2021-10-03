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
    public class Kingshome_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "ftp://kingbath_plikixml@ftp.kingbath.nazwa.pl/kinghomexml.xml";

        internal override string IdPrefix => "khdeco";

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

        internal override ProductForInsert GenerateProductForInsert(XmlNode sourceNode)
        {
            var product = new ProductForInsert();
            var price = decimal.Parse(sourceNode["cena_detaliczna_brutto_bez_waluty"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var stock = decimal.Parse(sourceNode["stan"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var weight = decimal.Parse(sourceNode["waga"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);

            product.code = IdPrefix + sourceNode["numer"].InnerText;
            product.pkwiu = string.Empty;
            product.stock.stock = stock;
            product.stock.price = price;
            product.stock.weight = weight;

            product.translations.pl_PL = new Translation
            {
                active = true,
                name = sourceNode["nazwa"].InnerText,
                description = sourceNode["opis"].InnerText,
            };
            
            return product;
        }

        internal override ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode)
        {
            var priceNew = decimal.Parse(sourceNode["cena_detaliczna_brutto_bez_waluty"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var stockNew = decimal.Parse(sourceNode["stan"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);

            if (existingProduct.stock.price != priceNew || existingProduct.stock.stock != stockNew)
            {
                Logger.Log($"UPDATING {existingProduct.code}, PRICE: {existingProduct.stock.price} -> {priceNew}, STOCK: {existingProduct.stock.stock} -> {stockNew}");
                return new ProductForUpdate
                {
                    product_id = existingProduct.product_id,
                    stock = new ProductStock
                    {
                        price = priceNew,
                        stock = stockNew,
                    }
                };
            }

            return default;
        }

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            var categories = new List<string> { "fotele", "hokery", "krzesła", "oświetlenie", "stoły", "szafki", "stołki", "stoliki", "podnóżki", "biurka" };

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (categories.Contains(sourceNode["kategoria_glowna"].InnerText.ToLower()) && sourceNode["produkt_wycofany"].InnerText == "NIE")
                {
                    ProcessProduct(sourceNode);
                }
            }
        }
    }
}
