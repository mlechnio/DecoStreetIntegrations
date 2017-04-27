using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class CustomForm_Integration : IntegrationBase
    {
        public override void Init()
        {
            destinationFileName = "customform_result.xml";
            sourcePath = "http://www.customform.pl/xml/homebook.xml";
        }

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//offers/offer");
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (!sourceNode["categories"].InnerText.Contains("Outlet") && sourceNode["stock"] != null && sourceNode["stock"].InnerText != string.Empty && sourceNode["stock"].InnerText != "brak")
                {
                    element.AppendChild(GenerateONode(xmlDoc, sourceNode));
                }
            }

            xmlDoc.Save(destinationStream);
        }

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = "CustomForm" + sourceNode["id"].InnerText;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["price"].InnerText;
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            attrAVAIL.Value = int.Parse(sourceNode["quantity"].InnerText) <= 0 ? "99" : "3";
            var attrWEIGHT = xmlDoc.CreateAttribute("weight");
            attrWEIGHT.Value = sourceNode["weight"].InnerText;
            var attrSTOCK = xmlDoc.CreateAttribute("stock");
            attrSTOCK.Value = sourceNode["quantity"].InnerText;

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE);
            nodeO.Attributes.Append(attrAVAIL);
            nodeO.Attributes.Append(attrWEIGHT);
            nodeO.Attributes.Append(attrSTOCK);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["categories"].InnerText.Split('|')[0].Trim());
            var nodeNAME = xmlDoc.CreateElement("name");
            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["name"].InnerText);
            var nodeIMGS = xmlDoc.CreateElement("imgs");
            if (sourceNode["img"].InnerText.Length > 0)
            {
                var nodeMAIN = xmlDoc.CreateElement("main");
                var attrMAINURL = xmlDoc.CreateAttribute("url");
                attrMAINURL.Value = sourceNode["img"].InnerText.Replace(" ", "%20");
                nodeMAIN.Attributes.Append(attrMAINURL);
                nodeIMGS.AppendChild(nodeMAIN);
            }

            var nodeDESC = xmlDoc.CreateElement("desc");
            nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["description"].InnerText + (sourceNode["stock"].InnerText == "Dostępny w ciągu 48h" || sourceNode["stock"].InnerText == "dostępny w 48 godzin" || sourceNode["stock"].InnerText == "dostępny w ciągu 48h" ? "" : " " + sourceNode["stock"].InnerText)); //!!
            var nodeATTRS = xmlDoc.CreateElement("attrs");
            var nodeANAME1 = xmlDoc.CreateElement("a");
            var attrNAME1 = xmlDoc.CreateAttribute("name");
            attrNAME1.Value = "Producent";
            nodeANAME1.Attributes.Append(attrNAME1);
            nodeANAME1.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["brand"].InnerText);
            var nodeANAME2 = xmlDoc.CreateElement("a");
            var attrNAME2 = xmlDoc.CreateAttribute("name");
            attrNAME2.Value = "Kod_producenta";
            nodeANAME2.Attributes.Append(attrNAME2);
            nodeANAME2.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["id"].InnerText);
            var nodeANAME3 = xmlDoc.CreateElement("a");
            var attrNAME3 = xmlDoc.CreateAttribute("name");
            attrNAME3.Value = "EAN";
            nodeANAME3.Attributes.Append(attrNAME3);
            nodeANAME3.InnerXml = ""; // string.Format(StringCostants.CDataFormat, sourceNode["EAN13"].InnerText);
            nodeATTRS.AppendChild(nodeANAME1);
            nodeATTRS.AppendChild(nodeANAME2);
            nodeATTRS.AppendChild(nodeANAME3);
            nodeO.AppendChild(nodeCAT);
            nodeO.AppendChild(nodeNAME);
            nodeO.AppendChild(nodeIMGS);
            nodeO.AppendChild(nodeDESC);
            nodeO.AppendChild(nodeATTRS);

            return nodeO;
        }
    }
}
