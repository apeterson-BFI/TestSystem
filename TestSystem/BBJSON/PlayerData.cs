using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class PlayerData
    {
        public int tid { get; set; }
        public List<int> statsTids { get; set; }
        public int rosterOrder { get; set; }
        public List<Rating> ratings { get; set; }
        public int weight { get; set; }
        public int hgt { get; set; }
        public BornData born { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string college { get; set; }
        public string imgURL { get; set; }
        public List<AwardData> awards { get; set; }
        public List<double> freeAgentMood { get; set; }
        public int yearsFreeAgent { get; set; }
        public int? retiredYear { get; set; }
        public DraftData draft { get; set; }
        public FaceData face { get; set; }
        public InjuryData injury { get; set; }
        public double ptModifier { get; set; }
        public bool hof { get; set; }
        public bool watch { get; set; }
        public int gamesUntilTradable { get; set; }
        public double value { get; set; }
        public double valueNoPot { get; set; }
        public double valueFuzz { get; set; }
        public double valueNoPotFuzz { get; set; }
        public double valueWithContract { get; set; }
        public List<SalaryData> salaries { get; set; }
        public ContractData contract { get; set; }
        public int pid { get; set; }
        public List<StatsData> stats { get; set; }
    }
}
