using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class AwardDetailData
    {
        public int season { get; set; }
        public BestRecordData bestRecord { get; set; }
        public List<BestRecordData> bestRecordConfs { get; set; }
        public PlayerSeasonData roy { get; set; }
        public List<PlayerSeasonData> allRookie { get; set; }
        public PlayerSeasonData mvp { get; set; }
        public PlayerSeasonData smoy { get; set; }
        public List<AllLeagueData> allLeague { get; set; }
        public PlayerDSeasonData dpoy { get; set; }
        public List<AllDefensiveData> allDefensive { get; set; }
        public PlayerSeasonData finalsMvp { get; set; }

    }
}
