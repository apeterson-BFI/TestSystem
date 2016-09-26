using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public class PStrategy : Strategy
    {
        private double goProb;

        public PStrategy(double goProb)
        {
            this.goProb = goProb;
        }

        public bool decide(int turn, Random rnm)
        {
            return (rnm.NextDouble() < goProb);
        }

        public Strategy mutate(Random rnm)
        {
            int r = rnm.Next(2);
            double m = goProb;

            if (r == 0)
            {
                m += 0.001;
            }
            else
            {
                m -= 0.001;
            }

            PStrategy p = new PStrategy(m);
            return p;
        }

        public override string ToString()
        {
            return string.Format("Go with P({0:0.000})", goProb);
        }
    }
}
