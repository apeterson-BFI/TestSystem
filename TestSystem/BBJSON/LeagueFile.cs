using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class LeagueFile
    {
        public MetaData meta { get; set; }
        public List<PlayerData> players { get; set; }
        public List<ReleasedData> releasedPlayers { get; set; }
        public List<AwardDetailData> awards { get; set; }

    }
}
