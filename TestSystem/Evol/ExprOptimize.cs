/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class ExprOptimize : ExprEvolver
    {
        public double turn_start_score;
        public int exp_kept;
        public int exp_newe;

        public ExprOptimize(extree refxp, double start, double end, double prec, double term, int steps, int kept, int newe)
        {
            reference = refxp;
            best_current_score = double.PositiveInfinity;
            turn_start_score = best_current_score;
            stepN = 0;
            exps = new List<extree>(40);
            xmin = start;
            xmax = end;
            xprec = prec;
            terminal_score = term;
            step_max = steps;
            exp_kept = kept;
            exp_newe = newe;

            Random rnm = new Random();

            SetupXFunctions(rnm);
            SetupConstantFunctions(rnm);
        }

        public override extree Process_Until_Terminal()
        {
            while ((best_current_score > terminal_score && stepN <= step_max) || best_current_score == double.NaN)
            {
                Step();
            }

            return (exps[0]);
        }

        public override void Step()
        {
            // Prune to 20

            foreach (extree e in exps)
            {
                e.ScoreSqrt(reference, xmin, xmax, xprec);
            }

            for (int i = 0; i < exps.Count; i++)
            {
                if (double.IsNaN(exps[i].true_score))
                {
                    exps[i].true_score = double.PositiveInfinity;
                    exps[i].true_score = double.PositiveInfinity;
                }
            }

            best_current_score = exps[0].true_score;

            exps.Sort(new Comparison<extree>(extree.CompareScores));

            if (exps.Count > exp_kept)
            {
                exps.RemoveRange(exp_kept, exps.Count - exp_kept);
            }

            extree w;

            for (int i = 0; i < exp_newe; i++)
            {
                w = GetRandomNovelty();
                w.ScoreSqrt(reference, xmin, xmax, xprec);

                if (w.true_score == double.NaN || w.true_score == double.NaN)
                {
                    w.true_score = double.PositiveInfinity;
                }

                exps.Add(w);
            }

            expr_type ty;
            extree a;
            double temp_score;
            int c = exps.Count;
            extree best = null;


            for (int q = 0; q < 18; q++)
            {
                ty = (expr_type)q;

                if (extree.IsUnary(ty))
                {
                    for (int i = 0; i < c; i++)
                    {
                        a = new extree(expr_type.compose, 0.0);
                        a.left = new extree(ty, double.NaN);
                        a.right = exps[i];

                        a.ScoreSqrt(reference, xmin, xmax, xprec);
                        temp_score = a.true_score;

                        if (temp_score < best_current_score)
                        {
                            best = a;
                            best_current_score = temp_score;
                        }

                        a = new extree(expr_type.compose, 0.0);
                        a.left = exps[i];
                        a.right = new extree(ty, double.NaN);

                        a.ScoreSqrt(reference, xmin, xmax, xprec);
                        temp_score = a.true_score;

                        if (temp_score < best_current_score)
                        {
                            best = a;
                            best_current_score = temp_score;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < c; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            a = new extree(ty, 0.0);
                            a.left = exps[i];
                            a.right = exps[j];
                            a.ScoreSqrt(reference, xmin, xmax, xprec);

                            if (double.IsNaN(a.true_score))
                                a.true_score = double.PositiveInfinity;

                            if (double.IsNaN(a.true_score))
                                a.true_score = double.PositiveInfinity;

                            temp_score = a.true_score;

                            if (temp_score < best_current_score)
                            {
                                best = a;
                                best_current_score = temp_score;
                            }
                        }
                    }
                }
            }

            if(best != null)
                exps.Add(best);

            exps.Sort(new Comparison<extree>(extree.CompareScores));

            if (exps[0].true_score < turn_start_score)
            {
                Console.Out.WriteLine("Optimizer At Step #" + stepN + ":");
                Console.Out.WriteLine("Best Expression: " + exps[0].ToString());
                Console.Out.WriteLine("With Error Score: " + exps[0].true_score);
            }

            turn_start_score = best_current_score;
            stepN++;
        }
    }
}
*/