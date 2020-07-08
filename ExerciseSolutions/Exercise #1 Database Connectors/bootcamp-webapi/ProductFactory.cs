using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace bootcamp_webapi
{
    public class ProductFactory : IDesignTimeDbContextFactory<ProductContext>
    {
        public ProductContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>();
            optionsBuilder.UseSqlite("DataSource=:memory:");

            return new ProductContext(optionsBuilder.Options);
        }
    }
}