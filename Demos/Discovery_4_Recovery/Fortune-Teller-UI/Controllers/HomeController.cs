using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fortune_Teller_UI.Services;

namespace Fortune_Teller_UI.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        IFortuneService _fortunes;

        public HomeController(IFortuneService fortunes)
        {
            _fortunes = fortunes;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("random")]
        public async Task<string> Random()
        {
            return await _fortunes.RandomFortuneAsync();
        }

        [HttpGet("destroy")]
        public void Destroy()
        {
            Console.WriteLine("Destorying current application instance to show HA and the platform restart it!");
            Environment.Exit(-1);
        }
    }
}
