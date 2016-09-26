using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public class BinTransferStrategy : Strategy
    {
        int transferTurn;
        Strategy first;
        Strategy second;

        public BinTransferStrategy(int transferTurn, Strategy first, Strategy second)
        {
            this.transferTurn = transferTurn;
            this.first = first;
            this.second = second;
        }

        public bool decide(int turn, Random rnm)
        {
            if (turn >= transferTurn)
            {
                return second.decide(turn, rnm);
            }
            else
            {
                return first.decide(turn, rnm);
            }
        }

        public Strategy mutate(Random rnm)
        {
            int m = rnm.Next(4);

            if (m == 0)
            {
                BinTransferStrategy b = new BinTransferStrategy(transferTurn + 1, first, second);
                return b;
            }
            else if (m == 1)
            {
                BinTransferStrategy b = new BinTransferStrategy(transferTurn - 1, first, second);
                return b;
            }
            else if (m == 2)
            {
                BinTransferStrategy b = new BinTransferStrategy(transferTurn, first.mutate(rnm), second);
                return b;
            }
            else
            {
                BinTransferStrategy b = new BinTransferStrategy(transferTurn, first, second.mutate(rnm));
                return b;
            }
        }

        public override string ToString()
        {
            return string.Format("({0} until turn {1} then {2})", first, transferTurn, second);
        }
    }
}
