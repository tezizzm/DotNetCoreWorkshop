using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.Extensions.Configuration.ConfigServer;
using NJsonSchema;
using NSwag.AspNetCore;

namespace bootcamp_webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var isMySqlBound = Configuration.GetServiceInfos<MySqlServiceInfo>().Any();
            services.AddDbContext<ProductContext>(options =>
            {
                if (isMySqlBound)
                    options.UseMySql(Configuration);
                else
                    options.UseSqlite("DataSource=:memory:");

            }, isMySqlBound ? ServiceLifetime.Scoped : ServiceLifetime.Singleton);

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

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3(settings => settings.Path = "");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
