using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TestSystem.EucAnaly
{
    public static class AnalysisSetup
    {
        public static void multiRandomSearch()
        {
            Stopwatch sw = new Stopwatch();

            StreamWriter s = File.CreateText(@"c:\users\aap\dropbox\other\multianalysis.txt");

            sw.Start();

            for (int i = 0; i < 100; i++)
            {
                givenHandRandomAB(s);
                Console.WriteLine("Search #: " + i + " completed.");
            }

            sw.Stop();

            s.WriteLine("100 given hand random searches took Time: " + sw.Elapsed.ToString());
            s.Flush();
            s.Close();
        }

        public static void givenHandRandomAB(StreamWriter sw)
        {
            Random rnm = new Random();

            List<string> pool = new List<string>() 
                {   "9S", "10S", "QS", "KS", "AS", 
                    "9C", "QC", "AC", 
                    "9H", "10H", "JH", "QH", "KH", "AH",
                    "9D", "10D", "JD", "QD", "AD"};

            List<string> e = new List<string>() { "JS", "JC", "KC", "10C", "KD" };
            List<string> w = new List<string>();
            List<string> n = new List<string>();
            List<string> s = new List<string>();

            int r;
            string res;

            int q;

            for (int i = 0; i < 15; i++)
            {
                r = rnm.Next(pool.Count);
                res = pool[r];
                pool.RemoveAt(r);

                q = i / 5;

                switch (q)
                {
                    case 0:
                        w.Add(res);
                        break;
                    case 1:
                        n.Add(res);
                        break;
                    case 2:
                        s.Add(res);
                        break;
                    default:
                        throw new ArgumentException("Error");
                }
            }

            DecTree d = new DecTree(n, e, s, w, Position.West, 0, 0, null, Suit.Spades);
            
            int ns = d.alphabeta(-1, 6);
            sw.WriteLine(ns + "," + (5 - ns).ToString() + "," + d.ToString());
        }
    }
}
