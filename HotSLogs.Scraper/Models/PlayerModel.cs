using System.Collections.Generic;

namespace HeroesAggregator.Scraping.Models
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
        public string HotSLogsId { get; set; }

        public PlayerHeroesPreferenceModel Stats { get; private set; }
        
        public PlayerModel(Dictionary<string, object> details, PlayerHeroesPreferenceModel stats = null)
        {
            HotSLogsId = details["PlayerId"].ToString();
            PlayerName = details["PlayerName"].ToString();
            BattleTag = details["BattleTag"].ToString();
            Role = details.ContainsKey("Role") ? details["Role"].ToString() : null;

            Stats = stats;

            var mmrStats = (Dictionary<MmrWeightingType, int>)details["Stats"];

            if (mmrStats == null)
            {
                TeamLeagueMmr = HeroLeagueMmr = UnrankedDraftMmr = -1;
                return;
            }

            TeamLeagueMmr = mmrStats.ContainsKey(MmrWeightingType.TeamLeague) ? mmrStats[MmrWeightingType.TeamLeague] : -1;
            HeroLeagueMmr = mmrStats.ContainsKey(MmrWeightingType.HeroLeague) ? mmrStats[MmrWeightingType.HeroLeague] : -1;
            UnrankedDraftMmr = mmrStats.ContainsKey(MmrWeightingType.UnrankedDraft) ? mmrStats[MmrWeightingType.UnrankedDraft] : -1;
        }
    }
}