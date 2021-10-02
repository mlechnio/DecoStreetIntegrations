﻿using DecoStreetIntegracja.Integrations.Base;
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

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (EnabledData.enabledD2Categories.Contains(sourceNode["kategoria_glowna"].InnerText))
                {
                    ProcessProduct(sourceNode);
                }
            }
        }

        internal override ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode)
        {
            var priceNew = decimal.Parse(sourceNode["cena_brutto"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var stockNew = decimal.Parse(sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), CultureInfo.InvariantCulture);

            if (existingProduct.stock.price != priceNew || existingProduct.stock.stock != stockNew)
            {
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

        internal override ProductForInsert GenerateProductForInsert(XmlNode sourceNode)
        {
            var product = new ProductForInsert();
            var price = decimal.Parse(sourceNode["cena_brutto"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var stock = decimal.Parse(sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), CultureInfo.InvariantCulture);
            var weight = decimal.Parse(sourceNode["waga"].InnerText.Replace(",", ".").Replace(".000", ".00"), CultureInfo.InvariantCulture);

            product.code = IdPrefix + sourceNode["numer"].InnerText;
            product.pkwiu = string.Empty;
            product.stock.stock = stock;
            product.stock.price = price;
            product.stock.weight = weight;

            product.translations.pl_PL = new Translation
            {
                active = true,
                name = sourceNode["nazwa"].InnerText,
                description = sourceNode["opis_tekstowy"].InnerText,
            };

            return product;
        }

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
    }
}