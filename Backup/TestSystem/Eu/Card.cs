using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Eu
{
    public class Card
    {
        public Rank r;
        public Suit s;

        public Card(int rankVal, int suitVal)
        {
            r = (Rank)rankVal;
            s = (Suit)suitVal;
        }

        public Card(Rank r, Suit s)
        {
            this.r = r;
            this.s = s;
        }

        public CardColor GetColor()
        {
            if (s == Suit.Clubs || s == Suit.Spades)
            {
                return CardColor.Black;
            }
            else
            {
                return CardColor.Red;
            }
        }

        public Suit GetEffectiveSuit(Suit trump)
        {
            Card test = new Card(1, (int)trump);
            CardColor c = test.GetColor();

            // Color other Jack
            if (c == GetColor() && trump != s && r == Rank.Jack)
            {
                return trump;
            }
            else
            {
                return s;
            }
        }

        public static bool operator ==(Card a, Card b)
        {
            return (a.s == b.s && a.r == b.r);
        }

        public static bool operator !=(Card a, Card b)
        {
            return (a.s != b.s || a.r != b.r);
        }

        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                Card t = (Card)obj;

                return (t.r == r && t.s == s);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (((int)s << 5) ^ (int)r);
        }
    }

    public enum Rank
    {
        Ace = 1,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13        
    }

    public enum CardColor
    {
        Black,
        Red
    }

    public enum Suit
    {
        Spades,
        Clubs,
        Diamonds,
        Hearts
    }
}
