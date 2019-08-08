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