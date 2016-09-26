using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public class TStrategy : Strategy
    {
        private int actTurn;

        public TStrategy(int actTurn)
        {
            this.actTurn = actTurn;
        }

        public bool decide(int turn, Random rnm)
        {
            return turn >= actTurn;
        }

        public Strategy mutate(Random rnm)
        {
            int m = rnm.Next(2);

            if (m == 0)
            {
                TStrategy ts = new TStrategy(actTurn + 1);
                return ts;
            }
            else
            {
                TStrategy ts = new TStrategy(actTurn - 1);
                return ts;
            }
        }

        public override string ToString()
        {
            return string.Format("Go on/after turn {0:0}", actTurn);
        }
    }
}
