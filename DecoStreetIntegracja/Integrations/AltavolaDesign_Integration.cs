using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class AltavolaDesign_Integration : IntegrationBase
    {
        internal override string DestinationFileName => "altavola_design_result.xml";

        internal override string SourcePath => "http://altavola-design.pl/edi/export-offer.php?client=sklep@decostreet.pl&language=pol&token=41c1551467eb613ff609e0e&shop=1&type=full&format=xml&iof_2_6";

        internal override string IdPrefix => "ALTAVOLA";

        public override void GenerateResult()
        {

            var xmlDocument = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("dupa", "http://www.iai-shop.com/developers/iof.phtml");
            nsmgr.AddNamespace("iaiext", "http://www.iai-shop.com/developers/iof/extensions.phtml");

            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//dupa:products", nsmgr);
            //var xmlNodeList = xmlDocument.SelectNodes("/iaiext:product", nsmgr);
            var productNodes = xmlNodeList[0].ChildNodes;
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);

            foreach (XmlNode sourceNode in productNodes)
            {
                var name = sourceNode["producer"].Attributes["name"].Value;
                if (name == "ALTAVOLA DESIGN")
                {
                    element.AppendChild(GenerateONode(xmlDoc, sourceNode));
                }
            }

            xmlDoc.Save(destinationStream);

            Console.WriteLine("Zakończono generowania plików wyjściowych");
        }

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");

            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = IdPrefix + sourceNode.Attributes["id"].Value;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["price"].Attributes["gross"].Value;
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            attrAVAIL.Value = "";// int.Parse(sourceNode["stan_liczbowy"].InnerText) == 0 ? "99" : "3"; //
            var attrWEIGHT = xmlDoc.CreateAttribute("weight");
            attrWEIGHT.Value = "";// sourceNode["waga"].InnerText;
            var attrSTOCK = xmlDoc.CreateAttribute("stock");
            attrSTOCK.Value = "";// sourceNode["stan"].InnerText;

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE);
            nodeO.Attributes.Append(attrAVAIL);
            nodeO.Attributes.Append(attrWEIGHT);
            nodeO.Attributes.Append(attrSTOCK);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, "");// sourceNode["kategoria_glowna"].InnerText);
            var nodeNAME = xmlDoc.CreateElement("name");
            var names = sourceNode.SelectNodes("name[@xml:lang='pol']", nsmgr);
            var name = string.Empty;

            foreach (XmlNode item in sourceNode["description"].ChildNodes)
            {
                if (item.Name == "name" && item.Attributes["xml:lang"].Value == "pol")
                {
                    name = item.InnerText;
                }
            }

            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, name);
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
