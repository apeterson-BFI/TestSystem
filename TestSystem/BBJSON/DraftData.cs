using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class DraftData
    {
        public int round { get; set; }
        public int pick { get; set; }
        public int tid { get; set; }
        public int year { get; set; }
        public int originalTid { get; set; }
        public double pot { get; set; }
        public int ovr { get; set; }
        public List<string> skills { get; set; }
    }
}
