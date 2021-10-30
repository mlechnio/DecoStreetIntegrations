using DecoStreetIntegracja.Integrator.ApiModels;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        internal void ProcessProduct(XmlNode sourceNode)
        {
            var productCode = IdPrefix + GetIdFromNode(sourceNode);

            //if (productCode != "khdeco4407")
            //{
            //    return;
            //}

            try
            {
                Console.WriteLine($"Processing product: {productCode}");

                Thread.Sleep(1000);

                var existingProduct = GetExistingProduct(productCode);

                if (existingProduct != null)
                {
                    var productForUpdate = GenerateProductForUpdate(existingProduct, sourceNode);
                    if (productForUpdate != null)
                    {
                        UpdateProduct(productForUpdate);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var product = GenerateProductForInsert(sourceNode);
                    var product_id = InsertProduct(product);

                    if (product_id == 0)
                    {
                        return;
                    }

                    Logger.Log($"ADDED {product.code}, PRICE: {product.stock.price}, QUANTITY: {product.stock.stock}");
                    Logger.NewProducts.Add(product.code);

                    foreach (var item in GenerateImagesForInsert2(product_id, sourceNode))
                    {
                        Thread.Sleep(1000);
                        InsertProductImage(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR {productCode}, <strong>EXCEPTION</strong>: {ex.Message}");
                Logger.LogException(ex);
            }
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

        //internal abstract IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode);

        internal abstract IEnumerable<string> GetImageUrls(XmlNode sourceNode);

        private ProductForInsert GenerateProductForInsert(XmlNode sourceNode)
        {
            var product = new ProductForInsert();
            var price = GetPriceFromNode(sourceNode);
            var stock = GetStockFromNode(sourceNode);
            var weight = GetWeightFromNode(sourceNode);

            product.code = IdPrefix + GetIdFromNode(sourceNode);
            product.pkwiu = string.Empty;
            product.stock.stock = stock;
            product.stock.price = price;
            product.stock.weight = weight;
            product.stock.delivery_id = GetDeliveryId();

            product.translations.pl_PL = new Translation
            {
                active = true,
                name = GetNameFromNode(sourceNode),
                description = GetDescriptionFromNode(sourceNode),
            };

            return product;
        }

        internal abstract void Process();

        internal abstract decimal GetPriceFromNode(XmlNode sourceNode);

        internal abstract decimal GetStockFromNode(XmlNode sourceNode);

        internal abstract decimal GetWeightFromNode(XmlNode sourceNode);

        internal abstract string GetIdFromNode(XmlNode sourceNode);

        internal abstract string GetNameFromNode(XmlNode sourceNode);

        internal abstract string GetDescriptionFromNode(XmlNode sourceNode);

        internal abstract int GetDeliveryId();

        private ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode)
        {
            var priceNew = GetPriceFromNode(sourceNode);
            var stockNew = GetStockFromNode(sourceNode);

            var priceChanged = existingProduct.stock.price != priceNew;
            var stockChanged = existingProduct.stock.stock != stockNew;
            var stylePriceChanged = priceChanged ? "style=\"color:red\"" : "";
            var styleStockChanged = stockChanged ? "style=\"color:red\"" : "";
            if (priceChanged || stockChanged)
            {
                Logger.Log($"UPDATING <strong>{existingProduct.code}</strong>, PRICE: {existingProduct.stock.price} -> <strong {stylePriceChanged} >{priceNew}</strong>, STOCK: {existingProduct.stock.stock} -> <strong {styleStockChanged}>{stockNew}</strong>");
                return new ProductForUpdate
                {
                    product_id = existingProduct.product_id,
                    stock = new ProductStock
                    {
                        price = priceNew,
                        stock = stockNew,
                        delivery_id = GetDeliveryId(),
                    }
                };
            }

            return default;
        }

        private Product GetExistingProduct(string productCode)
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
