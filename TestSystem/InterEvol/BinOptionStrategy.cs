using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public class BinOptionStrategy : Strategy
    {
        public Strategy left;
        public Strategy right;

        public BinOptionStrategy(Strategy left, Strategy right)
        {
            this.left = left;
            this.right = right;
        }

        public bool decide(int turn, Random rnm)
        {
            int r = rnm.Next(2);

            if (r == 0)
            {
                return left.decide(turn, rnm);
            }
            else
            {
                return right.decide(turn, rnm);
            }
        }

        public Strategy mutate(Random rnm)
        {
            int m = rnm.Next(2);

            if (m == 0)
            {
                BinOptionStrategy b = new BinOptionStrategy(left.mutate(rnm), right);
                return b;
            }
            else
            {
                BinOptionStrategy b = new BinOptionStrategy(left, right.mutate(rnm));
                return b;
            }
        }

        public override string ToString()
        {
            return string.Format("(50% of either: {0} or {1})", left, right);
        }
    }
}
