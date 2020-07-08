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
            var jsonString = await client.GetStringAsync("https://bootcamp-api-mk/api/products");
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