using System;
using System.Web.Mvc;
using System.Linq;
using Newtonsoft.Json;
using HeroesAggregator.Scraping.Models;
using HeroesAggregator.Scraping.Scrapers;

namespace HeroesAggregator.Controllers
{
    public class HeroesController : Controller
    {
        // GET: Heroes
        public ActionResult TeamMmr(string id)
        {
            var isJson = Request.AcceptTypes.Any(e => e == "application/json");

            if (string.IsNullOrWhiteSpace(id))
                return View((string)null);

            var team = ScrapeTeam(id);

            return isJson ? (ActionResult) Json(JsonConvert.SerializeObject(team), JsonRequestBehavior.AllowGet) : View(team);
        }

        private TeamModel ScrapeTeam(string id)
        {
            return HeroesLoungeScraper.ScrapeTeam(id);
        }
    }
}