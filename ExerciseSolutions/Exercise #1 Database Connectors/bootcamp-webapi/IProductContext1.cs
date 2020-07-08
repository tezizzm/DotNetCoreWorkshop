using Microsoft.EntityFrameworkCore;

namespace bootcamp_webapi
{
    public interface IProductContext
    {
        DbSet<Product> Products { get; set; }
    }
}