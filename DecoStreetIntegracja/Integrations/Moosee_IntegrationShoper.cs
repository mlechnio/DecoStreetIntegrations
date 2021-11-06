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

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            var list = xmlNodeList.Cast<XmlNode>()
                .Where(sourceNode => sourceNode["producent"].InnerText == "Moosee" && sourceNode["produkt_wycofany"].InnerText == "NIE").ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                ProcessProduct(list[i]);
            }
        }

        internal override int GetDeliveryId()
        {
            return 11;
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
