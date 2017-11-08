using System;
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

            var key = $"team_id_{id}";
            if (PrimitiveCache.HasKey(key))
                return View(PrimitiveCache.FetchItem<TeamModel>(key));

            var scraper = new HeroesLoungeScraper();

            TeamModel team;
            try
            {
                team = scraper.ScrapeTeam(id);
            }
            catch (Exception ex)
            {
                team = new TeamModel { Name = null };
            }

            PrimitiveCache.AddOrUpdateItem(key, team);

            return View(team);
        }
    }
}