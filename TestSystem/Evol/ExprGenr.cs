using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class ExprGenr : ExprEvolver
    {
        public int generations;
        public List<extree>[] gen_exps;
        public static int extreeCount = Enum.GetValues(typeof(expr_type)).Length;
        public bool msg_alert;


        public ExprGenr(extree refxp, double start, double end, double prec, double term, int steps, int gen)
        {
            xmin = start;
            xmax = end;
            xprec = prec;
            terminal_score = term;
            generations = gen;
            gen_exps = new List<extree>[gen];
            reference = refxp;
            step_max = steps;
            current_step = 1;

            for (int i = 0; i < gen; i++)
            {
                gen_exps[i] = new List<extree>();
            }

            rnm = new Random();

            for (int i = 0; i < 80; i++)
            {
                if (i % 2 == 0)
                {
                    gen_exps[0].Add(GetXFunction());
                }
                else
                {
                    gen_exps[0].Add(GetConstantFunction());
                }
            }
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            this.msg_alert = msg;
            while (best_current_score > terminal_score && current_step <= n)
            {
                Step();
            }

            if (best_current_score < terminal_score)
            {
                Console.WriteLine("Wow, best score " + best_current_score + " ");
            }

            List<extree> exfinal = new List<extree>();

            for (int i = 0; i < generations; i++)
            {
                for (int j = 0; j < gen_exps[i].Count; j++)
                {
                    exfinal.Add(gen_exps[i][j]);
                }
            }

            exfinal.Sort(new Comparison<extree>(extree.CompareScores));

            return exfinal[0]; 
        }

        public override extree Process_Until_Terminal()
        {
            return (ProcessNSteps(Int32.MaxValue, true));
        }

        public override void Step()
        {
            for (int i = 0; i < generations; i++)
            {
                foreach (extree e in gen_exps[i])
                {
                    e.ScoreSqrt(reference, xmin, xmax, xprec);
                }
            }

            gen_exps[0].Sort(new Comparison<extree>(extree.CompareScores));

            int removal = gen_exps[0].Count / 2;

            if (gen_exps[0][0].true_score < best_current_score)
            {
                if (msg_alert)
                {
                    Console.Out.WriteLine("Generational At Step #" + current_step + ":");
                    Console.Out.WriteLine("Best Expression: " + gen_exps[0][0].ToString());
                    Console.Out.WriteLine("With score: " + gen_exps[0][0].true_score);
                    Console.Out.WriteLine("With Sum Squared Error: " + (gen_exps[0][0].true_score / (1 + extree.sizepenalty * (double)gen_exps[0][0].nodeCount())));
                }

                best_current_score = gen_exps[0][0].true_score;
            }

            List<extree> newex = new List<extree>();

            for (int i = 0; i < removal; i++)
            {
                if (i % 3 == 1)
                {
                    newex.Add(GetRandomNovelty());
                }
                else if (i % 3 == 0)
                {
                    newex.Add(GetGenCombinator());
                }
                else
                {
                    newex.Add(SimplifyRandomExpr());
                }
            }

            if (removal != 0)
            {
                gen_exps[0].RemoveRange(gen_exps[0].Count - removal, removal);
            }

            for (int i = 0; i < gen_exps[1].Count; i++)
            {
                gen_exps[0].Add(gen_exps[1][i]);
            }

            for (int i = 2; i < generations; i++)
            {
                gen_exps[i - 1] = gen_exps[i];
            }

            gen_exps[generations - 1] = newex;

            current_step++;
        }

        public extree SimplifyRandomExpr()
        {
            int gen = rnm.Next(generations);
            int index;

            while (gen_exps[gen].Count == 0)
            {
                gen = rnm.Next(generations);
            }
            
            index = rnm.Next(gen_exps[gen].Count);

            extree s = gen_exps[gen][index];
            extree ns = gen_exps[gen][index];

            do
            {
                s = ns;
                ns = s.simplify();
            } while (!ns.Equals(s));

            return ns;
        }

        public extree GetGenCombinator()
        {
            int group1;
            int sel1;
            int group2;
            int sel2;

            int o1 = rnm.Next(extreeCount);
            expr_type ot = (expr_type)o1;

            if (extree.IsUnary(ot))
            {
                group1 = rnm.Next(generations);

                while (gen_exps[group1].Count == 0)
                {
                    group1 = rnm.Next(generations);
                }

                sel1 = rnm.Next(gen_exps[group1].Count);

                extree pm = new extree(expr_type.compose, 0.0);

                int c = rnm.Next(2);

                if (c == 0)
                {
                    pm.left = gen_exps[group1][sel1];
                    extree pma = new extree(ot, double.NaN);
                    pm.right = pma;
                }
                else
                {
                    pm.right = gen_exps[group1][sel1];
                    extree pma = new extree(ot, double.NaN);
                    pm.left = pma;
                }

                return pm;
            }
            else
            {
                group1 = rnm.Next(generations);

                while (gen_exps[group1].Count == 0)
                {
                    group1 = rnm.Next(generations);
                }

                group2 = rnm.Next(generations);

                while (gen_exps[group2].Count == 0)
                {
                    group2 = rnm.Next(generations);
                }

                sel1 = rnm.Next(gen_exps[group1].Count);
                sel2 = rnm.Next(gen_exps[group2].Count);

                extree pm = new extree(ot, 0.0);
                pm.left = gen_exps[group1][sel1];
                pm.right = gen_exps[group2][sel2];

                return pm;
            }
        }
    }
}
