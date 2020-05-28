using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class CustomFormGoogle_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "customformgoogle_result.xml";

        internal override string SourcePath => "https://customform.co/wp-content/uploads/googlefeeds/google.xml";

        internal override string IdPrefix => "CF";

        public override void GenerateResult()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//channel/item");
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (InStock(sourceNode))
                {
                    element.AppendChild(GenerateONode(xmlDoc, sourceNode));
                }
            }

            xmlDoc.Save(destinationStream);
        }

        private bool InStock(XmlNode sourceNode)
        {
            return sourceNode["g:availability"].InnerText == "in stock";
        }

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("g", "http://base.google.com/ns/1.0");
            Console.WriteLine(sourceNode["g:id"].InnerText);
            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = IdPrefix + sourceNode["g:id"].InnerText;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["g:price"].InnerText.Split(' ')[0];
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            var quantityText = sourceNode["g:availability"].InnerText;
            attrAVAIL.Value = quantityText == "in stock" ? "3" : "99";

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE); // jest wartosc + waluta...
            nodeO.Attributes.Append(attrAVAIL);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["g:google_product_category"].InnerText);
            var nodeNAME = xmlDoc.CreateElement("name");
            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["title"].InnerText);
            var nodeIMGS = xmlDoc.CreateElement("imgs");

            var nodeMAIN = xmlDoc.CreateElement("main");

            if (sourceNode["g:image_link"] != null)
            {
                var attrMAINURL = xmlDoc.CreateAttribute("url");
                attrMAINURL.Value = sourceNode["g:image_link"].InnerText;
                nodeMAIN.Attributes.Append(attrMAINURL);
                nodeIMGS.AppendChild(nodeMAIN);
            }

            var images = sourceNode.SelectNodes("g:additional_image_link", nsmgr);

            foreach (XmlNode image in images)
            {
                var nodeI = xmlDoc.CreateElement("i");
                var attrIRUL = xmlDoc.CreateAttribute("url");
                attrIRUL.Value = image.InnerText;
                nodeI.Attributes.Append(attrIRUL);
                nodeIMGS.AppendChild(nodeI);
            }

            var nodeDESC = xmlDoc.CreateElement("desc");
            nodeDESC.InnerXml = sourceNode["description"] != null ? string.Format(StringCostants.CDataFormat, sourceNode["description"].InnerText) : string.Empty;
            var nodeATTRS = xmlDoc.CreateElement("attrs");
            var nodeANAME1 = xmlDoc.CreateElement("a");
            var attrNAME1 = xmlDoc.CreateAttribute("name");
            attrNAME1.Value = "Producent";
            nodeANAME1.Attributes.Append(attrNAME1);
            //nodeANAME1.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["brand"].InnerText);
            var nodeANAME2 = xmlDoc.CreateElement("a");
            var attrNAME2 = xmlDoc.CreateAttribute("name");
            attrNAME2.Value = "Kod_producenta";
            nodeANAME2.Attributes.Append(attrNAME2);
            nodeANAME2.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["g:id"].InnerText);
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
