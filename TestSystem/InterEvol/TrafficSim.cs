using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestSystem.InterEvol
{
    public class TrafficSim
    {
        List<StrategyAbundance> abundances;
        Random rnm;
        StreamWriter sw;

        private int round = 0;

        public TrafficSim()
        {
            rnm = new Random();
            sw = File.CreateText(@"C:\users\aap\desktop\trsim.txt");

            PStrategy go = new PStrategy(1.0);
            PStrategy stop = new PStrategy(0.0);
            TStrategy t5 = new TStrategy(5);
            TStrategy t25 = new TStrategy(25);
            TStrategy t125 = new TStrategy(125);
            PStrategy hundredth = new PStrategy(0.01);

            abundances = new List<StrategyAbundance>();
            abundances.Add(new StrategyAbundance(go));
            abundances.Add(new StrategyAbundance(stop));
            abundances.Add(new StrategyAbundance(t5));
            abundances.Add(new StrategyAbundance(t25));
            abundances.Add(new StrategyAbundance(t125));
            abundances.Add(new StrategyAbundance(hundredth));
        }

        public void run()
        {
            while (true)
            {
                runRound();
                round++;

                if (round % 100 == 0)
                {
                    Console.WriteLine("Continue? (Q-quit, other-continue)");

                    if (Console.ReadLine().StartsWith("Q"))
                    {
                        sw.Flush();
                        sw.Close();
                        return;
                    }
                }
            }
        }

        private void runRound()
        {
            int[,] matchupScores = new int[abundances.Count, abundances.Count];
            double[] stratScores = new double[abundances.Count];
            int leftScore;
            int rightScore;
            double tempScore = 0.0;
            double accumScore = 0.0;

            for (int leftIndex = 0; leftIndex < abundances.Count; leftIndex++)
            {
                for (int rightIndex = leftIndex; rightIndex < abundances.Count; rightIndex++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        contest(abundances[leftIndex].strategy, abundances[rightIndex].strategy, out leftScore, out rightScore);

                        matchupScores[leftIndex, rightIndex] += leftScore;
                        matchupScores[rightIndex, leftIndex] += rightScore;
                    }
                }
            }

            for (int strat = 0; strat < abundances.Count; strat++)
            {
                accumScore = 0.0;

                for (int j = 0; j < abundances.Count; j++)
                {
                    tempScore = (double)matchupScores[strat, j];
                    tempScore *= abundances[j].population;

                    accumScore += tempScore;
                }

                stratScores[strat] = accumScore;
            }

            int rank = 0;

            // order strats
            for (int i = 0; i < abundances.Count; i++)
            {
                rank = 0;

                for (int j = 0; j < abundances.Count; j++)
                {
                    if (stratScores[i] >= stratScores[j])
                    {
                        rank++;
                    }
                }

                abundances[i].rank = rank;
            }

            for (int i = 0; i < abundances.Count; i++)
            {
                sw.WriteLine(string.Format("round: {0}, strategy: {1} score: {2:0.00}", round, abundances[i], stratScores[i]));
            }

            double adjustment = 0.0;
            int maxRank = abundances.Max(x => x.rank);

            for (int i = abundances.Count - 1; i >= 0; i--)
            {
                adjustment = -(((double)abundances[i].rank) - (1.0 + (double)maxRank) / 2) / (double)maxRank;
                adjustment = Math.Pow(2.0, adjustment);

                abundances[i].update(adjustment);
            }

            abundances.Sort(new Comparison<StrategyAbundance>((x, y) => x.rank.CompareTo(y.rank)));

            if (abundances.Count > 20)
            {
                abundances = abundances.Take(20).ToList();
            }

            int opt = rnm.Next(5);

            Strategy ns = null;
            StrategyAbundance a = null;

            if (opt == 0)
            {
                // new prob strat
                ns = new PStrategy(rnm.NextDouble());
            }
            else if (opt == 1)
            {
                // new turn-strat
                ns = new TStrategy(rnm.Next(200));
            }
            else if (opt == 2)
            {
                // mutate a strategy in the top 5.
                ns = abundances[weightedRandomIndex()].strategy.mutate(rnm);
            }
            else if (opt == 3)
            {
                // BinOption two strategies
                /*int leftIndex = 0;
                int rightIndex = 0;

                while (leftIndex == rightIndex)
                {
                    leftIndex = rnm.Next(abundances.Count);
                    rightIndex = rnm.Next(abundances.Count);
                }

                ns = new BinOptionStrategy(abundances[leftIndex].strategy, abundances[rightIndex].strategy);
                 */
                ns = new PStrategy(rnm.NextDouble());
            }
            else if (opt == 4)
            {
                // BinTransfer two strategies in the top 5.
                /*int leftIndex = 0;
                int rightIndex = 0;

                int transIndex = rnm.Next(100);

                while (leftIndex == rightIndex)
                {
                    leftIndex = rnm.Next(abundances.Count);
                    rightIndex = rnm.Next(abundances.Count);
                }

                ns = new BinTransferStrategy(transIndex, abundances[leftIndex].strategy, abundances[rightIndex].strategy);
                 */

                ns = new TStrategy(rnm.Next(100));
            }

            a = new StrategyAbundance(ns);

            abundances.Add(a);

            Console.WriteLine(string.Format("Best on round {0} : {1}", round, abundances[0].ToString()));
            sw.WriteLine(string.Format("Best on round {0} : {1}", round, abundances[0].ToString()));
        }

        private int weightedRandomIndex()
        {
            double m = rnm.NextDouble();

            m = 1.0 / m;

            int mi = (int)Math.Floor(m);
            mi = (mi - 1) % 5;

            if (mi >= abundances.Count)
            {
                mi = 0;
            }

            return mi;
        }

        private void contest(Strategy left, Strategy right, out int leftscore, out int rightscore)
        {
            int turn = 0;

            bool l;
            bool r;

            while (turn < 1000)
            {
                l = left.decide(turn, rnm);
                r = right.decide(turn, rnm);

                if (l)
                {
                    if (r)
                    {
                        leftscore = turn + 100;
                        rightscore = turn + 100;
                        return;
                    }
                    else
                    {
                        leftscore = turn;
                        rightscore = turn + 1;
                        return;
                    }
                }
                else
                {
                    if (r)
                    {
                        leftscore = turn + 1;
                        rightscore = turn;
                        return;
                    }
                    else
                    {
                        turn++;
                        continue;
                    }
                }
            }

            leftscore = 1000;
            rightscore = 1000;
            return;
        }
    }
}
