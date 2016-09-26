using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class ExprMutate : ExprEvolver
    {
        public List<extree> alphas;
        public List<double> alpha_scores;
        public double binaryPref;
        public double binaryDecay;
        public double alphaVsDelta;
        public extree bestExpr;
        public double bestScore;
        public extree delta;
        public double delta_score;
        public static Random rnm = new Random();
        public static double imp_tol = 0.01;

        public ExprMutate(int numAlphas, double binaryPref, double binaryDecay, double alphaVsDelta, extree reference, double xmin, double xmax, double prec)
        {
            alphas = new List<extree>(numAlphas);
            alpha_scores = new List<double>(numAlphas);
            this.binaryDecay = binaryDecay;
            this.binaryPref = binaryPref;
            this.alphaVsDelta = alphaVsDelta;
            this.terminal_score = 0.005;
            this.reference = reference;
            this.xmin = xmin;
            this.xmax = xmax;
            this.xprec = prec;
            bestExpr = null;
            bestScore = double.PositiveInfinity;
            current_step = 1;
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            // Algorithm:
            // Generate deltas by random selection of extrees
            // The best deltas compete with existing alphas to form the current set of alphas
            // Alphas are mutated in diferent parts by xshift, yshift, linear, and power-linear modifications
            // the modifications that improve things stay, those that do not are gone.

            SetupAlphas();
            ScoreAlphas();

            while (bestScore >= terminal_score && current_step < n)
            {
                current_step++;
                // delta gen
                delta = extree.RandomExtreeFromLayout(BinaryLayout.RandomLayout(binaryPref, binaryDecay));
                delta.ScoreSqrt(reference, xmin, xmax, xprec);
                delta_score = delta.true_score;

                for (int i = 0; i < alphas.Count; i++)
                {
                    if (BetterScore(delta_score, alpha_scores[i]))
                    {
                        alphas[i] = delta.simplify();
                        alpha_scores[i] = delta_score;
                        break;
                    }
                }

                for (int i = 0; i < alphas.Count; i++)
                {
                    // 1 - f(x) o (x + a) - x shift
                    // 0 - (x + a) o f(x) - y shift
                    // 2 - f(x) o (a * x + b) - linear transform
                    // 3 - (x^a) o f(x) or f(x) ^ a - power transform

                    int choice = rnm.Next(4);

                    extree x;
                    double tempval;

                    switch (choice)
                    {
                        // y shift
                        case 0:
                            tempval = ShiftVar();
                            x = new extree(expr_type.plus, 0.0);
                            x.left = alphas[i].CopyExtree();
                            x.right = new extree(expr_type.constant, tempval);
                            x.ScoreSqrt(reference, xmin, xmax, xprec);

                            if (BetterScore(x.true_score, alpha_scores[i]))
                            {
                                alphas[i] = x.simplify();
                                alpha_scores[i] = x.true_score;
                            }
                            break;
                        // x shift
                        case 1:
                            tempval = ShiftVar();
                            x = new extree(expr_type.compose, 0.0);
                            x.left = alphas[i].CopyExtree();
                            x.right = new extree(expr_type.plus, 0.0);
                            x.right.left = new extree(expr_type.constant, double.NaN);
                            x.right.right = new extree(expr_type.constant, tempval);
                            x.ScoreSqrt(reference, xmin, xmax, xprec);

                            if (BetterScore(x.true_score, alpha_scores[i]))
                            {
                                alphas[i] = x.simplify();
                                alpha_scores[i] = x.true_score;
                            }
                            break;
                        // linear transform
                        case 2:
                            x = new extree(expr_type.compose, 0.0);
                            x.left = alphas[i].CopyExtree();
                            x.right = new extree(expr_type.plus, 0.0);
                            x.right.left = new extree(expr_type.times, 0.0);
                            tempval = ShiftVar();
                            x.right.left.left = new extree(expr_type.constant, tempval);
                            x.right.left.right = new extree(expr_type.constant, double.NaN);
                            tempval = ShiftVar();
                            x.right.right = new extree(expr_type.constant, tempval);
                            x.ScoreSqrt(reference, xmin, xmax, xprec);

                            if (BetterScore(x.true_score, alpha_scores[i]))
                            {
                                alphas[i] = x;
                                alpha_scores[i] = x.true_score;
                            }
                            break;
                        // power transform
                        case 3:
                            x = new extree(expr_type.pow, 0.0);
                            x.left = alphas[i].CopyExtree();
                            tempval = rnm.NextDouble() + 0.5;
                            x.right = new extree(expr_type.constant, tempval);
                            x.ScoreSqrt(reference, xmin, xmax, xprec);

                            if (BetterScore(x.true_score, alpha_scores[i]))
                            {
                                alphas[i] = x;
                                alpha_scores[i] = x.true_score;
                            }
                            break;
                    }

                    if (BetterScore(alpha_scores[i], bestScore))
                    {
                        bestScore = alpha_scores[i];
                        bestExpr = alphas[i].simplify();

                        if (msg)
                        {
                            Console.Out.WriteLine("Mutate Evol At Step #" + current_step + ":");
                            Console.Out.WriteLine("Best Expression: " + bestExpr.ToString());
                            Console.Out.WriteLine("With Error Score: " + bestScore);
                        }
                    }
                }
            }

            return bestExpr;
        }

        private double ShiftVar()
        {
            // -(xmax - xmin)/10 to (xmax - xmin) /10
            return ((rnm.NextDouble() * 2.0 - 1.0) * (xmax - xmin) / 10.0);
        }

        public bool BetterScore(double curr, double prev)
        {
            return (curr - prev < -imp_tol);
        }

        private void SetupAlphas()
        {
            for (int i = 0; i < alphas.Capacity; i++)
            {
                alphas.Add(extree.RandomExtreeFromLayout(BinaryLayout.RandomLayout(binaryPref, binaryDecay)));
            }
        }

        public void ScoreAlphas()
        {
            for (int i = 0; i < alphas.Count; i++)
            {
                alphas[i].ScoreSqrt(reference, xmin, xmax, xprec);
                alpha_scores.Add(alphas[i].true_score);
            }
        }
    }
}
