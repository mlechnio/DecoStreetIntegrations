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
                ProcessProduct(list[i], canChangePromotion: true);
            }
        }

        internal override int? GetProducerId()
        {
            return 21;
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
            var descSplit = sourceNode["opis_tekstowy"].InnerText.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < descSplit.Length; i++)
            {
                descSplit[i] = "<p>" + descSplit[i].Replace("\r\n", "</p><p>") + "</p>";
            }

            var desc = string.Join("<br>", descSplit);

            return desc
                + (!string.IsNullOrWhiteSpace(sourceNode["wysokosc"].InnerText) ? "<p>Wysokość: " + sourceNode["wysokosc"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["szerokosc"].InnerText) ? "<p>Szerokość: " + sourceNode["szerokosc"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["glebokosc"].InnerText) ? "<p>Głębokość: " + sourceNode["glebokosc"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["wys_siedziska"].InnerText) ? "<p>Wysokość siedziska: " + sourceNode["wys_siedziska"].InnerText + " cm</p>" : string.Empty)
                + (!string.IsNullOrWhiteSpace(sourceNode["wys_Polokietnikow"].InnerText) ? "<p>Wysokość podłokietników: " + sourceNode["wys_Polokietnikow"].InnerText + " cm</p>" : string.Empty);
        }

        internal override int GetDeliveryId()
        {
            return 11;
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
            return decimal.Parse(sourceNode["cena_brutto_przed_promocja"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return sourceNode["czy_w_promocji"].InnerText == "TAK";
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            var dateString = sourceNode["promocja_od_dnia"].InnerText;
            if ("1900-01-01" == dateString || (DateTime.TryParse(dateString, out DateTime parsedDate) && parsedDate < DateTime.Now))
            {
                dateString = DateTime.Now.ToString("yyyy-MM-dd");
            }
            return dateString;
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            return sourceNode["promocja_do_dnia"].InnerText;
        }
    }
}
