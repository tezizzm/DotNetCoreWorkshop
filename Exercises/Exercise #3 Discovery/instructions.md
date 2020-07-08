# Exercise #3

## Goal

Explore service registration and discovery.

## Expected Results

Register existing microservice with a service registry. Create a client application so that it discovers the microservice and uses it.

## Introduction

This exercise helps us understand how to register our microservices with the Spring Cloud Services Registry, and also discover those services at runtime.

1. Return back to your `bootcamp-webapi` project root and find the project file.  We will add the following nuget package:

    ```powershell
    dotnet add package Steeltoe.Discovery.ClientCore --version 2.4.4
    ```

2. Navigate to the Program class and make the following edits:

   1. Set the following using statements:

        ```c#
        using Steeltoe.Discovery.Client;
        ```

   2. In the CreateHostBuilder method add the following line before the `webBuilder.UseStartup<Startup>();` call to configure the service discovery client

        ```c#
        webBuilder.AddDiscoveryClient();
        ```

3. In the root directory navigate to the appsettings.json file and add an entry for eureka like the below snippet.  These settings tell Eureka to register our service instance with the Eureka Server

    ```json
    "eureka": {
      "client": {
          "shouldRegisterWithEureka": true,
          "shouldFetchRegistry": false,
          "validateCertificates" : false
      }
    }
    ```

4. Run the following command to create an instance of Service Discovery **note: service name and type may be different depending on platform/operator configuration**

    ```powershell
    cf create-service p-service-registry standard myDiscoveryService
    ```

5. Navigate to the manifest.yml file and in the services section add an entry to bind the application to the newly created instance of the Service Discovery Service.

    ```yml
        - myDiscoveryService
    ```

6. We will now once again push the API application.  Run the `cf push` command to update the api.

7. Go "manage" the `Service Registry` instance from within Apps Manager. Notice our service is now listed!

We now change focus to a front end application that discovers our products API microservice.

1. Navigate to the workspace root.  Create a directory for our new UI with the following command:  `mkdir bootcamp-store`

2. Navigate to the newly created directory using the following command: `cd bootcamp-store`

3. Use the Dotnet CLI to scaffold a basic MVC application with the following command: `dotnet new mvc`.  This will create a new application with the name bootcamp-store.

   ***If you are running a newer version of the .NET CORE runtime, run the following command: `dotnet new globaljson --sdk-version 3.1.202`.  This command will add a global.json file with our configured SDK version to our application root.  By adding this file this will ensure the entire group is on a consistent version of the dotnet SDK.***

4. Navigate to the project file and edit it to add the following nuget packages:

    ```powershell
    dotnet add package Steeltoe.Extensions.Configuration.CloudFoundryCore --version 2.4.4
    dotnet add package Steeltoe.Discovery.ClientCore --version 2.4.4
    ```

5. Edit the Program.cs class adding the following using statements

    ```c#
    using Steeltoe.Discovery.Client;
    using Steeltoe.Extensions.Configuration.CloudFoundry;
    ```

    Above the `webBuilder.UseStartup<Startup>();` line add the following lines to configure the discovery client:

    ```c#
    webBuilder.AddServiceDiscovery();
    webBuilder.AddCloudFoundry();
    ```

6. Create a file named Product.cs that will serve as the model class that represents our store's catalog of products.  The class should have four fields: Id (long), Category (string), Name (string) and Inventory (int).  When complete the class should have the following definition:

    ```c#
    namespace bootcamp_store
    {
        public class Product
        {
            public long Id { get; set; }
            public string Category { get; set; }
            public string Name { get; set; }
            public int Inventory { get; set; }
        }
    }
    ```

7. Edit the HomeController.cs class to retrieve our products from the API.  ***Please take note of the {initials} placeholder and adjust accordingly to match your backend api microservice URL***. First add the appropriate using statements to bring in namespace references.  Then notice the `DiscoveryHttpClientHandler` property.  It maps our call to a discovered service instance and then completes the service request.  Once the request is complete we log the results and pass the data on to our view for display.  In this case the view is an MVC view, you can read more about views [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-3.1).  Once complete the file should look like the following:

    ```c#
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using bootcamp_store.Models;
    using Steeltoe.Common.Discovery;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    namespace bootcamp_store.Controllers
    {
        public class HomeController : Controller
        {
            readonly DiscoveryHttpClientHandler _handler;

            public HomeController(IDiscoveryClient client)
            {
                _handler = new DiscoveryHttpClientHandler(client);
            }

            public async Task<IActionResult> Index()
            {
                var client = new HttpClient(_handler, true);
                var jsonString = await client.GetStringAsync("https://bootcamp-api-{initials}/api/products");
                var products = JsonConvert.DeserializeObject<IList<Product>>(jsonString);
                foreach (var product in products)
                {
                    Console.WriteLine(product);
                }
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

8. Navigate to the views folder and edit the View file named Index.cshtml file to match the below snippet.  This file uses a mix of html and Razor syntax to iterate over and display the products returned from the Products API.  You can read about Razor Syntax [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1)

    ```c#
    @model IList<Product>

    @{
        ViewData["Title"] = "Home Page";
    }

    <div class="jumbotron">
        <div class="container">
            <h1>Bootcamp Store</h1>
            <h2>Welcome to the Bootcamp Store please see a listing of products below</h2>
        </div>
    </div>
    <div class="container">
        <h2>Products</h2>
        <div class="row">
            <div class="col-xs-12 table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Category</th>
                            <th>Inventory</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach(var product in Model)
                        {
                            <tr>
                                <td>@product.Name</td>
                                <td>@product.Category</td>
                                <td>@product.Inventory</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    ```

9. In the root directory navigate to the appsettings.json file and add an entry for eureka like follows.  Notice since we are consuming the service we do not register with Eureka.

    ```json
    "spring": {
      "application": {
        "name" : "bootcamp-store-{initials}"
      }
    },
    "eureka": {
      "client": {
        "shouldRegisterWithEureka": false,
        "shouldFetchRegistry": true,
        "validateCertificates": false
      }
    }
    ```

10. You are ready to now “push” your application.  Create a file at the root of your application name it manifest.yml and edit it as follows, be sure to once again take note of the ***{initials}*** placeholder:  **Note due to formatting issues simply copying the below manifest file may produce errors due to the nature of yaml formatting.  Use the CloudFoundry extension recommend in exercise 1 to assist in the correct formatting**

    ```yml
    applications:
    - name: bootcamp-store-{initials}
      buildpacks:
      - https://github.com/cloudfoundry/dotnet-core-buildpack#v2.3.11
      random-route: true
      memory: 256M
      env:
       ASPNETCORE_ENVIRONMENT: development
      services:
      - myDiscoveryService
    ```

11. Run the cf push command to build, stage and run your application on PCF.  Ensure you are in the same directory as your manifest file and type `cf push`.

12. Once the command has completed, navigate to the url to see the home page with products listed.
