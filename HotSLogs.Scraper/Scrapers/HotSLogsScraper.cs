using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HeroesAggregator.Scraping.Models;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HeroesAggregator.Scraping.Scrapers
{
    public static class HotSLogsScraper
    {
        private static readonly string _playerDetailsUrlFormat = "https://www.hotslogs.com/Player/HeroOverview?PlayerID={0}";

        private static string BuildPlayerStatsUrl(string playerId) => string.Format(_playerDetailsUrlFormat, playerId);

        public static PlayerHeroesPreferenceModel ScrapeHeroPreferences(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return null;

            var url = BuildPlayerStatsUrl(playerId);

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

            return new PlayerHeroesPreferenceModel { HeroStats = stats.OrderByDescending(e => e.GamesPlayed).ThenByDescending(e => e.WinPercent).ToList() , PlayerId = playerId };
        }

        public static PlayerHeroesPreferenceModel ScrapeHeroPreferencesPhantomJs(string playerId, int days = 60)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return null;

            var driver = new PhantomJSDriver();
            var url = BuildPlayerStatsUrl(playerId);

            driver.Navigate().GoToUrl(url);

            // Open the 'Days to show' selector
            driver.FindElementById("ctl00_MainContent_DropDownProfileTimeSpan").Click();            
            var daySelectParent = new WebDriverWait(driver, TimeSpan.FromSeconds(2)).Until(e => {
                var elem = e.FindElement(By.Id("ctl00_MainContent_DropDownProfileTimeSpan_DropDown"));
                return elem.GetAttribute("style").Contains("display: block;") ? elem : null;
            });

            // Select the 'Last X Days' option to show only heroes played in the last so many days
            var daySelectText = $"Last {days} Days";
            daySelectParent.FindElements(By.CssSelector("li")).First(e => e.Text == daySelectText).Click();

            // Do the same for the range (i.e. only show heroes that have been played more than x times)
            driver.FindElementById("ctl00_MainContent_DropDownGamesPlayedRequired").Click();
            var rangeSelectParent = new WebDriverWait(driver, TimeSpan.FromSeconds(2)).Until(e => {
                var elem = e.FindElement(By.Id("ctl00_MainContent_DropDownGamesPlayedRequired_DropDown"));
                return elem.GetAttribute("style").Contains("display: block;") ? elem : null;
            });

            var rangeSelectText = "5+ Games Played";
            new WebDriverWait(driver, TimeSpan.FromSeconds(2)).Until(e =>
            {
                var elem = rangeSelectParent.FindElements(By.CssSelector("li")).FirstOrDefault(li => li.Text == rangeSelectText);
                return elem ?? elem;
            }).Click();

            // Find the hero stats in the table and parse the stats out
            var rows = driver.FindElementsByCssSelector("#RadGridReplayCharacterScoreResultsAverage table tbody tr");
            var stats = new List<HeroStatsModel>();
            foreach (var row in rows)
            {
                var dataElems = row.FindElements(By.TagName("td"));
                var heroNameElem = dataElems[0];
                var gamesPlayedElem = dataElems[2];
                var winPercentElem = dataElems[3];

                var heroName = heroNameElem.FindElement(By.TagName("a")).GetAttribute("title");
                var gamesPlayed = gamesPlayedElem.Text;
                var winPercent = winPercentElem.Text.Replace("%", "");

                stats.Add(new HeroStatsModel { Name = WebUtility.HtmlDecode(heroName), GamesPlayed = Convert.ToInt32(gamesPlayed), WinPercent = Convert.ToDouble(winPercent) });
            }

            return new PlayerHeroesPreferenceModel { HeroStats = stats.OrderByDescending(e => e.GamesPlayed).ThenByDescending(e => e.WinPercent).ToList(), PlayerId = playerId } ;
        }

    }
}
