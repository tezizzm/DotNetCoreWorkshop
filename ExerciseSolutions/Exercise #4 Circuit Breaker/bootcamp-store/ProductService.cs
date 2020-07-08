using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.Common.Discovery;

namespace bootcamp_store
{
    public sealed class ProductService : HystrixCommand<IList<Product>>
    {
        private readonly DiscoveryHttpClientHandler _handler;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IHystrixCommandOptions options, IDiscoveryClient client, ILogger<ProductService> logger) :
            base(options)
        {
            _logger = logger;
            _handler = new DiscoveryHttpClientHandler(client);
            IsFallbackUserDefined = true;
        }

        public async Task<IList<Product>> RetrieveProducts()
        {
            _logger.LogDebug("Retrieving Products from Product Service");
            return await ExecuteAsync();
        }

        protected override async Task<IList<Product>> RunAsync()
        {
            var client = new HttpClient(_handler, false);
            _logger.LogDebug("Processing rest api call to get products");
            var jsonString = await client.GetStringAsync("https://bootcamp-api-mk/api/products");
            var products = JsonConvert.DeserializeObject<IList<Product>>(jsonString);

            foreach (var product in products)
            {
                Console.WriteLine(product);
            }

            return products;
        }

        protected override Task<IList<Product>> RunFallbackAsync()
        {
            _logger.LogDebug("Processing products from fallback method");
            IList<Product> products = new List<Product>()
                {
                    new Product {Id = 1L, Category = "Jewelry", Inventory = 500, Name="Tennis Bracelet"},
                    new Product {Id = 2L, Category = "Clothes", Inventory = 17, Name="Chinos"},
                    new Product {Id = 3L, Category = "Travel", Inventory = 23, Name="Deluxe Suitcase"},
                    new Product {Id = 4L, Category = "Home Goods", Inventory = 53, Name="Light Bulbs"},
                    new Product {Id = 5L, Category = "Bath", Inventory = 20, Name="Hand Towels"}
                };

            foreach (var product in products)
            {
                Console.WriteLine(product.Name);
            }

            return Task.FromResult(products);
        }
    }
}