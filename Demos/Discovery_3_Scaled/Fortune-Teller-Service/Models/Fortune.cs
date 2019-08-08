using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace FortuneTellerService.Models
{
    public class Fortune
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string InstanceId { get; set; }

        public int InstanceIndex { get; set; }
    }
}
