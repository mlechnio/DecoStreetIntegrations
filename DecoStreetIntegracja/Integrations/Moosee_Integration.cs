using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Moosee_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "moosee_result.xml";

        internal override string SourcePath => "ftp://kingbath_plikixml@ftp.kingbath.nazwa.pl/kinghomexml.xml";

        internal override string IdPrefix => "Moosee";

        internal override NetworkCredential SourceCredentials => new NetworkCredential("kingbath_plikixml", "Formanowa1");

        public override void GenerateResult()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (sourceNode["producent"].InnerText == "Moosee")
                {
                    element.AppendChild(GenerateONode(xmlDoc, sourceNode));
                }
            }

            xmlDoc.Save(destinationStream);

            Console.WriteLine("Zakończono generowania plików wyjściowych");
        }

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = IdPrefix + sourceNode["numer"].InnerText;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["cena_detaliczna_brutto_bez_waluty"].InnerText;
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            attrAVAIL.Value = int.Parse(sourceNode["stan_liczbowy"].InnerText) == 0 ? "99" : "3";
            var attrWEIGHT = xmlDoc.CreateAttribute("weight");
            attrWEIGHT.Value = sourceNode["waga"].InnerText;
            var attrSTOCK = xmlDoc.CreateAttribute("stock");
            attrSTOCK.Value = sourceNode["stan"].InnerText;

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE);
            nodeO.Attributes.Append(attrAVAIL);
            nodeO.Attributes.Append(attrWEIGHT);
            nodeO.Attributes.Append(attrSTOCK);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["kategoria_glowna"].InnerText);
            var nodeNAME = xmlDoc.CreateElement("name");
            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["nazwa"].InnerText);
            var nodeIMGS = xmlDoc.CreateElement("imgs");

            if (sourceNode["lista_zdjec"].ChildNodes.Count > 0)
            {
                var nodeMAIN = xmlDoc.CreateElement("main");
                var attrMAINURL = xmlDoc.CreateAttribute("url");
                attrMAINURL.Value = sourceNode["lista_zdjec"].ChildNodes[0].InnerText.Replace(" ", "%20");
                nodeMAIN.Attributes.Append(attrMAINURL);
                nodeIMGS.AppendChild(nodeMAIN);
                if (sourceNode["lista_zdjec"].ChildNodes.Count > 1)
                {
                    for (var i = 1; i < sourceNode["lista_zdjec"].ChildNodes.Count; ++i)
                    {
                        var nodeI = xmlDoc.CreateElement("i");
                        var attrIRUL = xmlDoc.CreateAttribute("url");
                        attrIRUL.Value = sourceNode["lista_zdjec"].ChildNodes[i].InnerText.Replace(" ", "%20");
                        nodeI.Attributes.Append(attrIRUL);
                        nodeIMGS.AppendChild(nodeI);
                    }
                }
            }

            var nodeDESC = xmlDoc.CreateElement("desc");
            nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["opis"].InnerText);
            var nodeATTRS = xmlDoc.CreateElement("attrs");
            var nodeANAME1 = xmlDoc.CreateElement("a");
            var attrNAME1 = xmlDoc.CreateAttribute("name");
            attrNAME1.Value = "Producent";
            nodeANAME1.Attributes.Append(attrNAME1);
            nodeANAME1.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["producent"].InnerText);
            //var nodeANAME2 = xmlDoc.CreateElement("a");
            //var attrNAME2 = xmlDoc.CreateAttribute("name");
            //attrNAME2.Value = "Kod_producenta";
            //nodeANAME2.Attributes.Append(attrNAME2);
            //nodeANAME2.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["symbol"].InnerText);
            //var nodeANAME3 = xmlDoc.CreateElement("a");
            //var attrNAME3 = xmlDoc.CreateAttribute("name");
            //attrNAME3.Value = "EAN";
            //nodeANAME3.Attributes.Append(attrNAME3);
            //nodeANAME3.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["EAN13"].InnerText);

            //var nodeANAME4 = xmlDoc.CreateElement("a");
            //var attrNAME4 = xmlDoc.CreateAttribute("name");
            //attrNAME4.Value = "Wysokość";
            //nodeANAME4.Attributes.Append(attrNAME4);
            //nodeANAME4.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["wysokosc_calkowita"].InnerText);

            //var nodeANAME5 = xmlDoc.CreateElement("a");
            //var attrNAME5 = xmlDoc.CreateAttribute("name");
            //attrNAME5.Value = "Głębokość";
            //nodeANAME5.Attributes.Append(attrNAME5);
            //nodeANAME5.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["glebokosc"].InnerText);

            //var nodeANAME6 = xmlDoc.CreateElement("a");
            //var attrNAME6 = xmlDoc.CreateAttribute("name");
            //attrNAME6.Value = "Szerokość";
            //nodeANAME6.Attributes.Append(attrNAME6);
            //nodeANAME6.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["szerokosc"].InnerText);

            nodeATTRS.AppendChild(nodeANAME1);
            //nodeATTRS.AppendChild(nodeANAME2);
            //nodeATTRS.AppendChild(nodeANAME3);
            //nodeATTRS.AppendChild(nodeANAME4);
            //nodeATTRS.AppendChild(nodeANAME5);
            //nodeATTRS.AppendChild(nodeANAME6);
            nodeO.AppendChild(nodeCAT);
            nodeO.AppendChild(nodeNAME);
            nodeO.AppendChild(nodeIMGS);
            nodeO.AppendChild(nodeDESC);
            nodeO.AppendChild(nodeATTRS);

            return nodeO;
        }
    }
}
