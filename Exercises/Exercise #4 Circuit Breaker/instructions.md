# Exercise #4

## Goal

Explore fault and latency tolerance along with service health monitoring.

## Expected Results

Change the UI service so that external calls are wrapped in a Hystrix Command to provide fault and latency tolerance.  Bind existing UI application with an instance of Hystrix Service to allow monitoring of the external calls.

## Introduction

This exercise helps us understand how to wrap our external calls in Hystrix Commands and allows us to view metrics on the health of those calls.

1. Return back to your `bootcamp-store` project root and find the project file.  We will add the following nuget package:

    ```powerhsell
    dotnet add package RabbitMQ.Client --version 5.2.0
    dotnet add package Steeltoe.CircuitBreaker.Hystrix.MetricsStreamCore --version 2.4.4
    dotnet add package Steeltoe.CircuitBreaker.HystrixCore --version 2.4.4
    ```

2. In the root of the project create a file called ProductService.cs with the below implementation.  This class will act as an abstraction to our product retrieval from our products API.  The logic for retrieving products has been moved to our RunAsync method and there is now a method to return placeholder values from the RunFallbackAsync method in cases of failures.

    ```c#
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
                var jsonString = await client.GetStringAsync("https://bootcamp-api-{initials}/api/products");
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
    ```

3. Navigate to the Startup class and make the following changes:

   1. set the following using statements:

        ```c#
        using Steeltoe.CircuitBreaker.Hystrix;
        ```

   2. In the ConfigureServices method use an extension method to add the service class and Hystrix metrics stream to the DI Container with the following lines of code.

        ```c#
        services.AddHystrixCommand<ProductService>("ProductService", Configuration);
        services.AddHystrixMetricsStream(Configuration);
        ```

   3. In the Configure method add swagger to the middleware pipeline by adding the following code snippet

        ```c#
        //make sure the request context is set before adding the metrics stream middleware
        app.UseHystrixRequestContext();
        ...
        app.UseHystrixMetricsStream();
        ```

4. Navigate to the appsettings.json file and add an entry for hystrix like follows.  It configures the hystrix command and tells the stream to ignore certificates.

    ```json
    "hystrix": {
        "stream": {
            "validate_certificates": false
        },
        "command": {
            "ProductService": {
            "threadPoolKeyOverride": "ProductServiceTPool"
            }
        }
    }
    ```

5. Update the HomeController to now utilize the ProductService as follows:

    ```c#
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using bootcamp_store.Models;
    using Microsoft.Extensions.Logging;

    namespace bootcamp_store.Controllers
    {
        public class HomeController : Controller
        {
            private readonly ProductService _productService;
            private readonly ILogger<HomeController> _logger;

            public HomeController(ProductService productService, ILogger<HomeController> logger)
            {
                _productService = productService;
                _logger = logger;
            }

            public async Task<IActionResult> Index()
            {
                var products = await _productService.RetrieveProducts();
                _logger.LogDebug("Retrieved Products");
                return View(products);
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
    ```

6. Run the following command to create an instance of the Circuit Breaker
 **note: service name and type may be different depending on platform/operator configuration**

    ```powershell
    cf create-service p-circuit-breaker-dashboard standard myHystrixService
    ```

7. Navigate to the manifest.yml file and in the services section add an entry to bind the application to the newly created instance of the Hystrix Service.

    ```yml
    - myHystrixService
    ```

8. Run the cf push command to build, stage and run your application on PCF.  Ensure you are in the same directory as your manifest file and type `cf push`.

9. Go "manage" the `Circuit Breaker` instance from within Apps Manager. Notice the dashboard listings.  At this point you may see a generic "loading..." message. Once the UI application has finished updating, navigate to it and refresh the page a couple of times.  In the Circuit Breaker instance you should start seeing activity being monitored.

*Optional: Explore advanced features of the Circuit Breaker:*

1. We will be using an HTTP Load Generator called Hey to achieve a sizable amount of load against our application.  Download the Hey Windows [release](https://storage.googleapis.com/jblabs/dist/hey_win_v0.1.2.exe).  Documentation on the hey can be found on [Github](https://github.com/rakyll/hey)

2. It is recommended that you add Hey to your path and for ease in using the command rename the executable from 'hey_win_v0.1.2.exe' to 'hey'.  *Reach out to one of the instructors if you need assistance with adding the executable to the path.*

3. Run the following command to begin hitting our UI with load. *Note you will have to edit the host name in angle brackets to match that of your UI application as well as the foundation URL*:

    ```powershell
    hey -c 12 -z 60s https://<ui-host-name>.apps.<foundation-url>
    ```

4. While the hey command is generating load, go back to the application UI and refresh to see the fallback logic in action.  Also view the Circuit and take note of the pass through calls being made.

5. Confirm that the hey command is complete and that the circuit has reentered a healthy state.  Now refresh the application UI to see the standard response.
