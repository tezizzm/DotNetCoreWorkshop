# Exercise #2

## Goal

Explore Externalized Configuration by working with Spring Cloud Config Server

## Expected Results

Create an instance of Spring Cloud Services Configuration Server and bind our API application to that instance of the Configuration Server.

## Introduction

In this exercise we explore how Configuration Server pulls configuration from a backend repository.  Also observe how we utilize Steeltoe to connect to Configuration Server and manipulate how we retrieve configuration.

1. We will once again be working with our WebAPI based Product microservice.  In the root directory of our microservice add the following Nuget packages.

    ```powershell
    dotnet add package NSwag.AspNetCore --version 13.6.2
    dotnet add package Steeltoe.Extensions.Configuration.ConfigServerCore --version 2.4.4
    ```

2. Edit the Program.cs class.

   1. Add the following using statement:

        ```c#
        using Microsoft.Extensions.DependencyInjection;
        using Steeltoe.Extensions.Configuration.ConfigServer;
        ```

   2. Create a method to configure the LogBuilder and edit the CreateWebHostBuilder method to utilize the extension method to add Config Server:

        ```c#
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.AddConfigServer(GetLoggerFactory());
                    webBuilder.UseStartup<Startup>();
                });

        public static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace)
                    .AddConsole()
                    .AddDebug();
            });
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }
        ```

3. Create a file named `ApiSettings.cs`.  This class will be used to store configuration information for our API.  The class should have the following definition:

    ```c#
    namespace bootcamp_webapi
    {
        public class ApiSettings
        {
            public string Version { get; set; }
            public string Title { get; set; }
        }
    }
    ```

4. Navigate to the Startup class and make the following code changes:

   1. Set the following using statements:

        ```c#
        using Steeltoe.Extensions.Configuration.ConfigServer;
        using NJsonSchema;
        using NSwag.AspNetCore;
        ```

   2. In the ConfigureServices method make the following code changes: use an extension method to add swagger to the DI Container with the following lines of code *before* the following line of code `services.AddControllers();`.

        ```c#
        var apiSettings = Configuration
            .GetSection("api")
            .Get<ApiSettings>();

        services.AddSwaggerDocument(config => 
        {
            config.PostProcess = document =>
            {
                document.Info.Version = apiSettings?.Version;
                document.Info.Title = apiSettings?.Title;
                document.Info.Description = "A simple ASP.NET Core web API";
                document.Schemes.Clear();
                document.Schemes.Add(NSwag.OpenApiSchema.Https);
            };
        });
        ```

   3. The Configure method is used to specify how the app responds to specific HTTP requests.  In the Configure method retrieve configuration from Config Server and add swagger to the middleware pipeline by adding the following code snippet just before the `app.UseHttpsRedirection();;` line.  This configures the pipeline to serve the Swagger specification based on our application.  For information on application start up in ASP.NET Core [see](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1)

        ```c#
        app.UseOpenApi();
        app.UseSwaggerUi3(settings => settings.Path = "");
        ```

5. In the root directory navigate to the appsettings.json file and add an entry for spring and spring cloud config like the below snippet.  These settings tell Eureka to register our service instance with the Eureka Server.  Where appropriate replace the `{initials}` placeholder with your initials.

    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Warning"
        }
      },
      "AllowedHosts": "*",
      "spring": {
        "application": {
          "name": "bootcamp-api-{initials}"
        }
      },
      "cloud": {
        "config": {
          "name": "bootcamp-api-{initials}",
          "env": "development",
          "validateCertificates" : false
        }
      }
    }
    ```

6. In the application root create a file and name it `config.json` and edit it in the following way.  The settings in this file will tell our instance of Spring Cloud Config that we will be connecting to a git repository at the location specified in the uri field.

    ```json
    {
        "git": {
            "uri": "https://github.com/tezizzm/bootcamp-repo"
        }
    }
    ```

7. Run the following command to create an instance of Spring Cloud Config Server with settings from config.json **note: service name and type may be different depending on platform/operator configuration**

    ```bat
    cf create-service p-config-server standard myConfigServer -c .\config.json
    ```

8. You are ready to now “push” your application.  Edit the manifest.yml file to match the snippet below and remember to replace the initials placeholders.  **Note due to formatting issues simply copying the below manifest file may produce errors due to the nature of yaml formatting.  Use the CloudFoundry extension recommend in exercise 1 to assist in the correct formatting**

    ```yml
    applications:
   - name: bootcamp-api-{initials}
     random-route: true
     buildpacks:
     - https://github.com/cloudfoundry/dotnet-core-buildpack$v2.3.11
     instances: 1
     memory: 256M
     env:
       ASPNETCORE_ENVIRONMENT: development
     services:
      #### Uncomment following line if service is available
      #- products-db-{initials}
     - myConfigServer
    ```

9. Run the cf push command to build, stage and run your application on PCF.  Ensure you are in the same directory as your manifest file and type `cf push`.

10. Once the `cf push` command has completed navigate to the given url and you should see the Swagger page.  To confirm the configuration have a look at the configured git repository in the config.json file.
