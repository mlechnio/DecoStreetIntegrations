using DecoStreetIntegracja.Integrator.ApiModels;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using RestSharp.Authenticators;
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

                    foreach (var item in GenerateImagesForInsert(product_id, sourceNode))
                    {
                        Thread.Sleep(1000);
                        InsertProductImage(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR {productCode}, EXCEPTION: {ex.Message}");
                Logger.LogException(ex);
            }
        }

        internal abstract IEnumerable<ProductImageForInsert> GenerateImagesForInsert(int product_id, XmlNode sourceNode);

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

        private ProductForUpdate GenerateProductForUpdate(Product existingProduct, XmlNode sourceNode)
        {
            var priceNew = GetPriceFromNode(sourceNode);
            var stockNew = GetStockFromNode(sourceNode);

            if (existingProduct.stock.price != priceNew || existingProduct.stock.stock != stockNew)
            {
                Logger.Log($"UPDATING <strong>{existingProduct.code}</strong>, PRICE: {existingProduct.stock.price} -> <strong>{priceNew}</strong>, STOCK: {existingProduct.stock.stock} -> <strong>{stockNew}</strong>");
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

        private Product GetExistingProduct(string productCode)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest("products?filters={\"stock.code\":\"" + productCode + "\"}", Method.GET);
            var response = client.Execute<ProductListResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Data.count > 0)
            {
                return response.Data.list.SingleOrDefault(x => x.code == productCode);
            }

            return default;
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
            // log that something went wrong
            return 0;
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
            // log that something went wrong
            return 0;
        }

        private bool UpdateProduct(ProductForUpdate product)
        {
            var client = new RestClient(Api);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AuthToken));
            var request = new RestRequest($"products/{product.product_id}", Method.PUT);

            request.AddJsonBody(product);

            var response = client.Execute<bool>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            // log that something went wrong
            return false;
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
            return response.Data.access_token;
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
