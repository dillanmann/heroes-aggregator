using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesAggregator.Scraping.Models
{
    /// <summary>
    ///     Model for the stats of a named hero from HotSLogs
    /// </summary>
    public class HeroStatsModel
    {
        /// <summary>
        ///     Name of the hero.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Games played with this hero.
        /// </summary>
        public int GamesPlayed { get; set; }

        /// <summary>
        ///     Win percentage with this hero.
        /// </summary>
        public double WinPercent { get; set; }
    }
}
