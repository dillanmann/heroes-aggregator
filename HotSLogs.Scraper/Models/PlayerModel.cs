using System;
using System.Collections.Generic;

namespace HotSLogs.Scraper.Models
{
    public class PlayerModel
    {
        public int TeamLeagueMmr { get; set; }
        public int HeroLeagueMmr { get; set; }
        public int UnrankedDraftMmr { get; set; }
        public double WeightedMmr { get
            {
                return (HeroLeagueMmr == -1 ? 0 : 0.5 * HeroLeagueMmr)
                    + (TeamLeagueMmr == -1 ? 0 : 0.3 * TeamLeagueMmr)
                    + (UnrankedDraftMmr == -1 ? 0 : 0.2 * UnrankedDraftMmr);
            } }

        public string Role { get; set; }
        public string BattleTag { get; set; }
        public string PlayerName { get; set; }
        
        public PlayerModel(Dictionary<string, object> details)
        {
            PlayerName = details["PlayerName"].ToString();
            BattleTag = details["BattleTag"].ToString();
            Role = details.ContainsKey("Role") ? details["Role"].ToString() : null;

            var stats = (Dictionary<MmrWeightingType, int>)details["Stats"];

            TeamLeagueMmr = stats.ContainsKey(MmrWeightingType.TeamLeague) ? stats[MmrWeightingType.TeamLeague] : -1;
            HeroLeagueMmr = stats.ContainsKey(MmrWeightingType.HeroLeague) ? stats[MmrWeightingType.HeroLeague] : -1;
            UnrankedDraftMmr = stats.ContainsKey(MmrWeightingType.UnrankedDraft) ? stats[MmrWeightingType.UnrankedDraft] : -1;
        }
    }
}