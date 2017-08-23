using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotSLogs.Scraper.Models;
using HotSLogs.Scraper.Scrapers;

namespace HeroesAggregator.Controllers
{
    public class HeroesController : Controller
    {
        // GET: Heroes
        public ActionResult TeamMmr(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return View((string)null);

            var scraper = new HeroesLoungeScraper();

            TeamModel team;
            try
            {
                team = scraper.ScrapeTeam(id);
            }
            catch (Exception)
            {
                team = new TeamModel { Name = null };
            }

            return View(team);
        }
    }
}