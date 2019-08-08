using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace CloudFoundry
{
    public class CosmosDbInfo
    {
        private readonly CloudFoundryServicesOptions _services;
        private const string ServiceNameString = "azure-cosmosdb";

        public CosmosDbInfo(IOptions<CloudFoundryServicesOptions> servicesOptions)
        {
            _services = servicesOptions.Value;
        }

        private string _endpoint;
        public string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(_endpoint))
                {
                    var cosmoDbService = _services.Services[ServiceNameString][0];
                    _endpoint = cosmoDbService.Credentials["cosmosdb_host_endpoint"].Value;
                }

                return _endpoint;
            }
        }

        private string _key;
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    var cosmoDbService = _services.Services[ServiceNameString][0];
                    _key = cosmoDbService.Credentials["cosmosdb_master_key"].Value;
                }

                return _key;
            }
        }

        private string _databaseId;
        public string DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(_databaseId))
                {
                    var cosmoDbService = _services.Services[ServiceNameString][0];
                    _databaseId = cosmoDbService.Credentials["cosmosdb_database_id"].Value;
                }

                return _databaseId;
            }
        }
    }
}
