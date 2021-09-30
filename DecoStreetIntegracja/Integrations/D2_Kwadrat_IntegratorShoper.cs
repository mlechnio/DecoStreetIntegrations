using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class D2_Kwadrat_IntegratorShoper : IntegratorShoperBase
    {
        internal override string SourcePath => "ftp://thyone.iq.pl/updatexml.xml";

        internal override NetworkCredential SourceCredentials => new NetworkCredential("dkwadrat_UD2", "zhz907h");

        internal override string IdPrefix => "DK";

        private XmlElement GenerateONode(XmlDocument xmlDoc, XmlNode sourceNode)
        {
            var nodeO = xmlDoc.CreateElement("o");
            var attrID = xmlDoc.CreateAttribute("id");
            attrID.Value = IdPrefix + sourceNode["numer"].InnerText;
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

        public override void UpdateProducts()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("//produkty/produkt");

            foreach (XmlNode sourceNode in xmlNodeList)
            {
                if (EnabledData.enabledD2Categories.Contains(sourceNode["kategoria_glowna"].InnerText))
                {
                    ProcessProduct(sourceNode);
                }
            }
        }

        public override ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode)
        {
            var priceNew = decimal.Parse(sourceNode["cena_brutto"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
            var stockNew = decimal.Parse(sourceNode["stan"].InnerText.Replace(",", ".").Replace(".00", "").Replace("+", ""), CultureInfo.InvariantCulture);

            if (existingProduct.stock.price != priceNew || existingProduct.stock.stock != stockNew)
            {
                return new ProductForUpdate
                {
                    product_id = existingProduct.product_id,
                    stock = new ProductStock
                    {
                        price = priceNew,
                        stock = stockNew,
                    }
                };
            }

            return default;
        }
    }
}
