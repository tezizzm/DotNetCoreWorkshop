using Microsoft.EntityFrameworkCore;

namespace bootcamp_webapi
{

    public class ProductContext : DbContext, IProductContext
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
            var products = new[]
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