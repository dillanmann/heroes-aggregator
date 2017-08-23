using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HotSLogs.Scraper
{
    internal enum MmrWeightingType
    {
        HeroLeague = 0,
        TeamLeague,
        UnrankedDraft
    }

    internal class HotSLogsApiInterface
    {
        private readonly string _rootUrlFormat = "https://api.hotslogs.com/Public/Players/{0}/{1}";

        private string FormatUrl(string battleTag, int region = 2)
        {
            if (battleTag.Contains("#"))
                battleTag = battleTag.Replace('#', '_');


            return string.Format(_rootUrlFormat, region, battleTag);
        }

        private Dictionary<MmrWeightingType, int> GetPlayerStats(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                string responseText;
                using (var stream = new StreamReader(response.GetResponseStream()))
                    responseText = stream.ReadToEnd();

                var responseJson = JObject.Parse(responseText);
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
        }

        public Dictionary<MmrWeightingType, int> GetPlayerStats(string battleTag, int region = 2)
        {
            var url = FormatUrl(battleTag, region);
            var stats = GetPlayerStats(url);

            if (stats == null)
                Console.WriteLine($"Got failed response from HotSLogs for player {battleTag}");

            return GetPlayerStats(url);
        }
    }
}
