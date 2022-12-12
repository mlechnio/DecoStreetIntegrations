using DecoStreetIntegracja.Integrator.ApiModels;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;

namespace DecoStreetIntegracja.Integrations.Base
{
    public abstract class IntegratorShoperBase : IDisposable
    {
        private readonly string Api = "http://decostreetpl-53073.shoparena.pl/webapi/rest/";

        internal MemoryStream sourceStream = new MemoryStream();

        internal abstract string SourcePath { get; }

        internal abstract string IdPrefix { get; }

        internal string AuthToken { get; }

        internal virtual NetworkCredential SourceCredentials { get; }

        public IntegratorShoperBase()
        {
            AuthToken = GetAuthToken();
            DownloadSourceFile();
            Process();
            Dispose();
        }

        internal void ProcessProduct(XmlNode sourceNode, bool insertNew = true, bool updateDescriptions = false, bool canChangePromotion = false)
        {
            var productCode = IdPrefix + GetIdFromNode(sourceNode);
            var productsToProcess = new List<string>();
            //productsToProcess.Add("DK282514");
            //productsToProcess.Add("DK211691");
            //productsToProcess.Add("DK190439");
            //productsToProcess.Add("DK9754");
            //productsToProcess.Add("DK24947");
            //productsToProcess.Add("DK63285");
            //productsToProcess.Add("DK185380");
            //productsToProcess.Add("DK127117");
            //productsToProcess.Add("DK127125");
            //productsToProcess.Add("DK150124");
            //productsToProcess.Add("khdeco34405");
            //productsToProcess.Add("khdeco34415");
            //productsToProcess.Add("khdeco34420");
            //productsToProcess.Add("khdeco34440");
            //productsToProcess.Add("khdeco34465");
            //productsToProcess.Add("khdeco34470");
            //productsToProcess.Add("khdeco34480");
            //productsToProcess.Add("khdeco34485");
            //productsToProcess.Add("khdeco34521");

            if (productsToProcess.Any() && !productsToProcess.Contains(productCode))
            {
                return;
            }

            try
            {
                Console.WriteLine($"Processing product: {productCode}");

                Thread.Sleep(500);

                var existingProduct = GetExistingProduct(productCode);

                if (existingProduct != null)
                {
                    var productForUpdate = GenerateProductForUpdate(existingProduct, sourceNode, updateDescriptions, canChangePromotion);
                    if (productForUpdate != null)
                    {
                        UpdateProduct(productForUpdate);
                        if (productForUpdate.RemovePromotion)
                        {
                            DeletePromotion(existingProduct.special_offer.promo_id);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (insertNew)
                    {
                        var product = GenerateProductForInsert(sourceNode);

                        if (product.stock.price == 0)
                        {
                            Logger.Log($"ERROR {productCode}, <strong style=\"color:red\">BAD DATA - PRICE ZERO</strong>");
                            return;
                        }

                        var product_id = InsertProduct(product);

                        if (product_id == 0)
                        {
                            return;
                        }

                        var errorAddingImages = false;

                        foreach (var item in GenerateImagesForInsert2(product_id, sourceNode))
                        {
                            Thread.Sleep(500);
                            try
                            {
                                InsertProductImage(item);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"ERROR ADDING IMAGE {productCode}, <strong>EXCEPTION</strong>: {ex.Message}");
                                Logger.LogException(ex);
                                errorAddingImages = true;
                            }
                        }

                        Logger.Log($"ADDED {product.code}, PRICE: {product.stock.price}, QUANTITY: {product.stock.stock}");

                        Logger.NewProducts.Add($"{product.code}{(errorAddingImages ? " - wystąpiły błędy przy dodawaniu obrazów!!" : "")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR {productCode}, <strong>EXCEPTION</strong>: {ex.Message}");
                Logger.LogException(ex);
            }
        }

        private bool DeletePromotion(int promo_id)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"specialoffers/{promo_id}", Method.DELETE);

            var response = client.Execute<bool>(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Data)
            {
                return response.Data;
            }

            throw new Exception($"DeletePromotion, StatusCode: {response.StatusCode}, Content: {response.Content}, Promo id: {promo_id}");
        }

        internal IEnumerable<ProductImageForInsert> GenerateImagesForInsert2(int product_id, XmlNode sourceNode)
        {
            var productImages = new List<ProductImageForInsert>();

            var count = 0;
            foreach (var url in GetImageUrls(sourceNode))
            {
                productImages.Add(new ProductImageForInsert
                {
                    product_id = product_id,
                    url = url,
                    name = GetNameFromNode(sourceNode) + (count > 0 ? " " + count : ""),
                    translations = new ProductTranslations { pl_PL = new Translation { name = GetNameFromNode(sourceNode) + (count > 0 ? " " + count : "") } }
                });
                count++;
            }

            return productImages;
        }

        internal abstract IEnumerable<string> GetImageUrls(XmlNode sourceNode);

        private ProductForInsert GenerateProductForInsert(XmlNode sourceNode)
        {
            var product = new ProductForInsert();
            var price = GetPriceFromNode(sourceNode);
            var stock = GetStockFromNode(sourceNode);
            var weight = GetWeightFromNode(sourceNode);

            product.code = IdPrefix + GetIdFromNode(sourceNode);
            product.pkwiu = string.Empty;
            product.producer_id = GetProducerId();
            product.stock.stock = stock;
            product.stock.price = price;
            product.stock.weight = 0;
            product.stock.delivery_id = GetDeliveryId();

            product.translations.pl_PL = new Translation
            {
                active = "0",
                name = GetNameFromNode(sourceNode),
                description = GetDescriptionFromNode(sourceNode),
            };

            return product;
        }

        internal abstract void Process();

        internal abstract decimal GetPriceFromNode(XmlNode sourceNode);

        internal abstract decimal GetPriceBeforeDiscount(XmlNode sourceNode);

        internal abstract bool GetIsInPromo(XmlNode sourceNode);

        internal abstract decimal GetStockFromNode(XmlNode sourceNode);

        internal abstract decimal GetWeightFromNode(XmlNode sourceNode);

        internal abstract string GetIdFromNode(XmlNode sourceNode);

        internal abstract string GetNameFromNode(XmlNode sourceNode);

        internal abstract string GetDescriptionFromNode(XmlNode sourceNode);

        internal abstract string GetPromoStartDateFromNode(XmlNode sourceNode);

        internal abstract string GetPromoEndDateFromNode(XmlNode sourceNode);

        internal abstract int GetDeliveryId();

        internal virtual int? GetProducerId() => null;

        private ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode, bool updateDescription, bool canChangePromotion = false)
        {
            var inPromo = GetIsInPromo(sourceNode);
            var priceNew = inPromo ? GetPriceBeforeDiscount(sourceNode) : GetPriceFromNode(sourceNode);
            var stockNew = GetStockFromNode(sourceNode);

            var priceChanged = existingProduct.stock.price != priceNew;
            var stockChanged = existingProduct.stock.stock != stockNew;
            var promoPriceChanged = canChangePromotion && existingProduct.stock.comp_promo_price != GetPriceFromNode(sourceNode);
            var stylePriceChanged = priceChanged ? existingProduct.stock.price < priceNew ? "style=\"color:red\"" : "style=\"color:green\"" : "";
            var styleStockChanged = stockChanged ? existingProduct.stock.stock > stockNew ? "style=\"color:red\"" : "style=\"color:green\"" : "";

            var promoEndDate = canChangePromotion && GetPromoEndDateFromNode(sourceNode).Split('-').Length == 3 ? new DateTime(int.Parse(GetPromoEndDateFromNode(sourceNode).Split('-')[0]), int.Parse(GetPromoEndDateFromNode(sourceNode).Split('-')[1]), int.Parse(GetPromoEndDateFromNode(sourceNode).Split('-')[2])) : DateTime.MinValue;
            var addingPromo = canChangePromotion && inPromo && (promoPriceChanged || existingProduct.special_offer == null) && promoEndDate > DateTime.Today;
            var removingPromo = canChangePromotion && (!inPromo && existingProduct.special_offer != null);

            if (updateDescription || addingPromo || removingPromo || priceChanged || stockChanged)
            {
                Logger.Log($"UPDATING <strong>{existingProduct.code}</strong>, PRICE: {existingProduct.stock.price} -> <strong {stylePriceChanged} >{priceNew}</strong>, STOCK: {existingProduct.stock.stock} -> <strong {styleStockChanged}>{stockNew}</strong>");
                //if (existingProduct.stock.weight != 30)
                //{
                //    Logger.Log($"UPDATING weight to 30");
                //}
                if (addingPromo)
                {
                    Logger.Log($"UPDATING ADDING PROMOTION: PRICE: <strong style=\"color:green\">{GetPriceFromNode(sourceNode)}</strong>");

                    if (GetPriceBeforeDiscount(sourceNode) - GetPriceFromNode(sourceNode) < 0)
                    {
                        Logger.Log($"ERROR {existingProduct.code}, <strong style=\"color:red\">BAD DATA - PROMO PRICE HIGHER THAN REGULAR PRICE</strong>");
                        return default;
                    }
                }
                if (removingPromo)
                {
                    Logger.Log($"UPDATING REMOVING PROMOTION");
                }

                var productToUpdate = new ProductForUpdate
                {
                    product_id = existingProduct.product_id,
                    stock = new ProductStock
                    {
                        price = priceNew,
                        stock = stockNew,
                        delivery_id = GetDeliveryId(),
                        weight = existingProduct.stock.weight,
                    },
                    special_offer = addingPromo ? new SpecialOffer
                    {
                        discount = GetPriceBeforeDiscount(sourceNode) - GetPriceFromNode(sourceNode),
                        date_from = GetPromoStartDateFromNode(sourceNode),
                        date_to = GetPromoEndDateFromNode(sourceNode),
                    } : null,
                    RemovePromotion = removingPromo,
                };

                if (updateDescription)
                {
                    productToUpdate.translations = new ProductTranslations();
                    productToUpdate.translations.pl_PL = new Translation
                    {
                        active = existingProduct.translations.pl_PL.active,
                        name = existingProduct.translations.pl_PL.name,
                        seo_url = existingProduct.translations.pl_PL.seo_url,
                        short_description = existingProduct.translations.pl_PL.short_description,
                        description = GetDescriptionFromNode(sourceNode),
                    };
                }

                return productToUpdate;
            }

            return default;
        }

