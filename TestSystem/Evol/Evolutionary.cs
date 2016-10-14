using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace TestSystem.Evol
{
    public class Evolutionary
    {
        public static void runRandomList(bool wait)
        {
            extree reference = new extree(expr_type.exp, double.NaN);
            Random rnm = new Random();

            double bestScore = double.PositiveInfinity;
            double score;

            extree r;

            int n = 0;

            while(bestScore > 0)
            {
                r = extree.makeRandom(rnm);

                r.ScoreSqrt(reference, 0.0, 1.0, 0.01);

                score = r.true_score;

                if(score < bestScore)
                {
                    bestScore = score;

                    Console.WriteLine(string.Format("{0} ({1}) on attempt {2}", r.ToString(), score, n));

                    if (wait)
                    {
                        Console.ReadLine();
                    }
                }

                n++;
            }
        }

        public static void runEnviro()
        {
            extree reference = new extree(expr_type.exp, double.NaN);

            Enviro e = new Enviro(reference, 0.0, 1.0);
            e.run();
        }

        public static void RunParamTestNM()
        {
            // Deg - 1 to 8
            // xdif - 0.0001, 0.001, 0.01, 0.04, 0.1, 0.25
            // xch - 0.0001, 0.001, 0.01, 0.04, 0.1, 0.25
            // reset - 5, 10, 15, 20, 25, 50, 75, 100

            // x ^ 2 + 1
            extree reference = new extree(expr_type.plus, 0.0);                 // +
            reference.left = new extree(expr_type.pow, 0.0);                    // ^
            reference.left.left = new extree(expr_type.constant, double.NaN);   // x
            reference.left.right = new extree(expr_type.constant, 2.0);         // 2.0
            reference.right = new extree(expr_type.constant, 1.0);              // 1.0

            double best = double.PositiveInfinity;
            double bestdg=0.0;
            double bestxdif=0.0;
            double bestxch=0.0;
            double bestreset=0.0;
            extree bestxp = null; ;

            ExprNewtonMethod n;

            int[] dg = { 1, 2, 3, 4, 5, 6, 7, 8 };
            double[] xdif = { 0.0001, 0.001, 0.01, 0.04, 0.1, 0.25 };
            double[] xch = { 0.0001, 0.001, 0.01, 0.04, 0.1, 0.25 };
            int[] reset = { 5, 10, 15, 20, 25, 50, 75, 100 };

            for (int i = 0; i < dg.Length; i++)
            {
                for (int j = 0; j < xdif.Length; j++)
                {
                    for (int k = 0; k < xch.Length; k++)
                    {
                        for (int l = 0; l < reset.Length; l++)
                        {
                            n = new ExprNewtonMethod(reference, 0.0, 1.0, 0.02, 0.0, 100000, dg[i], xdif[j], xch[k], reset[l]);
                            n.ProcessNSteps(300, false);

                            Console.WriteLine("Score with dg: " + dg[i] + ", xdif: " + xdif[j] + ", xch: " + xch[k] + ", reset: " + reset[l] + " = " + n.best_current_score);
                            Console.WriteLine(n.best.ToString());

                            if (n.best_current_score < best)
                            {
                                bestdg = dg[i];
                                bestxdif = xdif[j];
                                bestxch = xch[k];
                                bestreset = reset[l];
                                bestxp = n.best;
                                best = n.best_current_score;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Best with dg: " + bestdg + ", xdif: " + bestxdif + ", xch: " + bestxch + ", reset: " + bestreset + " = " + best);
            Console.WriteLine(bestxp.ToString());

            Console.WriteLine();
            Console.WriteLine("Press enter to continue:");
            Console.ReadLine();
        }

        public static void RunTestbedx21()
        {
            extree reference = new extree(expr_type.exp, double.NaN);

            ExprGenr gev = new ExprGenr(reference, 0.0, 1.0, 0.01, 0.001, 300000, 3);
            extree ex = gev.Process_Until_Terminal();

            Console.Out.WriteLine("Press any key to continue:");
            string sk = Console.In.ReadLine();



        }

        public static void Genr()
        {
            // x ^ 2 + x
            extree reference = new extree(expr_type.plus, 0.0);                 // +
            reference.left = new extree(expr_type.pow, 0.0);                    // ^
            reference.left.left = new extree(expr_type.constant, double.NaN);   // x
            reference.left.right = new extree(expr_type.constant, 2.0);         // 2.0
            reference.right = new extree(expr_type.constant, double.NaN);              // x

            ExprGenr gev = new ExprGenr(reference, 0, 2, 0.01, 0.05, 300000, 3);
            extree ex = gev.Process_Until_Terminal();

            Console.Out.WriteLine("Press any key to continue:");
            string sk = Console.In.ReadLine();
        }

        static void XM()
        {
            extree e = new extree(expr_type.plus, 0.0);
            e.left = new extree(expr_type.pow, 0.0);
            e.left.left = new extree(expr_type.constant, double.NaN);
            e.left.right = new extree(expr_type.constant, 2.0);
            e.right = new extree(expr_type.constant, 1.0);

            extree f = new extree(expr_type.exp, double.NaN);

            ExprMutate xm = new ExprMutate(1, 0.35, 0.01, 0.0, e, 0.0, 10.0, 0.25);
            xm.Process_Until_Terminal();
        }

        static void XP()
        {
            extree e = new extree(expr_type.plus, 0.0);
            e.left = new extree(expr_type.pow, 0.0);
            e.left.left = new extree(expr_type.constant, double.NaN);
            e.left.right = new extree(expr_type.constant, 2.0);
            e.right = new extree(expr_type.constant, 1.0);

            ExprPFit pm = new ExprPFit(0.0, 10.0, 0.25, e, 0.25, 0.1, 3, 2, -6, 6, 10000.0, 2);
            pm.Process_Until_Terminal();
        }

        static void XG()
        {
            extree e = new extree(expr_type.plus, 0.0);
            e.left = new extree(expr_type.pow, 0.0);
            e.left.left = new extree(expr_type.constant, double.NaN);
            e.left.right = new extree(expr_type.constant, 2.0);
            e.right = new extree(expr_type.constant, 1.0);

            ExprGenr g = new ExprGenr(e, 0.0, 10.0, 0.25, 0.25, Int32.MaxValue, 4);
            g.Process_Until_Terminal();
        }
    }
}
