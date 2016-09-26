using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem.EucAnaly
{
    public class DecTree
    {
        public List<DecTree> subnodes;
        public static int minDone = 20;
                public Dictionary<Position, Hand> hands;
        public Dictionary<Position, Card> inPlay;
        public Position nextToPlay;
        public int nsTricks;
        public int ewTricks;
        public Card led;
        public Suit trump;

        public DecTree(List<Card> north, List<Card> east, List<Card> south, List<Card> west, Position next, int ns, int ew, Card led, Suit trump)
        {
            subnodes = null;
            hands = new Dictionary<Position, Hand>();
            inPlay = new Dictionary<Position, Card>();

            hands.Add(Position.North, new Hand(north, Position.North));
            hands.Add(Position.East, new Hand(east, Position.East));
            hands.Add(Position.South, new Hand(south, Position.South));
            hands.Add(Position.West, new Hand(west, Position.West));
            nextToPlay = next;
            nsTricks = ns;
            ewTricks = ew;
            this.led = led;
            this.trump = trump;
        }

        public DecTree(List<string> north, List<string> east, List<string> south, List<string> west, Position next, int ns, int ew, Card led, Suit trump)
        {
            subnodes = null;

            hands = new Dictionary<Position, Hand>();
            inPlay = new Dictionary<Position, Card>();

            hands.Add(Position.North, new Hand(north, Position.North));
            hands.Add(Position.East, new Hand(east, Position.East));
            hands.Add(Position.South, new Hand(south, Position.South));
            hands.Add(Position.West, new Hand(west, Position.West));
            nextToPlay = next;
            nsTricks = ns;
            ewTricks = ew;
            this.led = led;
            this.trump = trump;
        }

        public DecTree(DecTree other)
        {
            subnodes = null;
            Position p = Position.North;
            hands = new Dictionary<Position, Hand>();

            do
            {
                hands[p] = new Hand(other.hands[p]);
                p = Hand.nextPosition(p);
            } while (p != Position.North);

            inPlay = new Dictionary<Position, Card>(other.inPlay);
            nextToPlay = other.nextToPlay;
            nsTricks = other.nsTricks;
            ewTricks = other.ewTricks;
            led = other.led;
            trump = other.trump;
        }

        public int alphabeta(int alpha, int beta)
        {
            if (isEmpty())
            {
                return nsTricks;
            }

            subnodes = branchPly();

            int a = alpha;
            int b = beta;

            // is Maximizing team? (ns)
            if (Hand.isNS(nextToPlay))
            {
                for (int i = 0; i < subnodes.Count; i++)
                {
                    a = Math.Max(a, subnodes[i].alphabeta(a, b));

                    if (b <= a)
                    {
                        break;
                    }
                }

                return a;
            }
            else
            {
                for (int i = 0; i < subnodes.Count; i++)
                {
                    b = Math.Min(b, subnodes[i].alphabeta(a, b));

                    if (b <= a)
                    {
                        break;
                    }
                }

                return b;
            }
        }

        public void listByTier(StreamWriter sw)
        {
            for (int i = 0; i <= 20; i++)
            {
                listAtTier(sw, "", 0, i);
            }

            sw.Flush();
        }

        public void listAtTier(StreamWriter sw, string path, int currTier, int desiredTier)
        {
            if (currTier < desiredTier)
            {
                if (subnodes != null)
                {
                    for (int i = 0; i < subnodes.Count; i++)
                    {
                        subnodes[i].listAtTier(sw, path + i.ToString(), currTier + 1, desiredTier);
                    }
                }
            }
            else if (currTier == desiredTier)
            {
                string line = path + "," + nsTricks + "," + ewTricks + ",";
                line += getPosKeyValue(Position.South, inPlay) + ",";
                line += getPosKeyValue(Position.West, inPlay) + ",";
                line += getPosKeyValue(Position.North, inPlay) + ",";
                line += getPosKeyValue(Position.East, inPlay) + ",";
                line += getPosKeyValue(Position.South, hands) + ",";
                line += getPosKeyValue(Position.West, hands) + ",";
                line += getPosKeyValue(Position.North, hands) + ",";
                line += getPosKeyValue(Position.East, hands);

                sw.WriteLine(line);
            }
        }

        public static string getPosKeyValue<T>(Position p, Dictionary<Position, T> d)
            where T : class
        {
            if (d.ContainsKey(p))
            {
                return (d[p].ToString());
            }
            else
            {
                return "";
            }
        }

        // Listing contents of node Boards after expansion but before min-max analysis.
        // Send 0 for top-level tier.
        public void expansionListing(StreamWriter sw, int tier)
        {
            sw.WriteLine("[" + tier + "]: " + ToString());

            if (subnodes != null)
            {
                for (int i = 0; i < subnodes.Count; i++)
                {
                    subnodes[i].expansionListing(sw, tier + 1);
                }
            }

            // top level tier flushes stream.
            if (tier == 0)
            {
                sw.Flush();
            }
        }

        public override string ToString()
        {
            string line = nsTricks + "," + ewTricks + ",";
            line += DecTree.getPosKeyValue(Position.South, inPlay) + ",";
            line += DecTree.getPosKeyValue(Position.West, inPlay) + ",";
            line += DecTree.getPosKeyValue(Position.North, inPlay) + ",";
            line += DecTree.getPosKeyValue(Position.East, inPlay) + ",";
            line += DecTree.getPosKeyValue(Position.South, hands) + ",";
            line += DecTree.getPosKeyValue(Position.West, hands) + ",";
            line += DecTree.getPosKeyValue(Position.North, hands) + ",";
            line += DecTree.getPosKeyValue(Position.East, hands);

            return line;
        }

        public List<DecTree> branchPly()
        {
            Hand h = hands[nextToPlay];
            h.playableSetup(led, trump);

            List<DecTree> pBoards = new List<DecTree>();
            DecTree bTemp;

            for (int i = 0; i < h.cards.Count; i++)
            {
                if (!h.isPlayable(led, trump, i))
                {
                    continue;
                }

                bTemp = new DecTree(this);

                if (!bTemp.hands[nextToPlay].cards.Remove(h.cards[i]))
                {
                    Console.WriteLine("Card to be removed not found");
                }

                if (bTemp.inPlay.Count == 4)
                {
                    bTemp.inPlay = new Dictionary<Position, Card>();
                }

                bTemp.inPlay.Add(nextToPlay, h.cards[i]);
                bTemp.nextToPlay = Hand.nextPosition(bTemp.nextToPlay);

                // Trick
                if (bTemp.inPlay.Count == 4)
                {
                    bTemp.resolveTrick();
                }
                // Lead
                else if (bTemp.inPlay.Count == 1)
                {
                    bTemp.led = h.cards[i];
                }

                pBoards.Add(new DecTree(bTemp));
            }

            return pBoards;
        }

        public bool isEmpty()
        {
            Position p = Position.North;

            do
            {
                if (hands[p].cards.Count > 0)
                {
                    return false;
                }

                p = Hand.nextPosition(p);

            } while (p != Position.North);

            return true;
        }

        // inPlay should have 4 entries
        // nextToPlay should be back to the lead position
        private void resolveTrick()
        {
            if (inPlay.Count != 4)
            {
                throw new ArgumentException("Full trick not in play.");
            }

            Suit ledSuit = led.effectiveSuit(trump);

            Trick t = new Trick(ledSuit, trump, inPlay, nextToPlay);
            Position winner = t.findWinner();

            bool isNS = false;

            switch (winner)
            {
                case Position.South:
                case Position.North:
                    isNS = true;
                    break;
                case Position.East:
                case Position.West:
                    isNS = false;
                    break;
            }

            if (isNS)
            {
                nsTricks++;
            }
            else
            {
                ewTricks++;
            }

            nextToPlay = winner;
        }
    }
}