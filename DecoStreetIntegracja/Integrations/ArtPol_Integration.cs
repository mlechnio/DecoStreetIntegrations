using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class ArtPol_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "gobi_obrazy_result_baletnice_r.xml";

        internal override string SourcePath => "http://www.art-pol.pl/offers/pl_wszystkie.xml";

        internal override string IdPrefix => "obraz";

        public override void GenerateResult()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//group/o");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                //if (!EnabledData.enabledGobiObrazy.Contains(sourceNode.Attributes["id"].InnerText))
                //if (!EnabledData.enabledGobiObrazyFilmoweM.Contains(sourceNode.Attributes["symbol"].InnerText))
                //if (!EnabledData.enabledGobiObrazyFilmoweR.Contains(sourceNode.Attributes["symbol"].InnerText))
                //if (!EnabledData.enabledGobiObrazyFilmoweKlimtM.Contains(sourceNode.Attributes["symbol"].InnerText))
                //if (!EnabledData.enabledGobiObrazyFilmoweKlimtR.Contains(sourceNode.Attributes["symbol"].InnerText))
                if (!EnabledData.enabledGobiObrazyBaletniceR.Contains(sourceNode.Attributes["symbol"].InnerText))
                {
                    sourceNode.ParentNode.RemoveChild(sourceNode);
                }
                else
                {
                    sourceNode["name"].InnerXml = string.Format(StringCostants.CDataFormat, $"Obraz {sourceNode.Attributes["symbol"].InnerText}");
                    sourceNode.Attributes["id"].InnerText = IdPrefix + sourceNode.Attributes["id"].InnerText;
                    var nodeDESC = xmlDocument.CreateElement("desc");
                    // Malowane M
                    //nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, "Piękny obraz namalowany na płótnie przez dyplomowaną artystkę pracowni malarskiej z Wrocławia. Wysoką jakość zapewniają włoskie oraz holenderskie farby olejne, klejone blejtramy z drewna sosnowego oraz klejone ramy z drewna lipowego (dotyczy obrazów z ramą). Wymiary (z ramą jeżeli dotyczy): " + sourceNode["size"].InnerText);
                    // Reprodukcja R
                    nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, "Reprodukcja obrazu drukowana na najwyższej jakości płótnie typu Canvas oraz wykorzystanie druku ekosolwentowego, zapewnia najlepsze odwzorowanie barw w stosunku do oryginału. Płótno jest naciągnięte na sosnowy blejtram. Wymiary (z ramą jeżeli dotyczy): " + sourceNode["size"].InnerText);
                    sourceNode.AppendChild(nodeDESC);
                }
            }

            xmlDocument.Save(destinationStream);
        }
    }
}
