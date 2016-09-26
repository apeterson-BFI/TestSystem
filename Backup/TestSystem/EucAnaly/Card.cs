using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.EucAnaly
{
    public class Card
    {
        public Rank rank;
        public Suit suit;

        public Card(Rank r, Suit s)
        {
            rank = r;
            suit = s;
        }

        // Format: 9S, JH, AD, 10C etc.
        public Card(string code)
        {
            if (code.Length < 2 || code.Length > 3)
            {
                throw new ArgumentException("Invalid card code.");
            }

            string rchar = code.Substring(0, code.Length - 1); // all but last char
            string schar = code.Substring(code.Length - 1, 1); // last char

            switch (rchar)
            {
                case "9":
                    rank = Rank.Nine;
                    break;
                case "10":
                    rank = Rank.Ten;
                    break;
                case "J":
                    rank = Rank.Jack;
                    break;
                case "Q":
                    rank = Rank.Queen;
                    break;
                case "K":
                    rank = Rank.King;
                    break;
                case "A":
                    rank = Rank.Ace;
                    break;
                default:
                    throw new ArgumentException("Invalid card code.");
            }

            switch (schar)
            {
                case "S":
                    suit = Suit.Spades;
                    break;
                case "C":
                    suit = Suit.Clubs;
                    break;
                case "H":
                    suit = Suit.Hearts;
                    break;
                case "D":
                    suit = Suit.Diamonds;
                    break;
                default:
                    throw new ArgumentException("Invalid card code.");
            }
        }

        public Color getCardColor()
        {
            return (getColor(suit));
        }

        public static Color getColor(Suit s)
        {
            switch (s)
            {
                case Suit.Spades:
                    return Color.Black;
                case Suit.Clubs:
                    return Color.Black;
                case Suit.Hearts:
                    return Color.Red;
                case Suit.Diamonds:
                    return Color.Red;
                default:
                    throw new ArgumentException("Invalid suit");
            }
        }

        public bool isBower(Suit trump)
        {
            if (rank == Rank.Jack)
            {
                return (getColor(trump) == getCardColor());
            }
            else
            {
                return false;
            }
        }

        public Suit effectiveSuit(Suit trump)
        {
            if(isBower(trump))
            {
                return trump;
            }
            else
            {
                return suit;
            }
        }

        public static string getSuitString(Suit s)
        {
            switch (s)
            {
                case Suit.Spades:
                    return("S");
                case Suit.Clubs:
                    return("C");
                case Suit.Hearts:
                    return("H");
                case Suit.Diamonds:
                    return("D");
                default:
                    return ("Invalid!");
            }
        }

        public static string getRankString(Rank r)
        {
            switch (r)
            {
                case Rank.Nine:
                    return ("9");
                case Rank.Ten:
                    return ("10");
                case Rank.Jack:
                    return ("J");
                case Rank.Queen:
                    return ("Q");
                case Rank.King:
                    return ("K");
                case Rank.Ace:
                    return ("A");
                default:
                    return ("Invalid!");
            }
        }

        public int getTrumpScore(Suit trump)
        {
            switch (rank)
            {
                case Rank.Nine:
                    return 0;
                case Rank.Ten:
                    return 1;
                case Rank.Queen:
                    return 3;
                case Rank.King:
                    return 4;
                case Rank.Ace:
                    return 5;
                case Rank.Jack:
                    // Right bower
                    if (suit == trump)
                    {
                        return 7;
                    }
                    // Left bower
                    else
                    {
                        return 6;
                    }
                default:
                    throw new ArgumentException("Invalid rank.");
            }
        }

        public int getNonTrumpScore()
        {
            switch (rank)
            {
                case Rank.Nine:
                    return 0;
                case Rank.Ten:
                    return 1;
                case Rank.Jack:
                    return 2;
                case Rank.Queen:
                    return 3;
                case Rank.King:
                    return 4;
                case Rank.Ace:
                    return 5;
                default:
                    throw new ArgumentException("Invalid rank");
            }
        }

        public int getCardScore(Suit trump)
        {
            if (effectiveSuit(trump) == trump)
            {
                return (getTrumpScore(trump));
            }
            else
            {
                return (getNonTrumpScore());
            }
        }

        public override string ToString()
        {
            return (getRankString(rank) + getSuitString(suit));
        }
    }

    public enum Color
    {
        Red,
        Black
    }

    public enum Rank
    {
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Suit
    {
        Spades,
        Clubs,
        Hearts,
        Diamonds
    }
}
