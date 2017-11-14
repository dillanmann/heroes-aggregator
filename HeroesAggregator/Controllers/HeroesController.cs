using System;
using System.Web.Mvc;
using HeroesAggregator.Scraping.Models;
using HeroesAggregator.Scraping.Scrapers;

namespace HeroesAggregator.Controllers
{
    public class HeroesController : Controller
    {
        // GET: Heroes
        public ActionResult TeamMmr(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return View((string)null);

            var cacheKey = $"team_id_{id}";

            TeamModel team;
/*            try
            {*/
                if (PrimitiveCache.HasKey(cacheKey))
                    return View(PrimitiveCache.FetchItem<TeamModel>(cacheKey));

                var scraper = new HeroesLoungeScraper();
                team = scraper.ScrapeTeam(id);
/*            }
            catch (Exception ex)
            {
                team = new TeamModel { Name = null };
            }*/

            PrimitiveCache.AddOrUpdateItem(cacheKey, team);

            return View(team);
        }
    }
}