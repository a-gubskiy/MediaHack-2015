using System.Configuration;
using MediaHack.Core;
using System.Web.Mvc;

namespace MediaHack.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Cloud _cloud;

        public HomeController()
        {
            var config = new CloudConfig
            {
                StorageServiceAccountName = ConfigurationSettings.AppSettings["StorageServiceAccountName"],
                StorageServiceAccessKey = ConfigurationSettings.AppSettings["StorageServiceAccessKey"],
                CdnEndpointName = ConfigurationSettings.AppSettings["CdnEndpointName"]
            };

            if (config.IsEmpty)
            {
                _cloud = new Cloud(@"d:\config.json");
            }
            else
            {
                _cloud = new Cloud(config);
            }
        }

        public ActionResult Index()
        {
            var videos = _cloud.GetVideos();


            return View(videos);
        }

        [Route("video/{name}")]
        public ActionResult Video(string name)
        {
            var video = _cloud.GetVideos(name);

            return View(video);
        }
    }
}