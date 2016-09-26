using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.EucAnaly
{
    public class Trick
    {
        public Dictionary<Position, Card> inPlay;
        public Position current;
        public Suit ledSuit;
        public bool trumpLed;
        public Position currWin;
        public int currWinScore;
        public int testScore;
        public bool winnerIsTrump;
        public Suit trump;
        public Position nextToPlay;

        public Trick(Suit ledSuit, Suit trump, Dictionary<Position, Card> inPlay, Position nextToPlay)
        {
            this.ledSuit = ledSuit;
            this.trump = trump;
            this.inPlay = inPlay;
            this.nextToPlay = nextToPlay;
            this.current = nextToPlay;

            bool trumpLed = (ledSuit == trump);

            Position currWin = nextToPlay;
            currWinScore = inPlay[nextToPlay].getCardScore(trump);
            winnerIsTrump = trumpLed;
            testScore = 0;
        }

        public Position findWinner()
        {
            current = Hand.nextPosition(current);

            do
            {
                if (trumpLed)
                {
                    trumpLedWinner();
                }
                else
                {
                    if (inPlay[current].effectiveSuit(trump) == trump)
                    {
                        trumpPlayedWinner();
                    }
                    else if (inPlay[current].effectiveSuit(trump) == ledSuit)
                    {
                        suitFollowedWinner();
                    }
                }

                current = Hand.nextPosition(current);

            } while (current != nextToPlay);

            return(currWin);
        }

        private void suitFollowedWinner()
        {
            if (!winnerIsTrump)
            {
                testScore = inPlay[current].getCardScore(trump);

                if (testScore > currWinScore)
                {
                    currWin = current;
                    currWinScore = testScore;
                }
            }
        }

        private void trumpPlayedWinner()
        {
            if (!winnerIsTrump)
            {
                winnerIsTrump = true;
                currWin = current;
                currWinScore = inPlay[current].getCardScore(trump);
            }
            else
            {
                testScore = inPlay[current].getCardScore(trump);

                if (testScore > currWinScore)
                {
                    currWin = current;
                    currWinScore = testScore;
                }
            }
        }

        private void trumpLedWinner()
        {
            if (inPlay[current].effectiveSuit(trump) == trump)
            {
                testScore = inPlay[current].getCardScore(trump);

                if (testScore > currWinScore)
                {
                    currWin = current;
                    currWinScore = testScore;
                }
            }
        }
    }
}
