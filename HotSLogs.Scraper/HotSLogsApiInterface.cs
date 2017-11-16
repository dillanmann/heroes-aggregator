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
    internal enum MmrWeightingType
    {
        HeroLeague = 0,
        TeamLeague,
        UnrankedDraft
    }

    internal class HotSLogsApiInterface
    {
        private static string _rootUrl = "https://api.hotslogs.com/Public/";
        private readonly static string _urlArgs = "Players/{0}/{1}";
        private readonly string _rootUrlFormat = _rootUrl + _urlArgs ;
        private readonly static RestClient _restClient = new RestClient(_rootUrl);

        private string FormatUrl(string url, string battleTag, int region = 2)
        {
            if (battleTag.Contains("#"))
                battleTag = battleTag.Replace('#', '_');


            return string.Format(url, region, battleTag);
        }

        private Dictionary<MmrWeightingType, int> GetPlayerStats(string url, out string playerId)
        {
            var request = new RestRequest(url, Method.GET);
            var response = _restClient.Execute(request);
            var responseText = response.Content;

            if(responseText == null || responseText == "null")
            {
                playerId = string.Empty;
                return null;
            }

            var responseJson = JObject.Parse(responseText);
            var id = responseJson["PlayerID"];
            playerId = id.ToString();
            var leaderBoards = responseJson["LeaderboardRankings"];

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

        public Dictionary<MmrWeightingType, int> GetPlayerStats(string battleTag, out string playerId, int region = 2)
        {
            var url = FormatUrl(_urlArgs, battleTag, region);
            var stats = GetPlayerStats(url, out playerId);

            if (stats == null)
                Console.WriteLine($"Got failed response from HotSLogs for player {battleTag}");

            return stats;
        }
    }
}
