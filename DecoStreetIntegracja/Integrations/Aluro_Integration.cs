using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Aluro_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "aluro_result.xml";

        internal override string SourcePath => "http://www.partner.aluro.pl/export-xml/aluro_products_export_ldWd8HWmUY.xml";

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//export_products/product");
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (EnabledData.enabledAluroIds.Contains(sourceNode["symbol"].InnerText))
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
            attrID.Value = "Aluro" + sourceNode["symbol"].InnerText;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["cena_detaliczna"].InnerText;
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            attrAVAIL.Value = int.Parse(sourceNode["stock"].InnerText) == 0 ? "99" : "3";
            var attrWEIGHT = xmlDoc.CreateAttribute("weight");
            attrWEIGHT.Value = sourceNode["waga_z_opakowaniem"].InnerText;
            var attrSTOCK = xmlDoc.CreateAttribute("stock");
            attrSTOCK.Value = sourceNode["stock"].InnerText;

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE);
            nodeO.Attributes.Append(attrAVAIL);
            nodeO.Attributes.Append(attrWEIGHT);
            nodeO.Attributes.Append(attrSTOCK);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["product_name"].InnerText.Split(' ', '-')[0]);
            var nodeNAME = xmlDoc.CreateElement("name");
            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["product_name"].InnerText);
            var nodeIMGS = xmlDoc.CreateElement("imgs");
            if (sourceNode["images"].InnerText.Split(',').Length > 0)
            {
                var imagesLinks = sourceNode["images"].InnerText.Split(',');

                var nodeMAIN = xmlDoc.CreateElement("main");
                var attrMAINURL = xmlDoc.CreateAttribute("url");
                attrMAINURL.Value = imagesLinks[0].Trim().Replace(" ", "%20");
                nodeMAIN.Attributes.Append(attrMAINURL);
                nodeIMGS.AppendChild(nodeMAIN);
                if (imagesLinks.Length > 1)
                {
                    for (var i = 1; i < imagesLinks.Length; ++i)
                    {
                        var nodeI = xmlDoc.CreateElement("i");
                        var attrIRUL = xmlDoc.CreateAttribute("url");
                        attrIRUL.Value = imagesLinks[i].Trim().Replace(" ", "%20");
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
            nodeANAME1.InnerXml = string.Format(StringCostants.CDataFormat, "ALURO");
            var nodeANAME2 = xmlDoc.CreateElement("a");
            var attrNAME2 = xmlDoc.CreateAttribute("name");
            attrNAME2.Value = "Kod_producenta";
            nodeANAME2.Attributes.Append(attrNAME2);
            nodeANAME2.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["symbol"].InnerText);
            var nodeANAME3 = xmlDoc.CreateElement("a");
            var attrNAME3 = xmlDoc.CreateAttribute("name");
            attrNAME3.Value = "EAN";
            nodeANAME3.Attributes.Append(attrNAME3);
            nodeANAME3.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["EAN13"].InnerText);
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
