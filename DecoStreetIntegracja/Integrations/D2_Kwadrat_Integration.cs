using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class D2_Kwadrat_Integration : IntegrationBase
    {
        public override void Init()
        {
            destinationFileName = "d2_kwadrat_result.xml";
            sourcePath = "ftp://thyone.iq.pl/updatexml.xml";
            sourceCredentials = new NetworkCredential("dkwadrat_UD2", "zhz907h");
        }

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = "DK" + sourceNode["numer"].InnerText;
            var attrURL = xmlDoc.CreateAttribute("url");
            attrURL.Value = "";
            var attrPRICE = xmlDoc.CreateAttribute("price");
            attrPRICE.Value = sourceNode["cena_brutto"].InnerText.Replace(",", ".");
            var attrAVAIL = xmlDoc.CreateAttribute("avail");
            int stan;
            attrAVAIL.Value = int.TryParse(sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), out stan) ? stan == 0 ? "99" : "3" : "99";
            var attrWEIGHT = xmlDoc.CreateAttribute("weight");
            attrWEIGHT.Value = sourceNode["waga"].InnerText.Replace(",", ".").Replace(".000", ".00");
            var attrSTOCK = xmlDoc.CreateAttribute("stock");
            attrSTOCK.Value = sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", "");

            nodeO.Attributes.Append(attrID);
            nodeO.Attributes.Append(attrURL);
            nodeO.Attributes.Append(attrPRICE);
            nodeO.Attributes.Append(attrAVAIL);
            nodeO.Attributes.Append(attrWEIGHT);
            nodeO.Attributes.Append(attrSTOCK);

            var nodeCAT = xmlDoc.CreateElement("cat");
            nodeCAT.InnerXml = string.Format(StringCostants.CDataFormat, (object)sourceNode["kategoria_glowna"].InnerText);
            var nodeNAME = xmlDoc.CreateElement("name");
            nodeNAME.InnerXml = string.Format(StringCostants.CDataFormat, (object)sourceNode["nazwa"].InnerText);
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
            nodeDESC.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["opis_tekstowy"].InnerText);
            var nodeATTRS = xmlDoc.CreateElement("attrs");
            var nodeANAME1 = xmlDoc.CreateElement("a");
            var attrNAME1 = xmlDoc.CreateAttribute("name");
            attrNAME1.Value = "Producent";
            nodeANAME1.Attributes.Append(attrNAME1);
            nodeANAME1.InnerXml = string.Format(StringCostants.CDataFormat, "D2");
            var nodeANAME2 = xmlDoc.CreateElement("a");
            var attrNAME2 = xmlDoc.CreateAttribute("name");
            attrNAME2.Value = "Kod_producenta";
            nodeANAME2.Attributes.Append(attrNAME2);
            nodeANAME2.InnerXml = string.Format(StringCostants.CDataFormat, string.Empty);
            var nodeANAME3 = xmlDoc.CreateElement("a");
            var attrNAME3 = xmlDoc.CreateAttribute("name");
            attrNAME3.Value = "EAN";
            nodeANAME3.Attributes.Append(attrNAME3);
            nodeANAME3.InnerXml = string.Format(StringCostants.CDataFormat, sourceNode["kod_kreskowy"].InnerText);
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

        public override void GenerateResult()
        {
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var element = xmlDoc.CreateElement("offers", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(element);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("URL;Title;Short description;Description;Category;Tags;Vendor;Visibility;Quantity;Buy if empty;Promotion Price;Price Regular;SKU;Barcode;Width;Height;Depth;Diameter;Weight;Image 1;Image 2;Image 3;Image 4;Image 5;Image 6;Image 7;Image 8;Image 9;Image 10;Image 11;Image 12;Image 13;Image 14;Image 15");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (EnabledData.enabledD2Categories.Contains(sourceNode["kategoria_glowna"].InnerText))
                {
                    element.AppendChild(GenerateONode(xmlDoc, sourceNode));
                    var array = Enumerable.Repeat<string>(string.Empty, 15).ToArray<string>();
                    for (int index = 0; index < sourceNode["lista_zdjec"].ChildNodes.Count; ++index)
                        array[index] = sourceNode["lista_zdjec"].ChildNodes[index].InnerText.Replace(" ", "%20");
                    var str = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23};{24};{25};{26};{27};{28};{29};{30};{31};{32};{33}", (object)string.Empty, (object)sourceNode["nazwa"].InnerText, (object)string.Empty, (object)sourceNode["opis_tekstowy"].InnerText, (object)sourceNode["kategoria_glowna"].InnerText, (object)string.Empty, (object)"D2", (object)1, (object)sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), (object)0, (object)sourceNode["cena_brutto"].InnerText.Replace(",", "."), (object)sourceNode["cena_brutto"].InnerText.Replace(",", "."), (object)sourceNode["numer"].InnerText, (object)sourceNode["kod_kreskowy"].InnerText, (object)sourceNode["szerokosc"].InnerText.Split('-')[0], (object)sourceNode["wysokosc"].InnerText.Split('-')[0], (object)sourceNode["glebokosc"].InnerText.Split('-')[0], (object)0, (object)sourceNode["waga"].InnerText.Replace(",", ".").Replace(".000", ".00"), (object)array[0], (object)array[1], (object)array[2], (object)array[3], (object)array[4], (object)array[5], (object)array[6], (object)array[7], (object)array[8], (object)array[9], (object)array[10], (object)array[11], (object)array[12], (object)array[13], (object)array[14]);
                    stringBuilder.AppendLine(str);
                }
            }

            xmlDoc.Save(destinationStream);

            //File.WriteAllText("d2-result.csv", stringBuilder.ToString());
            //Console.WriteLine("Utworzono plik: " + "d2-result.csv");
            Console.WriteLine("Zakończono generowania plików wyjściowych");
        }
    }
}
