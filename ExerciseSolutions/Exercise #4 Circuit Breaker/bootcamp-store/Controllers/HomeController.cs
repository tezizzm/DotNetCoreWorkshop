using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bootcamp_store.Models;
using bootcamp_store.Service;
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