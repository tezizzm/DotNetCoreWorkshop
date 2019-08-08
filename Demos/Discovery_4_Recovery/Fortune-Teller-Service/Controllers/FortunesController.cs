
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using FortuneTellerService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace FortuneTellerService.Controllers
{
    [Route("api/[controller]")]
    public class FortunesController : Controller
    {
        private IFortuneRepository _fortunes;
        private ILogger<FortunesController> _logger;
        private CloudFoundryApplicationOptions _applicationOptions;

        public FortunesController(IFortuneRepository fortunes, ILogger<FortunesController> logger,
            IOptions<CloudFoundryApplicationOptions> applicationOptions)
        {
            _fortunes = fortunes;
            _logger = logger;
            _applicationOptions = applicationOptions.Value;
        }

        // GET: api/fortunes
        [HttpGet]
        public IEnumerable<Fortune> Get()
        {
            _logger?.LogInformation("GET api/fortunes");
            return _fortunes.GetAll();
        }

        // GET api/fortunes/random
        [HttpGet("random")]
        public Fortune Random()
        {
            _logger?.LogInformation("GET api/fortunes/random");
            return _fortunes.RandomFortune();
        }
    }
}
