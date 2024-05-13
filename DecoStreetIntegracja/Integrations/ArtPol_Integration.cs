using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.IO;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class ArtPol_Integration : IntegrationBase
    {
        internal override string DestinationFileName => $"artpol-{IdPrefix}.xml";

        internal override string SourcePath => "http://www.art-pol.pl/offers/pl_wszystkie.xml";

        internal override string IdPrefix => "absgeoRM";

        private string Name => "Obraz Abstrakcje Geometryczne";

        private string BreadCrumb => "Abstrakcje - Geometryczne";

        private bool IsR => false;

        public override void GenerateResult()
        {
            var counter = 0;
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");
            var breancrumbtrail = IsR ? $"Obrazy olejne ręcznie malowane / Reprodukcje / {BreadCrumb}" : $"Obrazy olejne ręcznie malowane / {BreadCrumb}";
            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (!sourceNode["breadcrumbtrail"].InnerText.Contains(breancrumbtrail))
                {
                    sourceNode.ParentNode.RemoveChild(sourceNode);
                }
                else
                {
                    sourceNode["name"].InnerXml = string.Format(StringCostants.CDataFormat, $"{Name} {sourceNode.Attributes["symbol"].InnerText} {sourceNode["size"].InnerText}{(IsR ? " R" : string.Empty)}");
                    sourceNode.Attributes["id"].InnerText = IdPrefix + sourceNode.Attributes["id"].InnerText;
                    var nodeDESC = xmlDocument.CreateElement("desc");

                    if (IsR)
                    {
                        // Reprodukcja R
                        nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, "Reprodukcja obrazu drukowana na najwyższej jakości płótnie typu Canvas oraz wykorzystanie druku ekosolwentowego, zapewnia najlepsze odwzorowanie barw w stosunku do oryginału. Płótno jest naciągnięte na sosnowy blejtram. Wymiary: " + sourceNode["size"].InnerText);
                    }
                    else
                    {
                        // Malowane M
                        nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, "Piękny obraz namalowany na płótnie przez dyplomowaną artystkę pracowni malarskiej z Wrocławia. Wysoką jakość zapewniają włoskie oraz holenderskie farby olejne, klejone blejtramy z drewna sosnowego oraz klejone ramy z drewna lipowego. Wymiary: " + sourceNode["size"].InnerText);
                    }
                    sourceNode.AppendChild(nodeDESC);
                    counter++;
                }
            }
            xmlDocument.Save(destinationStream);
        }
    }
}
