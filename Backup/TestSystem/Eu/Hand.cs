using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Eu
{
    public class Hand
    {
        public List<Card> cards;
        public Side side;

        public Hand(List<Card> cards, Side side)
        {
            this.cards = cards;
            this.side = side;
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void Remove(Card card)
        {

        }

        public List<Card> LegalPlays(Suit trump, Suit follow, bool yourLead)
        {
            if (yourLead)
            {
                return (cards);
            }
            else
            {
                List<Card> temp = cards.FindAll(new Predicate<Card>(x => x.GetEffectiveSuit(trump) == follow));

                if (temp.Count == 0)
                {
                    // Can't follow suit
                    return cards;
                }
                else
                {
                    // Must follow suit
                    return temp;
                }
            }
        }
    }
}
