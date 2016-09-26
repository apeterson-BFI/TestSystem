using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public abstract class ExprEvolver
    {
        public List<extree> exps;
        public int stepN;
        public extree reference;
        public double xmin;
        public double xmax;
        public double xprec;
        public double terminal_score;
        public double best_current_score;
        public int step_max;
        public int current_step;

        public ExprEvolver()
        {
            best_current_score = double.PositiveInfinity;
            stepN = 0;
        }

        public ExprEvolver(extree refxp, double start, double end, double prec, double term, int steps)
        {
            reference = refxp;
            best_current_score = double.PositiveInfinity;
            stepN = 0;
            exps = new List<extree>(40);
            xmin = start;
            xmax = end;
            xprec = prec;
            terminal_score = term;
            step_max = steps;

            Random rnm = new Random();

            SetupXFunctions(rnm);
            SetupConstantFunctions(rnm);
        }

        public virtual extree Process_Until_Terminal()
        {
            return (ProcessNSteps(Int32.MaxValue, true));
        }

        protected void SetupConstantFunctions(Random rnm)
        {
            // Constant functions
            for (int i = 0; i < 20; i++)
            {
                exps.Add(GetConstantFunction(rnm));
            }
        }

        protected extree GetConstantFunction(Random rnm)
        {
            extree temp;

            int p;

            p = rnm.Next(18);

            expr_type e = (expr_type)p;

            if (extree.IsUnary(e))
                temp = new extree((expr_type)p, GetConstant());
            else
            {
                temp = new extree(e, 0.0);
                temp.left = new extree(expr_type.constant, GetConstant());
                temp.right = new extree(expr_type.constant, GetConstant());
            }

            return temp;
        }

        protected extree GetXFunction(Random rnm)
        {
            double v = 0.0;
            int p;

            extree temp;
            p = rnm.Next(18);
            expr_type e = (expr_type)p;

            if (extree.IsUnary(e))
                temp = new extree(e, double.NaN);
            else
            {
                temp = new extree(e, 0.0);
                p = rnm.Next(2);

                if (p == 0)
                {
                    temp.left = new extree(expr_type.constant, double.NaN);
                    v = GetConstant();
                    temp.right = new extree(expr_type.constant, v);
                }
                else if (p == 1)
                {
                    temp.right = new extree(expr_type.constant, double.NaN);
                    v = GetConstant();
                    temp.left = new extree(expr_type.constant, v);
                }
            }

            return temp;
        }

        protected void SetupXFunctions(Random rnm)
        {
            // X functions
            for (int i = 0; i < 20; i++)
            {
                exps.Add(GetXFunction(rnm));
            }
        }

        public abstract extree ProcessNSteps(int n, bool msg);

        public virtual void Step()
        {
            foreach (extree e in exps)
            {
                e.ScoreSqrt(reference, xmin, xmax, xprec);
            }

            for(int i = 0; i < exps.Count; i++)
            {
                if (double.IsNaN(exps[i].true_score))
                {
                    exps[i].true_score = double.PositiveInfinity;
                }
            }

            exps.Sort(new Comparison<extree>(extree.CompareScores));

            if (exps[0].true_score < best_current_score)
            {
                Console.Out.WriteLine("At Step #" + stepN + ":");
                Console.Out.WriteLine("Best Expression: " + exps[0].ToString());
                Console.Out.WriteLine("With Error Score: " + exps[0].true_score);

                best_current_score = exps[0].true_score;
            }

            List<extree> newex = new List<extree>();

            for (int i = 0; i < 10; i++)
            {
                newex.Add(GetRandomPlebMix());
            }

            for (int i = 0; i < 10; i++)
            {
                newex.Add(GetRandomNovelty());
            }
            
            int range_left = 20;

            // Remove Worst
            exps.RemoveRange(exps.Count - range_left, range_left);

            for (int i = 0; i < newex.Count; i++)
            {
                exps.Add(newex[i]);
            }

            stepN++;
        }

        // (ex1 + ex2) / 2
        public extree GetBestExprAverage()
        {
            Random rnm = new Random();
            int p1;
            int p2;

            int v = exps.Count / 3;
            extree gm1 = new extree(expr_type.divides, 0.0);
            extree gm1_a = new extree(expr_type.plus, 0.0);
            extree gm1_b = new extree(expr_type.constant, 2.0);

            p1 = rnm.Next(v);
            p2 = rnm.Next(v);
            p2 = (p2 == p1 ? p2 + 1 : p2);
            gm1_a.left = exps[p1];
            gm1_a.right = exps[p2];
            gm1.left = gm1_a;
            gm1.right = gm1_b;

            return gm1;
        }

        // Sqrt(ex1 * ex2)
        public extree GetBestExprGeomAverage()
        {
            Random rnm = new Random();
            int p1;
            int p2;

            int v = exps.Count / 3;
            extree gm1 = new extree(expr_type.pow, 0.0);
            extree gm1_a = new extree(expr_type.times, 0.0);
            extree gm1_b = new extree(expr_type.constant, (1.0 / 2.0));

            p1 = rnm.Next(v);
            p2 = rnm.Next(v);
            p2 = (p2 == p1 ? p2 + 1 : p2);
            gm1_a.left = exps[p1];
            gm1_a.right = exps[p2];
            gm1.left = gm1_a;
            gm1.right = gm1_b;

            return gm1;
        }

        // Random operation on random expression(s)
        public extree GetRandomPlebMix()
        {
            Random rnm = new Random();

            int o1 = rnm.Next(19);
            expr_type ot = (expr_type)o1;

            if (extree.IsUnary(ot))
            {
                if(ot == expr_type.constant)
                    return(GetRandomPlebMix());

                int p1 = rnm.Next(exps.Count);
                extree pm = new extree(expr_type.compose, 0.0);

                int c = rnm.Next(2);

                if (c == 0)
                {
                    pm.left = exps[p1];
                    extree pma = new extree(ot, double.NaN);
                    pm.right = pma;
                }
                else
                {
                    pm.right = exps[p1];
                    extree pma = new extree(ot, double.NaN);
                    pm.left = pma;
                }

                return pm;
            }
            else
            {
                int p1 = rnm.Next(exps.Count);
                int p2 = rnm.Next(exps.Count);

                extree pm = new extree(ot, 0.0);
                pm.left = exps[p1];
                pm.right = exps[p2];

                return pm;
            }
        }

        public extree GetRandomNovelty()
        {
            Random rnm = new Random();

            int p = rnm.Next(19);
            expr_type px = (expr_type)p;

            if (extree.IsUnary(px))
            {
                extree pm = new extree(px, GetConstant());
                return pm;
            }
            else
            {
                extree pm = new extree(px, 0.0);
                extree pml;
                extree pmr;

                double v1;
                double v2;

                v1 = GetConstant();
                v2 = GetConstant();
                p = rnm.Next(2);


                if (p == 0)
                {
                    pml = new extree(expr_type.constant, double.NaN);
                    pm.left = pml;
                }
                else
                {
                    pml = new extree(expr_type.constant, v1);
                    pm.left = pml;
                }

                p = rnm.Next(2);

                if (p == 0)
                {
                    pmr = new extree(expr_type.constant, double.NaN);
                    pm.right = pmr;
                }
                else
                {
                    pmr = new extree(expr_type.constant, v2);
                    pm.right = pmr;
                }

                return pm;
            }
        }

        public double GetConstant()
        {
            Random rnm = new Random();
            /*
            // 0.0 - 1.0
            double v1 = rnm.NextDouble();
            // 0.0 - 20.0
            v1 = v1 * 20.0;
            // -10.0 - 10.0
            v1 = v1 - 10.0;

            return v1;
             *
             */

            if (rnm.Next(6) == 5)
            {
                return double.NaN;
            }

            double v1 = rnm.NextDouble() * 40.0 - 20.0;
            return v1;
        }
    }
}
