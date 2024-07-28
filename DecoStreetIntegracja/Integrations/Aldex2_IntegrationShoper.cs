using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Aldex2_IntegrationShoper : IntegratorShoperBase
    {
        internal override string SourcePath => @"C:\Users\mariu\Downloads\produkty_xml_3_23-06-2024_13_17_09_pl.xml";

        internal override string IdPrefix => "Aldex";

        private List<string> ImportNames = new List<string> { "KINKIET LOOP BABY PINK", "PLAFON TULL 4 (1+2) BLACK", "PLAFON TULL 4 BLACK", "PLAFON TULL 3 (1+2) BLACK", "PLAFON TULL 3 BLACK", "LAMPKA BIURKOWA LOOP BABY PINK", "LAMPKA BIURKOWA LOOP DUSTY BLUE", "LAMPKA BIURKOWA LOOP RED WINE", "LAMPKA BIURKOWA LOOP CORAL", "LAMPKA BIURKOWA UNA BEIGE", "LAMPA WISZĄCA UNA 2 BEIGE", "LAMPA WISZĄCA UNA 4 BEIGE L", "PLAFON UNA 2 BEIGE", "KINKIET UNA BEIGE", "PLAFON STICK ALL BLACK S", "PLAFON STICK ALL WHITE S", "PLAFON STICK BLACK S", "PLAFON STICK WHITE S", "KINKIET BALL II BEIGE S", "KINKIET BALL II DUSTY BLUE S", "KINKIET BALL II RED WINE S", "KINKIET BALL II MUSTARD S", "KINKIET BALL II LILAC S", "KINKIET BALL II PISTACHIO S", "KINKIET BALL II CORAL S" };

        internal override void Process()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(sourceStream);
            var xmlNodeList = xmlDocument.SelectNodes("/products/product");

            var list = xmlNodeList.Cast<XmlNode>().ToList();

            Logger.Log($"To process: {list.Count}");

            for (int i = 0; i < list.Count; i++)
            {
                if (ImportNames.Contains(GetNameFromNode(list[i])))
                {
                    ProcessProduct(list, i, list.Count);
                }
            }
        }

        //internal override IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode)
        //{
        //    var productImages = new List<ProductImageForInsert>();

        //    productImages.Add(new ProductImageForInsert
        //    {
        //        product_id = product_id,
        //        url = sourceNode["zdjecie"].InnerText.Replace(" ", "%20"),
        //        name = GetNameFromNode(sourceNode),
        //        translations = new ProductTranslations { pl_PL = new Translation { name = GetNameFromNode(sourceNode) } }
        //    });

        //    return productImages;
        //}

        internal override int GetDeliveryId()
        {
            return 12;
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            return sourceNode["desc"].InnerText;
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            return sourceNode["sku"].InnerText;
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            return sourceNode["name"].InnerText;
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            return decimal.Parse(sourceNode["retailPriceGross"].InnerText.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            return 900;
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            return 30;
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            if (sourceNode["photos"].ChildNodes.Count > 0)
            {
                yield return sourceNode["photos"].ChildNodes[0].InnerText.Replace(" ", "%20");

                if (sourceNode["photos"].ChildNodes.Count > 1)
                {
                    for (var i = 1; i < sourceNode["photos"].ChildNodes.Count; ++i)
                    {
                        yield return sourceNode["photos"].ChildNodes[i].InnerText.Replace(" ", "%20");
                    }
                }
            }
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            return false;
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override int? GetProducerId()
        {
            return 471;
        }
    }
}
