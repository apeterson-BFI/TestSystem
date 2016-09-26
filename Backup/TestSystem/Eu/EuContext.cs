using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Eu
{
    public class EuContext
    {
        public List<Hand> hands;
        public Side active;
        public PlayField field;
        public int nsTricks;
        public int ewTricks;
        public Card flipCard;

        public EuContext(Side dealer, List<Card> cards21)
        {
            if (cards21.Count != 21)
            {
                throw new ArgumentException("Invalid deal");
            }

            active = dealer;
            field = new PlayField();

            nsTricks = 0;
            ewTricks = 0;

            Hand temp;
            Side ts = dealer;

            for(int i = 0; i < 4; i++)
            {
                temp = new Hand(cards21.GetRange(i * 5, 5), ts);
                hands.Add(temp);

                ts = ClockwiseNextSide(ts);
            }

            flipCard = cards21[20];
        }

        public EuContext(Side active, List<Hand> hands, int ns, int ew, PlayField pf)
        {
            nsTricks = ns;
            ewTricks = ew;

            this.active = active;
            this.hands = hands;
            field = pf;
        }

        public static Side ClockwiseNextSide(Side s)
        {
            if (s == Side.North)
                return Side.East;
            else if (s == Side.East)
                return Side.South;
            else if (s == Side.South)
                return Side.West;
            else if (s == Side.West)
                return Side.North;
            else
                throw new ArgumentException("Enum changed without updating ClockwiseNextSide");
        }


    }
}
