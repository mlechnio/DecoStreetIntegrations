﻿using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Kingshome_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "ftp://kingbath_plikixml@ftp.kingbath.nazwa.pl/kinghomexml.xml";

        internal override string IdPrefix => "khdeco";

        internal override NetworkCredential SourceCredentials => new NetworkCredential("kingbath_plikixml", "Formanowa1");

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            var categories = new List<string> { "fotele", "hokery", "krzesła", "oświetlenie", "stoły", "szafki", "stołki", "stoliki", "podnóżki", "biurka", "krzesła barowe" };

            var list = xmlNodeList.Cast<XmlNode>()
                .Where(sourceNode =>
                sourceNode["producent"].InnerText != "Moosee"
                && (categories.Contains(sourceNode["kategoria_glowna"].InnerText.ToLower()) || (sourceNode["kategoria_glowna"].InnerText.ToLower() == "akcesoria" && sourceNode["kategoria_wielopoziomowa"].InnerText.ToLower() == "wieszaki"))
                && sourceNode["produkt_wycofany"].InnerText == "NIE"
                ).ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list, i, list.Count, insertNew: true);
            }

            var toDelete = xmlNodeList.Cast<XmlNode>().Where(sourceNode => sourceNode["produkt_wycofany"].InnerText == "TAK").ToList();

            for (int i = 0; i < toDelete.Count; i++)
            {
                var productCode = IdPrefix + GetIdFromNode(toDelete[i]);

                Console.WriteLine($"Trying to delete product: {productCode}");

                Thread.Sleep(500);

                var existingProduct = GetExistingProduct(productCode);

                if (existingProduct != null)
                {
                    try
                    {
                        DeleteProduct(existingProduct.product_id);
                        Logger.Log($"DeletedProduct: {new JsonSerializer().Serialize(existingProduct)}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
                else
                {
                    Logger.Log($"Product already doesn't exist: {productCode}");
                }
            }
        }

        internal override int GetDeliveryId()
        {
            return 11;
        }

        internal override int? GetProducerId()
        {
            return 288;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            var descSplit = sourceNode["opis"].InnerText.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < descSplit.Length; i++)
            {
                descSplit[i] = "<p>" + descSplit[i].Replace("\r\n", "</p><p>") + "</p>";
            }

            var desc = string.Join("<br>", descSplit);

            return desc
                + (!string.IsNullOrWhiteSpace(sourceNode["wysokosc_calkowita"].InnerText) ? "<p>Wysokość całkowita: " + sourceNode["wysokosc_calkowita"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["szerokosc"].InnerText) ? "<p>Szerokość: " + sourceNode["szerokosc"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["glebokosc"].InnerText) ? "<p>Głębokość: " + sourceNode["glebokosc"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["wysokosc_podlokietnikow"].InnerText) ? "<p>Wysokość podłokietników: " + sourceNode["wysokosc_podlokietnikow"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["wysokosc_krawedzi_siedziska"].InnerText) ? "<p>Wysokość krawędzi siedziska: " + sourceNode["wysokosc_krawedzi_siedziska"].InnerText + " cm</p>" : string.Empty);
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

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            if (sourceNode["lista_zdjec"].ChildNodes.Count > 0)
            {
                yield return sourceNode["lista_zdjec"].ChildNodes[0].InnerText.Replace(" ", "%20");

                if (sourceNode["lista_zdjec"].ChildNodes.Count > 1)
                {
                    for (var i = 1; i < sourceNode["lista_zdjec"].ChildNodes.Count; ++i)
                    {
                        yield return sourceNode["lista_zdjec"].ChildNodes[i].InnerText.Replace(" ", "%20");
                    }
                }
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
