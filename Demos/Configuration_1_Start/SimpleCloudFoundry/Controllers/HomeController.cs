
using Microsoft.AspNetCore.Mvc;
using SimpleCloudFoundry.Model;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using SimpleCloudFoundry.ViewModels.Home;
using Pivotal.Extensions.Configuration.ConfigServer;
using Microsoft.Extensions.Configuration;

namespace SimpleCloudFoundry.Controllers
{
    public class HomeController : Controller
    {

        private IOptionsSnapshot<ConfigServerData> IConfigServerData { get; set; }
        private CloudFoundryServicesOptions CloudFoundryServices { get; set; }
        private CloudFoundryApplicationOptions CloudFoundryApplication { get; set; }
        private ConfigServerClientSettingsOptions ConfigServerClientSettingsOptions { get; set; }
        private IConfigurationRoot Config { get; set; }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult ConfigServer()
        {
            CreateConfigServerDataViewData();
            return View();
        }
        public IActionResult ConfigServerSettings()
        {
            if (ConfigServerClientSettingsOptions != null)
            {
                ViewData["AccessTokenUri"] = ConfigServerClientSettingsOptions.AccessTokenUri;
                ViewData["ClientId"] = ConfigServerClientSettingsOptions.ClientId;
                ViewData["ClientSecret"] = ConfigServerClientSettingsOptions.ClientSecret;
                ViewData["Enabled"] = ConfigServerClientSettingsOptions.Enabled;
                ViewData["Environment"] = ConfigServerClientSettingsOptions.Environment;
                ViewData["FailFast"] = ConfigServerClientSettingsOptions.FailFast;
                ViewData["Label"] = ConfigServerClientSettingsOptions.Label;
                ViewData["Name"] = ConfigServerClientSettingsOptions.Name;
                ViewData["Password"] = ConfigServerClientSettingsOptions.Password;
                ViewData["Uri"] = ConfigServerClientSettingsOptions.Uri;
                ViewData["Username"] = ConfigServerClientSettingsOptions.Username;
                ViewData["ValidateCertificates"] = ConfigServerClientSettingsOptions.ValidateCertificates;
            }
            else
            {
                ViewData["AccessTokenUri"] = "Not Available";
                ViewData["ClientId"] = "Not Available";
                ViewData["ClientSecret"] = "Not Available";
                ViewData["Enabled"] = "Not Available";
                ViewData["Environment"] = "Not Available";
                ViewData["FailFast"] = "Not Available";
                ViewData["Label"] = "Not Available";
                ViewData["Name"] = "Not Available";
                ViewData["Password"] = "Not Available";
                ViewData["Uri"] = "Not Available";
                ViewData["Username"] = "Not Available";
                ViewData["ValidateCertificates"] = "Not Available";
            }
            return View();
        }
        public IActionResult CloudFoundry()
        {
            return View(new CloudFoundryViewModel(
                CloudFoundryApplication == null ? new CloudFoundryApplicationOptions() : CloudFoundryApplication,
                CloudFoundryServices == null ? new CloudFoundryServicesOptions() : CloudFoundryServices));
        }

        public IActionResult Reload()
        {
            if (Config != null)
            {
                Config.Reload();
            }

            return View();
        }

        private void CreateConfigServerDataViewData()
        {

            // IConfigServerData property is set to a IOptionsSnapshot<ConfigServerData> that has been
            // initialized with the configuration data returned from the Spring Cloud Config Server
            if (IConfigServerData != null && IConfigServerData.Value != null)
            {
                var data = IConfigServerData.Value;
                ViewData["Bar"] = data.Bar ?? "Not returned";
                ViewData["Foo"] = data.Foo ?? "Not returned";

                ViewData["Info.Url"] = "Not returned";
                ViewData["Info.Description"] = "Not returned";

                if (data.Info != null)
                {
                    ViewData["Info.Url"] = data.Info.Url ?? "Not returned";
                    ViewData["Info.Description"] = data.Info.Description ?? "Not returned";
                }
                ViewData["Vault"] = data.Vault ?? "Not returned";
            }
            else {
                ViewData["Bar"] = "Not Available";
                ViewData["Foo"] = "Not Available";
                ViewData["Info.Url"] = "Not Available";
                ViewData["Info.Description"] = "Not Available";
                ViewData["Vault"] = "Not Available";
            }

        }

    }
}
