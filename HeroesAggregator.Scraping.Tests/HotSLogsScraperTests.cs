using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeroesAggregator.Scraping.Scrapers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesAggregator.Scraping.Tests
{
    [TestClass]
    public class HotSLogsScraperTests
    {
        [TestMethod]
        public void TestScraping()
        {
            HotSLogsScraper.ScrapeHeroPreferences("459446");
        }

        // todo 
        // Test case for null coming from HotsLogs
    }
}
