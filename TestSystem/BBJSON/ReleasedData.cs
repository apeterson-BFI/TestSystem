using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.BBJSON
{
    public class ReleasedData
    {
        public int pid { get; set; }
        public int tid { get; set; }
        public ContractData contract { get; set; }
        public int rid { get; set; }
    }
}
