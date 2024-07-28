using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace DecoStreetIntegracja.Integrations.ArtPol
{
    public class ArtPol_IntegrationShoperRCANT : IntegratorShoperBase
    {
        private readonly string BreadCrumbTrail = "Obrazy olejne ręcznie malowane / REPRODUKCJE / Architektura / Antyk";

        private readonly string NamePart = "Architektura / Antyk";

        private readonly string ProducerAndNameFileName = "ProducerAndName140724";

        internal override string SourcePath => "http://www.art-pol.pl/offers/pl_wszystkie.xml";

        internal override string IdPrefix => "RCANT";

        internal override int? GetCategoryId()
        {
            return 1092;
        }

        internal override int GetDeliveryId()
        {
            return 9;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return $@"<p>Reprodukcja obrazu drukowana na najwyższej jakości płótnie typu Canvas oraz wykorzystanie druku ekosolwentowego, zapewnia najlepsze odwzorowanie barw w stosunku do oryginału.</p>
<p>Płótno jest naciągnięte na sosnowy blejtram.</p>
<p>Wymiary: {sourceNode["size"].InnerText}</p>";
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode.Attributes["id"].InnerText;
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            foreach (XmlNode item in sourceNode["imgs"].ChildNodes)
            {
                yield return item.Attributes["url"].InnerText.Replace(" ", "%20");
            }
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return false;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return $@"Obraz {NamePart} {sourceNode.Attributes["symbol"].InnerText} {sourceNode["size"].InnerText} RC";
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            var price = Math.Ceiling((decimal.Parse(sourceNode.Attributes["price"].InnerText.Replace('.', ',')) * 1.45M) + 15);
            return price;
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
            return decimal.Parse(sourceNode.Attributes["stock"].InnerText, CultureInfo.InvariantCulture);
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return 0;
        }

        internal override int? GetProducerId()
        {
            return 473;
        }

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            var list = xmlNodeList.Cast<XmlNode>().Where(node => node["breadcrumbtrail"].InnerText == BreadCrumbTrail).ToList();

            Logger.Log($"To process: {list.Count}");

            var existingSymbols = new CsvUtils().GetExistingSymbols(ProducerAndNameFileName);

            for (int i = 0; i < list.Count; i++)
            {
                var symbolToProcess = list[i].Attributes["symbol"].InnerText;
                if (!existingSymbols.Contains(symbolToProcess))
                {
                    ProcessProduct(list, i, list.Count);
                }
                else
                {
                    Logger.Log($"{symbolToProcess} was already in the system.");
                }
            }
        }
    }
}
