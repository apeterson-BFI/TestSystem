using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.EucAnaly
{
    public class Hand
    {
        public List<Card> cards;
        public Position position;
        public bool ledMatch;

        public Hand(List<Card> cards, Position position)
        {
            this.ledMatch = false;
            this.cards = cards;
            this.position = position;
        }

        public Hand(List<string> cardtext, Position position)
        {
            this.ledMatch = false;
            this.position = position;

            this.cards = new List<Card>();

            for (int i = 0; i < cardtext.Count; i++)
            {
                this.cards.Add(new Card(cardtext[i]));
            }
        }

        public Hand(Hand h)
        {
            this.ledMatch = false;
            this.cards = new List<Card>(h.cards);
            this.position = h.position;
        }

        public static Position nextPosition(Position p)
        {
            switch (p)
            {
                case Position.North:
                    return Position.East;
                case Position.East:
                    return Position.South;
                case Position.South:
                    return Position.West;
                case Position.West:
                    return Position.North;
                default:
                    throw new ArgumentException("Invalid position");
            }
        }

        public static bool isNS(Position p)
        {
            return (p == Position.North || p == Position.South);
        }

        public void playableSetup(Card led, Suit trump)
        {
            ledMatch = false;

            if (led == null)
            {
                return;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (led.effectiveSuit(trump) == cards[i].effectiveSuit(trump))
                {
                    ledMatch = true;
                    break;
                }
            }
        }

        public bool isPlayable(Card led, Suit trump, int index)
        {
            if (ledMatch)
            {
                return (cards[index].effectiveSuit(trump) == led.effectiveSuit(trump));
            }
            else
            {
                return true;
            }
        }


        public List<Card> getPlayable(Card led, Suit trump)
        {
            List<Card> temp = new List<Card>();

            // Anything is possible on first play of trick
            if (led == null)
            {
                temp = new List<Card>(cards);
                return temp;
            }

            // If there is at least one card matching led suit then only the matching cards are playable
            for (int i = 0; i < cards.Count; i++)
            {
                if (led.effectiveSuit(trump) == cards[i].effectiveSuit(trump))
                {
                    temp.Add(cards[i]);
                }
            }

            // Otherwise, if no matches exist, anything can be played.
            temp.Clear();
            temp = new List<Card>(cards);

            return temp;
        }

        public static string getPosString(Position p)
        {
            switch (p)
            {
                case Position.North:
                    return "North";
                case Position.East:
                    return "East";
                case Position.South:
                    return "South";
                case Position.West:
                    return "West";
                default:
                    return "Invalid Position";
            }
        }

        public override string ToString()
        {
            string temp = "";

            for (int i = 0; i < 5; i++)
            {
                if (i != 0)
                {
                    temp += ",";
                }

                if (i < cards.Count)
                {
                    temp += cards[i].ToString();
                }
            }

            return temp;
        }
    }

    public enum Position
    {
        North,
        East,
        South,
        West
    }
}
