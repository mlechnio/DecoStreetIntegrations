using CsvHelper;
using CsvHelper.Configuration;
using DecoStreetIntegracja.Integrations.Base;
using DecoStreetIntegracja.Integrator.Models;
using DecoStreetIntegracja.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DecoStreetIntegracja.Integrations
{
    public class Eltap_IntegrationShoper : IntegratorShoperBase
    {
        private readonly string CatalogUrl = "https://catalog.eltap.com/api/v1/";
        private readonly string InventoryUrl = "https://inventory.eltap.com/api/v1.0/inventory/";
        private readonly string MediaUrl = "https://media.eltap.com/api/v1/Files/";
        public string FilePath { get; set; } = @"C:\Users\mariu\Downloads\";

        internal override string SourcePath => null;

        internal override string IdPrefix => "Eltap_";

        internal override void Process()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //var availableProductsIds = GetAvailableProductList().Select(x => x.ProductId).ToList();

            //var parentProductIds = GetProductsByIds(availableProductsIds).Select(x => x.PrimaryProductId.HasValue ? x.PrimaryProductId.Value : x.Id).Distinct().ToList();

            //var parentProducts = GetProductsByIds(parentProductIds);

            //var count = 1;
            //foreach (var product in parentProducts)
            //{
            //    Console.WriteLine(count++);
            //    ProcessProduct(product);
            //}
            var names = GetNamesFromCsv();
            foreach (var name in names)
            {
                //var products = GetProductsByName(name);
                var product = GetProductBySku(name);
                ProcessProduct(product);

                //foreach (var product in products.Where(x => x.Name.Trim() == name))
                //foreach (var product in products)
                //{
                //    ProcessProduct(product);
                //}
            }
        }

        private List<string> GetNamesFromCsv()
        {
            var result = new List<string>();
            //using (var reader = new StreamReader(FilePath + "Eltap_narozniki" + ".csv")) 
            //using (var reader = new StreamReader(FilePath + "Eltap_sofyikanapy" + ".csv"))
            //using (var reader = new StreamReader(FilePath + "Eltap_wersalki" + ".csv"))
            //using (var reader = new StreamReader(FilePath + "Eltap_kompletywypoczynkowe" + ".csv"))
            using (var reader = new StreamReader(FilePath + "Eltap_skus" + ".csv"))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = (headerNames, index, context) =>
                {
                },
            }))
            {
                var records = csv.GetRecords<CsvName>().ToList();

                foreach (var item in records)
                {
                    if (!result.Contains(item.Name))
                    {
                        result.Add(item.Name);
                    }
                }
            }

            return result;
        }

        public class CsvName
        {
            public string Name { get; set; }
        }

        internal void ProcessProduct(Product productData)
        {
            var productCode = IdPrefix + productData.Sku;

            try
            {
                Console.WriteLine($"Processing product: {productCode}");
                Thread.Sleep(500);
                var existingProduct = GetExistingProduct(productCode);

                if (existingProduct != null)
                {
                    return;
                }

                if (true)
                {
                    var product = GenerateProductForInsert(productData);

                    var product_id = InsertProduct(product);

                    if (product_id == 0)
                    {
                        return;
                    }

                    var errorAddingImages = false;
                    var erroredImageUrl = new List<string>();

                    foreach (var item in GenerateImagesForInsert2(product_id, productData))
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
                            erroredImageUrl.Add(item.url);
                        }
                    }

                    Logger.Log($"ADDED {product.code}, PRICE: {product.stock.price}, QUANTITY: {product.stock.stock}");

                    Logger.NewProducts.Add($"{product.code}{(errorAddingImages ? $" - wystąpiły błędy przy dodawaniu obrazów - {string.Join(" ", erroredImageUrl)}" : "")}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR {productCode}, <strong>EXCEPTION</strong>: {ex.Message}");
                Logger.LogException(ex);
            }
        }

        internal IEnumerable<ProductImageForInsert> GenerateImagesForInsert2(int product_id, Product productData)
        {
            var productImages = new List<ProductImageForInsert>();

            var count = 0;
            foreach (var mediaId in productData.Images.Take(6))
            {
                Logger.Log($"Getting image {count + 1}/6");
                var mediaData = GetMediaData(mediaId);

                productImages.Add(new ProductImageForInsert
                {
                    product_id = product_id,
                    content = Convert.ToBase64String(mediaData),
                    name = productData.Name + (count > 0 ? " " + count : ""),
                    translations = new ProductTranslations { pl_PL = new Translation { name = productData.Name + (count > 0 ? " " + count : "") } }
                });
                count++;
            }

            return productImages;
        }

        private byte[] GetMediaData(Guid mediaId)
        {
            var client = new RestClient(MediaUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Accept-Language", "pl");
            var request = new RestRequest(mediaId.ToString(), Method.GET);

            //var response = client.Execute<ProductsResponse>(request);
            byte[] response = client.DownloadData(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }

            throw new Exception($"GetMediaData");
        }

        private ProductForInsert GenerateProductForInsert(Product productData)
        {
            var product = new ProductForInsert();
            var price = 10000;
            var stock = 1000;
            var weight = productData.ProductAttributes.Where(x => x.Key == "totalWeight").SingleOrDefault()?.Values[0] ?? "0";

            product.code = IdPrefix + productData.Sku;
            product.pkwiu = string.Empty;
            product.producer_id = 458;
            product.stock.stock = stock;
            product.stock.price = price;
            product.stock.weight = decimal.Parse(weight, CultureInfo.InvariantCulture);
            product.stock.delivery_id = 2;

            product.translations.pl_PL = new Translation
            {
                active = "0",
                name = productData.Name + " ELTAP",
                description = $"<p>{productData.Description}</p>" + string.Join("", productData.ProductAttributes.Select(x => $"<p>{x.Name}: {string.Join(", ", x.Values)}</p>"))
            };

            return product;
        }

        internal List<Product> GetProductsByIds(List<Guid> pids)
        {
            var client = new RestClient(CatalogUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Accept-Language", "pl");
            var request = new RestRequest("Products", Method.GET)
            .AddQueryParameter("pageIndex", "1")
            .AddQueryParameter("itemsPerPage", "250")
            .AddQueryParameter("ids", string.Join(",", pids));
            var response = client.Execute<ProductsResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.Data;
            }

            throw new Exception($"GetProductsByIds, StatusCode: {response.StatusCode}, Content: {response.Content}");
        }

        internal Product GetProductBySku(string sku)
        {
            var client = new RestClient(CatalogUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Accept-Language", "pl");
            var request = new RestRequest($"Products/sku/{sku}", Method.GET);
            var response = client.Execute<Product>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"GetProductsBySku, StatusCode: {response.StatusCode}, Content: {response.Content}");
        }

        internal List<Product> GetProductsByName(string name)
        {
            var client = new RestClient(CatalogUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Accept-Language", "pl");
            var request = new RestRequest("Products", Method.GET)
            .AddQueryParameter("pageIndex", "1")
            .AddQueryParameter("itemsPerPage", "250")
            .AddQueryParameter("searchTerm", name)
            .AddQueryParameter("hasPrimaryProduct", "false");

            var response = client.Execute<ProductsResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.Data;
            }

            throw new Exception($"GetProductsByIds, StatusCode: {response.StatusCode}, Content: {response.Content}");
        }

        internal List<AvailableProduct> GetAvailableProductList()
        {
            var client = new RestClient(InventoryUrl);
            client.AddDefaultHeader("Accept", "application/json");
            var request = new RestRequest("availableproducts?pageIndex=1&itemsPerPage=250", Method.GET);
            var response = client.Execute<AvailableProductsResponse>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.Data;
            }

            throw new Exception($"GetProductList, StatusCode: {response.StatusCode}, Content: {response.Content}");
        }

        public class ProductsResponse
        {
            public List<Product> Data { get; set; }

            public int Total { get; set; }
        }

        public class Product
        {
            public Guid Id { get; set; }

            public Guid? PrimaryProductId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Sku { get; set; }

            public List<Guid> Images { get; set; }

            public List<ProductAttribute> ProductAttributes { get; set; }
        }

        public class ProductAttribute
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public List<string> Values { get; set; }
        }

        public class AvailableProductsResponse
        {
            public List<AvailableProduct> Data { get; set; }

            public int Total { get; set; }
        }

        public class AvailableProduct
        {
            public Guid ProductId { get; set; }

            public string ProductName { get; set; }
        }

        internal override int GetDeliveryId()
        {
            throw new NotImplementedException();
        }

        internal override string GetDescriptionFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetIdFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<string> GetImageUrls(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override bool GetIsInPromo(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetNameFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetPriceBeforeDiscount(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetPriceFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetPromoEndDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override string GetPromoStartDateFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetStockFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }

        internal override decimal GetWeightFromNode(XmlNode sourceNode)
        {
            throw new NotImplementedException();
        }
    }
}
