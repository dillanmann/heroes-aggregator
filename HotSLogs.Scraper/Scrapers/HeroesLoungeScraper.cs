using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HotSLogs.Scraper.Models;

namespace HotSLogs.Scraper.Scrapers
{
    public class HeroesLoungeScraper
    {
        private static readonly string _rootHeroesLoungeDomain = "https://heroeslounge.gg/";
        private static readonly string _rootTeamDetailsUrl = $"{_rootHeroesLoungeDomain}/team/view/";
        private static readonly string _rootTeamListUrl = $"{_rootHeroesLoungeDomain}/season-{{0}}/division-{{1}}";
        private static readonly string _currentSeason = "5";
        private static readonly List<string> _divisions = new List<string> { "1", "2", "3", "4", "5" };

        public TeamModel ScrapeTeam(string teamId)
        {
            var teamUrl = _rootTeamDetailsUrl + (teamId.StartsWith("/") ? teamId.Skip(1) : teamId);
            
            var webGet = new HtmlWeb();
            var document = webGet.Load(teamUrl).DocumentNode;

            var teamName = WebUtility.HtmlDecode(document.SelectSingleNode("//h1[contains(@class, block-title)]").InnerText);
            var roster = document.SelectSingleNode("//div[@id='general']");
            var playerCards = roster.SelectNodes("div/div[contains(class, card-deck)]/div[contains(class, card)]/div[@class='card-body']");

            if (playerCards == null || playerCards.Count == 0)
                return null;

            var players = new Dictionary<string, string>();
            foreach(var card in playerCards)
            {
                var playerName = card.SelectSingleNode("h4[@class='card-title']/a").InnerText;
                var cardTexts = card.SelectNodes("p[@class='card-text']");
                var battleTag = cardTexts.FirstOrDefault(e => e.SelectNodes("img[@title='Battle Tag']") != null)?.InnerText
                    .Replace("\n", "").Replace(" ", "");

                if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(battleTag))
                {
                    Console.WriteLine($"Failed to find battletag for player {playerName}");
                    continue;
                }

                players[playerName] = battleTag;
            }

            var hotsLogsInterface = new HotSLogsApiInterface();
            var playerModels = new List<PlayerModel>();
            foreach(var player in players)
            {
                var stats = hotsLogsInterface.GetPlayerStats(player.Value);
                var details = new Dictionary<string, object>
                {
                    { "PlayerName" , player.Key },
                    { "BattleTag" , player.Value },
                    { "Stats" , stats }
                };
                playerModels.Add(new PlayerModel(details));
            }

            return new TeamModel
            {
                Name = teamName,
                Players = playerModels
            };
        }

        /// <summary>
        ///     Scrape the names of all the teams from the divisions of the current season.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<TeamUrlModel>> ScrapeTeamUrls()
        {
            var results = new Dictionary<string, List<TeamUrlModel>>();

            var webGet = new HtmlWeb();
            foreach(var division in _divisions)
            {
                var teamListUrl = string.Format(_rootTeamListUrl, _currentSeason, division);
                var document = webGet.Load(teamListUrl).DocumentNode;
                var teamList = document.SelectSingleNode("//table/tbody");
                var teams = teamList.SelectNodes("tr/td/a").Select(e => 
                    new TeamUrlModel {
                        Name = WebUtility.HtmlDecode(e.InnerText),
                        Url = e.Attributes["href"].Value.Split(new[] { "view/" }, StringSplitOptions.None).Last()
                    }).ToList();
                
                results[division] = teams;
            }

            return results;
        }
    }
}
