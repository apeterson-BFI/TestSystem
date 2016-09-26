using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Eu
{
    public class PlayField
    {
        public Card north;
        public Card east;
        public Card south;
        public Card west;


        public void PlayCard(Hand hand, Card c)
        {
            if (hand.side == Side.North)
            {
                north = c;
            }
            else if (hand.side == Side.East)
            {
                east = c;
            }
            else if (hand.side == Side.South)
            {
                south = c;
            }
            else if (hand.side == Side.West)
            {
                west = c;
            }
        }
    }

    public enum Side
    {
        North,
        East,
        South,
        West
    }
}
