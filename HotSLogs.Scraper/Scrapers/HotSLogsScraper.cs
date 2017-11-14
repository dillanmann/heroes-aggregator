using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HeroesAggregator.Scraping.Models;

namespace HeroesAggregator.Scraping.Scrapers
{
    public static class HotSLogsScraper
    {
        private static readonly string _playerDetailsUrlFormat = "https://www.hotslogs.com/Player/HeroOverview?PlayerID={0}";

        public static PlayerHeroesPreferenceModel ScrapeHeroPreferences(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentNullException(playerId);

            var url = string.Format(_playerDetailsUrlFormat, playerId);

            var webGet = new HtmlWeb();
            var document = webGet.Load(url).DocumentNode;

            var rows = document.SelectNodes("//div[@id='RadGridReplayCharacterScoreResultsAverage']/table/tbody/tr");
            var stats = new List<HeroStatsModel>();
            foreach (var row in rows)
            {
                var dataElems = row.SelectNodes("td");
                var heroNameElem = dataElems[0];
                var gamesPlayedElem = dataElems[2];
                var winPercentElem = dataElems[3];

                var heroName = heroNameElem.SelectSingleNode("a").Attributes["title"].Value;
                var gamesPlayed = gamesPlayedElem.InnerText;
                var winPercent = winPercentElem.InnerText.Replace("%", "");

                stats.Add(new HeroStatsModel { Name = WebUtility.HtmlDecode(heroName), GamesPlayed = Convert.ToInt32(gamesPlayed), WinPercent = Convert.ToDouble(winPercent) });
            }

            return new PlayerHeroesPreferenceModel { HeroStats = stats.OrderByDescending(e => e.GamesPlayed).ThenByDescending(e => e.WinPercent).ToList() , PlayerId = playerId } ;
        }

    }
}
