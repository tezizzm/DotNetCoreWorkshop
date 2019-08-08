using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace FortuneTellerService.Models
{
    public class FortuneRepository : IFortuneRepository
    {
        private FortuneContext _db;
        Random _random = new Random();
        private readonly CloudFoundryApplicationOptions _applicationOptions;

        public FortuneRepository(FortuneContext db, IOptions<CloudFoundryApplicationOptions> applicationOptions)
        {
            _db = db;
            _applicationOptions = applicationOptions.Value;
        }
        public IEnumerable<Fortune> GetAll()
        {
            return _db.Fortunes.AsEnumerable();
        }

        public Fortune RandomFortune()
        {
            int count = _db.Fortunes.Count();
            var index = _random.Next() % count;
            return DecorateFortune(GetAll().ElementAt(index));
        }

        private Fortune DecorateFortune(Fortune fortune)
        {
            if (_applicationOptions != null)
            {
                fortune.InstanceId = _applicationOptions.InstanceId;
                fortune.InstanceIndex = _applicationOptions.InstanceIndex;
            }

            return fortune;
        }
    }
}
