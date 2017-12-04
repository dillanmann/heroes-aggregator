using System;
using System.Web.Mvc;
using System.Linq;
using Newtonsoft.Json;
using HeroesAggregator.Scraping;
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

        private TeamPlayersModel ScrapeTeam(string id)
        {
            var scraper = new HeroesLoungeScraper();
            return scraper.ScrapeTeamPlayers(id);
        }

        /// <summary>
        ///     Get the hero stats of a player by HotSLogs player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public JsonResult PlayerHeroStats(string playerId)
        {
            return Json(JsonConvert.SerializeObject(HotSLogsScraper.ScrapeHeroPreferences(playerId)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Get mmr of a player from their HotSLogs player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public JsonResult PlayerMmr(string playerId)
        {
            var hotslogsInterface = new HotSLogsApiInterface();
            return Json(JsonConvert.SerializeObject(hotslogsInterface.GetPlayerStats(playerId)), JsonRequestBehavior.AllowGet);
        }

 /*       /// <summary>
        ///     Get the hotslogs id of a player from a battle tag
        /// </summary>
        /// <param name="battleTag"></param>
        /// <returns></returns>
        public JsonResult PlayerIdFromBattletag(string battleTag, int region = 2)
        {
            return Json(JsonConvert)
        }*/
    }
}