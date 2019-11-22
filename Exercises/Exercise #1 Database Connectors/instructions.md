# Exercise #1

## Goal

Explore Steeltoe Connectors by connecting to an external persistence store.

## Expected Results

Create our backed microservice, a Web API application, bind it to a persistent store and utilize the Steeltoe connectors to discover and connect the Web API to the persistent store.

## Introduction

In this exercise we create a Web API application that will serve as the backend microservice for the remaining exercises in this workshop.  We will bind a persistent store to our microservice and observe the Steeltoe connectors discover and connect to the persistent store.  **Note we assume there is an instance of MySql available to bind to your application.  If not the code will detect this and connect and utilize an in memory data store.**

1. Create a directory for our new API with the following command:  `mkdir bootcamp-webapi`

2. Navigate to the newly created directory using the following command: `cd bootcamp-webapi`

    ***In the bootcamp-webapi folder run the following command: `dotnet new globaljson --sdk-version 2.2.402`.  This command will add a global.json file with our configured SDK version to our application root.  By adding this file this will ensure the entire group is on a consistent version of the dotnet SDK.***

3. Use the Dotnet CLI to scaffold a basic Web API application with the following command: `dotnet new webapi`.  This will create a new application with name bootcamp-webapi.  **Note the project will take the name of the folder that the command is run from unless given a specific name**