        internal Product GetExistingProduct(string productCode)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest("products?filters={\"stock.code\":\"" + productCode + "\"}", Method.GET);
            var response = client.Execute<ProductListResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data.count > 0)
                {
                    return response.Data.list.SingleOrDefault(x => x.code == productCode);
                }
                return default;
            }

            throw new Exception($"GetExistingProduct, StatusCode: {response.StatusCode}, Content: {response.Content}, Item: {productCode}");
        }

        private int InsertProduct(ProductForInsert product)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"products", Method.POST);

            request.AddJsonBody(product);

            var response = client.Execute<int>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"InsertProduct, StatusCode: {response.StatusCode}, Content: {response.Content}, Item: {new JsonSerializer().Serialize(product)}");
        }

        private int InsertProductImage(ProductImageForInsert image)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"product-images", Method.POST);

            request.AddJsonBody(image);

            var response = client.Execute<int>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"InsertProductImage, StatusCode: {response.StatusCode}, Content: {response.Content}, Item: {new JsonSerializer().Serialize(image)}");
        }

        private bool UpdateProduct(ProductForUpdate product)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"products/{product.product_id}", Method.PUT);

            request.AddJsonBody(product);

            var response = client.Execute<bool>(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Data)
            {
                return response.Data;
            }

            throw new Exception($"UpdateProduct, StatusCode: {response.StatusCode}, Content: {response.Content}, Item: {new JsonSerializer().Serialize(product)}");
        }

        internal bool DeleteProduct(int productId)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"products/{productId}", Method.DELETE);

            var response = client.Execute<bool>(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Data)
            {
                return response.Data;
            }

            throw new Exception($"DeleteProduct, StatusCode: {response.StatusCode}, Content: {response.Content}, product_id: {productId}");
        }

        private void DownloadSourceFile()
        {
            using (var webClient = new WebClient())
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Console.WriteLine("Rozpoczecie pobierania pliku " + SourcePath);

                webClient.Credentials = SourceCredentials;

                sourceStream = new MemoryStream(webClient.DownloadData(SourcePath));

                Console.WriteLine("Pobrano");
            }
        }

        private string GetAuthToken()
        {
            var client = new RestClient(Api);
            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["api_login"], ConfigurationManager.AppSettings["api_pass"]);
            var request = new RestRequest("auth", Method.POST);
            var response = client.Execute<AuthResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.access_token;
            }

            throw new Exception($"GetAuthToken, StatusCode: {response.StatusCode}, Content: {response.Content}");
        }

        public virtual void Dispose()
        {
            if (sourceStream != null)
            {
                sourceStream.Dispose();
            }
        }
    }
}
