using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace HeroesAggregator.Scraping
{
    /// <summary>
    ///     HotSLogs MMR weightings
    /// </summary>
    public enum MmrWeightingType
    {
        HeroLeague = 0,
        TeamLeague,
        UnrankedDraft
    }

    /// <summary>
    ///     Interface for interacting with the HotSLogs REST API.
    /// </summary>
    public class HotSLogsApiInterface
    {
        private const string _rootUrl = "https://api.hotslogs.com/Public/";
        private const string _playerDetailsByBattleTagUrl = "Players/{0}/{1}";
        private const string _playerDetailsByIdUrl = "Players/{0}";
        private readonly RestClient _restClient = new RestClient(_rootUrl);

        private string FormatUrl(string url, string battleTag, int region = 2)
        {
            if (battleTag.Contains("#"))
                battleTag = battleTag.Replace('#', '_');

            return string.Format(url, region, battleTag);
        }

        private JObject GetPlayerJson(string url)
        {
            var request = new RestRequest(url, Method.GET);
            var response = _restClient.Execute(request);
            var responseText = response.Content;

            if (responseText == null || responseText == "null")
                return null;

            return JObject.Parse(responseText);
        }

        private Dictionary<MmrWeightingType, int> GetPlayerStats(string url, out string playerId)
        {
            var responseJson = GetPlayerJson(url);
            var id = responseJson["PlayerID"];
            playerId = id.ToString();

            var stats = ParsePlayerStats(responseJson);

            return stats;
        }

        private Dictionary<MmrWeightingType, int> ParsePlayerStats(JObject json)
        {
            var leaderBoards = json["LeaderboardRankings"];

            var stats = new Dictionary<MmrWeightingType, int>();

            foreach (var ranking in leaderBoards)
            {
                var jsonType = ranking["GameMode"];
                if (!Enum.TryParse(jsonType.ToString(), out MmrWeightingType type))
                    continue;

                var mmr = ranking["LeagueRank"].ToString() == "null" ? -1 : Convert.ToInt32(ranking["CurrentMMR"]);
                stats[type] = mmr;
            }

            return stats;
        }

        /// <summary>
        ///     Get the HotSLogs ID of a player from their battle tag and region.
        /// </summary>
        /// <param name="battleTag"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public string PlayerIdFromBattleTag(string battleTag, int region = 2)
        {
            if (battleTag.Contains('#'))
                battleTag = battleTag.Replace('#', '_');

            var url = FormatUrl(_playerDetailsByBattleTagUrl, battleTag, region);
            var responseJson = GetPlayerJson(url);

            var id = responseJson["PlayerID"];
            return id.ToString();
        }

        /// <summary>
        ///     Get the player stats from a HotSLogs player id
        /// </summary>
        /// <param name="playerId">HotSLogs player id</param>
        /// <returns></returns>
        public Dictionary<MmrWeightingType, int> GetPlayerStats(string playerId)
        {
            var url = string.Format(_playerDetailsByIdUrl, playerId);
            return ParsePlayerStats(GetPlayerJson(url));
        }

        /// <summary>
        ///     Get the player stats from a battletag and region.
        /// </summary>
        /// <param name="battleTag"></param>
        /// <param name="playerId"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public Dictionary<MmrWeightingType, int> GetPlayerStats(string battleTag, out string playerId, int region = 2)
        {
            var url = FormatUrl(_playerDetailsByBattleTagUrl, battleTag, region);
            var stats = GetPlayerStats(url, out playerId);

            if (stats == null)
                Console.WriteLine($"Got failed response from HotSLogs for player {battleTag}");

            return stats;
        }
    }
}
