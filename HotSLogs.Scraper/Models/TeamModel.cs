﻿using System.Collections.Generic;
using System.Linq;

namespace HeroesAggregator.Scraping.Models
{
    public class TeamModel
    {
        public List<PlayerModel> Players { get; set; }

        public string Name { get; set; }

        public double AverageMmr {
            get
            {
                return Players.Select(e => e.WeightedMmr).Sum() / Players.Count;
            }
        }
    }
}