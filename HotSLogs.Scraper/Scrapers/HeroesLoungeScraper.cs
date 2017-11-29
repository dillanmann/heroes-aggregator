using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HeroesAggregator.Scraping.Models;

namespace HeroesAggregator.Scraping.Scrapers
{
    public class HeroesLoungeScraper
    {
        private const string _rootHeroesLoungeDomain = "https://heroeslounge.gg/";
        private readonly string _rootTeamDetailsUrl = $"{_rootHeroesLoungeDomain}/team/view/";
        private readonly string _rootTeamListUrl = $"{_rootHeroesLoungeDomain}/season-{{0}}/division-{{1}}";
        private const string _currentSeason = "5";
        private readonly List<string> _divisions = new List<string> { "1", "2", "3", "4", "5" };

        private Dictionary<string, string> GetPlayersOnTeam(string teamId, out string teamName)
        {
            var teamUrl = _rootTeamDetailsUrl + (teamId.StartsWith("/") ? teamId.Skip(1) : teamId);

            var webGet = new HtmlWeb();
            var document = webGet.Load(teamUrl).DocumentNode;

            teamName = WebUtility.HtmlDecode(document.SelectSingleNode("//h1[contains(@class, block-title)]").InnerText);
            var roster = document.SelectSingleNode("//div[@id='general']");
            var playerCards = roster.SelectNodes("div/div[contains(class, card-deck)]/div[contains(class, card)]/div[@class='card-body']");

            if (playerCards == null || playerCards.Count == 0)
                return null;

            var players = new Dictionary<string, string>();
            foreach (var card in playerCards)
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

            return players;
        }

        public TeamModel ScrapeTeam(string teamId)
        {
            var players = GetPlayersOnTeam(teamId, out string teamName);

            var hotsLogsInterface = new HotSLogsApiInterface();
            var playerModels = new List<PlayerModel>();
            foreach(var player in players)
            {
                var playerId = string.Empty;
                var stats = hotsLogsInterface.GetPlayerStats(player.Value, out playerId);
                var details = new Dictionary<string, object>
                {
                    { "PlayerId", playerId },
                    { "PlayerName" , player.Key },
                    { "BattleTag" , player.Value },
                    { "Stats" , stats }
                };

                var playerStats = HotSLogsScraper.ScrapeHeroPreferences(playerId);

                playerModels.Add(new PlayerModel(details, playerStats));
            }

            return new TeamModel
            {
                Name = teamName,
                Players = playerModels
            };
        }

        /// <summary>
        ///     Scrape the players from a team based on the heroes lounge team id.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public TeamPlayersModel ScrapeTeamPlayers(string teamId)
        {
            var players = GetPlayersOnTeam(teamId, out string teamName);
            var hotsInterface = new HotSLogsApiInterface();
            return new TeamPlayersModel
            {
                Name = teamName,
                Players = players.Select(e => new PlayerDetailsModel { BattleTag = e.Value, Name = e.Key, Id = hotsInterface.PlayerIdFromBattleTag(e.Value) }).ToList()
            };
        }

        /// <summary>
        ///     Scrape the names of all the teams from the divisions of the current season.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<TeamUrlModel>> ScrapeTeamUrls()
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
