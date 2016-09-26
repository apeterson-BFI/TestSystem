using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public interface Strategy
    {
        bool decide(int turn, Random rnm);
        Strategy mutate(Random rnm);
    }
}
