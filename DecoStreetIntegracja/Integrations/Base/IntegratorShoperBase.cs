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
            Console.WriteLine("Rozpoczęcie generowania plików wyjściowych");
            UpdateProducts();
            Dispose();
        }

        internal void ProcessProduct(XmlNode sourceNode)
        {
            Console.WriteLine($"Processing product: {sourceNode["numer"].InnerText}");
            var existingProduct = GetExistingProduct(IdPrefix + sourceNode["numer"].InnerText);

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
                //      add product
                //      add product images
            }
        }

        public abstract void UpdateProducts();

        public abstract ProductForUpdate GenerateProductForUpdate(Product product, XmlNode sourceNode);

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
