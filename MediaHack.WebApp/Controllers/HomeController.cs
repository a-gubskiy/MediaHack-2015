using MediaHack.Core;
using System.Web.Mvc;

namespace MediaHack.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private Cloud _cloud;

        public HomeController()
        {
            _cloud = new Cloud(@"d:\config.json");
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