4. Now utilize the Dotnet CLI to add nuget packages to your project.  Nuget is the .NET package manager for managing external libraries.  Each command will update your project file (ending in .csproj) with the package name and version.  When restoring and publishing your project these packages will be downloaded on to the target machine.  More information about Nuget can be found [here](https://www.nuget.org/)

    ```powershell

    dotnet add package Pomelo.EntityFrameworkCore.MySql --version 2.2.0

    dotnet add package Steeltoe.CloudFoundry.ConnectorCore --version 2.2.0

    dotnet add package Steeltoe.CloudFoundry.Connector.EFCore --version 2.2.0

    dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 2.2.6
    ```

5. In the Program.cs class add the following using statement and edit the CreateWebHostBuilder method in the following way.  The using statement allows us to use types of a given namespace without fully qualifying a given type [see](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive) for information on the c# using statement.

    ```c#
    using Steeltoe.Extensions.Configuration.CloudFoundry;
    ```

    ```c#
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseCloudFoundryHosting()
            .AddCloudFoundry()
            .UseStartup<Startup>();
    ```

    **Take note of the UseCloudFoundryHosting which is an extension that allows us to listen on a configured port.**

6. Create a file named `Product.cs` that will serve as the entity class that represents our store's catalog of products.  The class should have four fields: Id (long), Category (string), Name (string) and Inventory (int).  When complete the class should have the following definition:

    ```c#
    namespace bootcamp_webapi
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

7. Create a file named `ProductContext.cs` that will serve as our context class that will be utilized by Entity Framework to store seed our database.  The class should extend DbContext, define 2 constructors, one without parameters and another which takes DbContextOptions and creates a DbSet of Products.  Finally the class will override the OnModelCreating method.  In this method we will set up data to be seeded when we later create and execute our database migrations.  For a specific discussion on DbContext see the following [article](https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext) and to further look at Entity Framework Core see the following [article](https://docs.microsoft.com/en-us/ef/core/).  When complete it should have the following definition.

    ```c#
    using Microsoft.EntityFrameworkCore;

    namespace bootcamp_webapi
    {
        public class ProductContext : DbContext
        {
            public ProductContext(DbContextOptions options) : base(options)
            {
            }

            protected ProductContext()
            {
            }

            public DbSet<Product> Products { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var products = new []
                {
                    new {Id = 1L, Category = "Books", Inventory = 5, Name="The Ultimate Guide To Budget Travel"},
                    new {Id = 2L, Category = "Sports", Inventory = 4, Name="Upper Deck Baseball Set"},
                    new {Id = 3L, Category = "Groceries", Inventory = 2, Name="Gatorade"},
                    new {Id = 4L, Category = "Electronics", Inventory = 50, Name="Google Pixel 3"},
                    new {Id = 5L, Category = "Home and Garden", Inventory = 20, Name="Kitchenette Stand Mixer"}
                };

                modelBuilder.Entity<Product>().HasData(products);
            }
        }
    }
    ```

8. Once the ProductContext class has been created, we will create an interface to aid with dependency management among other things.  In Visual Studio or Visual Studio Code you can hover over the ProductContext word in the class definition `public class ProductContext : DbContext` and select `Extract Interface`.  **Note, in Visual Studio this menu item may be nested under a Refactor menu or somewhere else entirely if you have other tools installed like Resharper for example.**  When completed the interface should have the following definition:

    ```c#
    using Microsoft.EntityFrameworkCore;

    namespace bootcamp_webapi
    {
        public interface IProductContext
        {
            DbSet<Product> Products { get; set; }
        }
    }
    ```

9. Navigate to the Startup class and edit the file as follows: 

   1. set the following using statements:

        ```c#
        using Microsoft.EntityFrameworkCore;
        using Steeltoe.CloudFoundry.Connector;
        using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
        using Steeltoe.CloudFoundry.Connector.Services;
        ```

   2. The Startup.ConfigureServices method is responsible for defining the services the app uses.  In the ConfigureServices method use the following code snippet to add the database context class and utilize the Steeltoe connector.  **Note the if else statement, this is where we determine if our application is bound to a MySql instance and register the corresponding configuration.**  For a in depth look at dependency injection in ASP.NET Core see the following [article](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)

        ```c#
        var isMySqlBound = Configuration.GetServiceInfos<MySqlServiceInfo>().Any();
        services.AddDbContext<ProductContext>(options =>
        {
            if (isMySqlBound)
                options.UseMySql(Configuration);
            else
                options.UseSqlite("DataSource=:memory:");

        }, isMySqlBound ? ServiceLifetime.Scoped : ServiceLifetime.Singleton);
        ```

10. Database migrations allow us to keep our database in sync without our model.  Create a file called `EnsureMigration.cs`.  We will utilize this class when we set up our middleware pipeline to make sure all Entity Framework Migrations have been executed.  The class should have the following definition.  For a discussion on Entity Framework Migrations see the following [article](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

    ```c#
    using System.Data.Common;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class EnsureMigrations
    {
        static DbConnection _connection;
        public static void EnsureMigrationOfContext<T>(this IWebHost webHost) where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            using (var serviceScope = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var productContext = serviceScope.ServiceProvider.GetService<T>();
                _connection = productContext.Database.GetDbConnection();
                _connection.Open();
                productContext.Database.Migrate();
            }
        }
    }
    ```

11. Navigate back to the Program.cs.  We will utilize the EnsureMigration class to apply our database migrations once the application has started up.  To do this we have to modify our main method where we set up our Web Host.  When complete the Main Method should look like the following snippet:

    ```c#
    IWebHost webHost = CreateWebHostBuilder(args).Build();
    webHost.EnsureMigrationOfContext<ProductContext>();
    webHost.Run();
    ```

12. We must now create the actual migrations themselves.  To do this we run the command `dotnet ef migrations add InitialCreation` which will create a folder named Migrations in our solution.  This folder will hold the initial migration files that create our database based on the definition of our Product context and entity class.  For further reading on creating migrations [see](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/#create-a-migration)

13. A controller is used to define and group a set of actions which are methods that handle incoming requests.  For an in depth view of ASP.NET Core MVC [see](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-2.1) and for a Controller specific discussion [see](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-2.1).  In the controllers folder create a new class and name it `ProductsController.cs` and then paste the following contents into the file:

    ```c#
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    namespace bootcamp_webapi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ProductsController : Controller
        {
            private readonly ProductContext _context;
            public ProductsController([FromServices] ProductContext context)
            {
                _context = context;
            }

            // GET api/products
            [HttpGet]
            public IActionResult Get()
            {
                var connection = _context.Database.GetDbConnection();
                Console.WriteLine($"Retrieving product catalog from {connection.DataSource}/{connection.Database}");
                return Json(_context.Products.ToList());
            }
        }
    }
    ```

14. In the root directory navigate to the appsettings.json file and add an entry for spring like the below snippet.  **Take note of *{Initials}* placeholder in the configuration file and replace with yor initials accordingly.  This will avoid conflicts if the workshop attendee applications are targeting the same space**

    ```json
    "spring": {
      "application": {
        "name": "bootcamp-api-{initials}"
      }
    }
    ```

15. In this step we will create our MySql database provided we have the capability to do so in the target environment.  Recall, from earlier exercises, if there is no bound MySql service then we use the in memory database and can safely ***skip*** this step.

    **In the following command, the service name and type may be different depending on platform/operator configuration.  Run the following command `cf marketplace` to find the specific instance name and plan for your platforms installation of MySql.  Once again take note of and replace the *{Initials}* placeholder in the command.**

    ```powershell
    cf create-service p.mysql db-small products-db-{Initials}
    ```

16. You are ready to now “push” your application.  Create a file at the root of your application name it manifest.yml and edit it to match the snippet below.  The settings in this file instruct the cloud foundry cli how to stage and deploy your application.  **Note due to formatting issues simply copying the below manifest file MAY produce errors due to the nature of yaml formatting.  Use the CloudFoundry extension recommend in exercise 0 to assist in the correct formatting.  Once again take note of and replace the *{Initials}* placeholder in the command.**

    ```yml
    applications:
    - name: bootcamp-api-{initials}
      random-route: true
      buildpacks:
      - https://github.com/cloudfoundry/dotnet-core-buildpack
      instances: 1
      memory: 256M
      env:
        ASPNETCORE_ENVIRONMENT: development
      #### Uncomment following two lines if service is available from the previous step
      #services:
      #- products-db-{initials}
    ```

17. Run the cf push command to build, stage and run your application on PCF.  Ensure you are in the same directory as your manifest file and type `cf push`.

    ***The operation to create the MySql instance can be timely.  If you attempt to push the application while the service is still being created, you will see errors indicating that an operation on the service is pending.***

18. Once the `cf push` command has completed navigate to the given application url at which point you will see a 401 Not Found Status. This is expected since we haven't configured a default route and will be updated in the next lab.  If you navigate to the api/products path you should see a json array of products that are pulled from the backend store.
