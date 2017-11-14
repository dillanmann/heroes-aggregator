using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesAggregator.Scraping.Models
{
    /// <summary>
    ///     Model for the hero preferences of a player from HotSLogs
    /// </summary>
    public class PlayerHeroesPreferenceModel
    {
        /// <summary>
        ///     Stats of the heroes the player has played
        /// </summary>
        public IReadOnlyCollection<HeroStatsModel> HeroStats { get; set; } = new List<HeroStatsModel>();

        /// <summary>
        ///     Id of the player (HotSLogs ID)
        /// </summary>
        public string PlayerId { get; set; }
    }
}
