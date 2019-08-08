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
