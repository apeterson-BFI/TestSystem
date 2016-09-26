using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Evol;

namespace TestSystem.Evol
{
    public class ExprPFit : ExprEvolver
    {
        public double h;
        public int nterms;
        public static Random rnm = new Random();
        public int nran;
        public int ndesc;
        public List<List<double>> ranTerms;
        public List<List<double>> descTerms;
        public double sMin;
        public double sMax;
        public extree bestExpr;
        public List<extree> ranExtrees;
        public List<extree> descExtrees;
        public double baseN;
        public double powN;

        public ExprPFit(double xmin, double xmax, double xprec, extree reference, double err, double h, int nran, int ndesc, double sMin, double sMax, double baseN, double powN)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.xprec = xprec;
            this.reference = reference;
            this.terminal_score = err;
            this.h = h;
            this.nterms = 1;
            this.stepN = 0;
            this.ranTerms = new List<List<double>>();
            this.descTerms = new List<List<double>>();
            this.nran = nran;
            this.ndesc = ndesc;
            this.sMin = sMin;
            this.sMax = sMax;
            this.best_current_score = double.PositiveInfinity;
            this.bestExpr = null;
            ranExtrees = new List<extree>();
            descExtrees = new List<extree>();
            this.baseN = baseN;
            this.powN = powN;

            for (int i = 0; i < nran; i++)
            {
                ranTerms.Add(RandTermBuilder());
                ranExtrees.Add(BuildP(ranTerms[i]));
                ranExtrees[i].ScoreSqrt(reference, xmin, xmax, xprec);
            }

            for (int i = 0; i < ndesc; i++)
            {
                descTerms.Add(RandTermBuilder());
                descExtrees.Add(BuildP(descTerms[i]));
                descExtrees[i].ScoreSqrt(reference, xmin, xmax, xprec);
            }
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            // n randoms, m descents
            // randoms pick new places in polynomial [degree] space.
            // descents perturb their position to find the slopespace of the surrounding area in terms of error, and follow the path of greatest descent.
            // the number of stemps taking determines the nterms
            // 
            // if(stepN >= 50 * Pow(3, degree) nterms++
            // so the following table lists the first few:
            // 0 - 150 Constant
            // 151-450 Linear
            // 451-1350 Quadratic
            // ...
            // If a random is better than a descent, it replaces the worst of the descents
            // sMin and sMax give the allowed range for poly coefficients
            while (best_current_score > terminal_score && stepN < n)
            {
                for (int i = 0; i < nran; i++)
                {
                    for (int j = 0; j < ndesc; j++)
                    {
                        if (ranExtrees[i].true_score < descExtrees[j].true_score)
                        {
                            descExtrees[j] = ranExtrees[i];
                            ranExtrees[i] = null;
                            break;
                        }
                    }
                }

                for (int j = 0; j < ndesc; j++)
                {
                    if (descExtrees[j].true_score < best_current_score)
                    {
                        bestExpr = descExtrees[j];
                        best_current_score = descExtrees[j].true_score;

                        if (msg)
                        {
                            Console.Out.WriteLine("PFit Evol At Step #" + stepN + ":");
                            Console.Out.WriteLine("Best Expression: " + bestExpr.ToString());
                            Console.Out.WriteLine("With Error Score: " + best_current_score);
                        }

                    }

                    List<double> tq = RandTermBuilder(-h, h);
                    extree tqe;
                    List<double> tqo = new List<double>();
                    extree tqoe;

                    for (int i = 0; i < nterms; i++)
                    {
                        tqo.Add(0);
                    }

                    for (int i = 0; i < nterms; i++)
                    {
                        tq[i] += descTerms[j][i];
                        tqo[i] = descTerms[j][i] - tq[i];
                    }

                    tqe = BuildP(tq);
                    tqoe = BuildP(tqo);

                    tqe.ScoreSqrt(reference, xmin, xmax, xprec);
                    tqoe.ScoreSqrt(reference, xmin, xmax, xprec);

                    // tqe -> h best
                    if (tqe.true_score < descExtrees[j].true_score && tqe.true_score < tqoe.true_score)
                    {
                        // make tqe the new desc[j]
                        descExtrees[j] = tqe;
                    }
                    // tqoe -> -h best
                    else if (tqoe.true_score < descExtrees[j].true_score && tqoe.true_score < tqe.true_score)
                    {
                        descExtrees[j] = tqoe;
                    }
                    // desc[j] best.
                    // no change
                    else
                    {
                        continue;
                    }
                }

                for (int i = 0; i < nran; i++)
                {
                    ranTerms[i] = RandTermBuilder();
                    ranExtrees[i] = BuildP(ranTerms[i]);
                    ranExtrees[i].ScoreSqrt(reference, xmin, xmax, xprec);
                }

                stepN++;

                if (stepN > (int)(baseN * (Math.Pow(powN, (double)(nterms + 1)) - 1) / (powN - 1.0)))
                {
                    nterms++;
                    ranExtrees = new List<extree>();
                    ranTerms = new List<List<double>>();
                    descTerms = new List<List<double>>();
                    descExtrees = new List<extree>();
                    
                    for (int i = 0; i < nran; i++)
                    {
                        ranTerms.Add(RandTermBuilder());
                        ranExtrees.Add(BuildP(ranTerms[i]));
                        ranExtrees[i].ScoreSqrt(reference, xmin, xmax, xprec);
                    }

                    for (int i = 0; i < ndesc; i++)
                    {
                        descTerms.Add(RandTermBuilder());
                        descExtrees.Add(BuildP(descTerms[i]));
                        descExtrees[i].ScoreSqrt(reference, xmin, xmax, xprec);
                    }
                }
            }

            return bestExpr;
        }

        public extree BuildP(List<double> terms)
        {
            if (terms == null || terms.Count == 0)
            {
                return (new extree(expr_type.constant, 0.0));
            }
            else if (terms.Count == 1)
            {
                return (new extree(expr_type.constant, terms[0]));
            }
            else
            {
                extree builder = new extree(expr_type.plus, 0.0);
                extree temp2;

                builder.left = new extree(expr_type.constant, terms[0]);
                temp2 = builder;

                for (int i = 1; i < terms.Count - 1; i++)
                {
                    temp2.right = new extree(expr_type.plus, 0.0);
                    temp2 = temp2.right;
                    temp2.left = BuildPTerm(i, terms[i]);
                }

                temp2.right = BuildPTerm(terms.Count - 1, terms[terms.Count - 1]);

                return builder;
            }
        }

        public extree BuildPTerm(int n, double a)
        {
            if (n < 1)
                throw new ArgumentException("BuildPTerm argerr");

            if (n == 1)
            {
                extree tmul = new extree(expr_type.times, 0.0);
                tmul.left = new extree(expr_type.constant, double.NaN);
                tmul.right = new extree(expr_type.constant, a);
                return tmul;
            }
            else
            {
                extree tpow = new extree(expr_type.times, 0.0);
                tpow.right = new extree(expr_type.constant, a);
                tpow.left = new extree(expr_type.pow, 0.0);
                tpow.left.left = new extree(expr_type.constant, double.NaN);
                tpow.left.right = new extree(expr_type.constant, (double)n);
                return tpow;
            }
        }

        public List<double> RandTermBuilder()
        {
            return(RandTermBuilder(sMin, sMax));
        }

        public List<double> RandTermBuilder(double min, double max)
        {
            List<double> temp = new List<double>();

            for (int i = 0; i < nterms; i++)
            {
                // from a - b in doubles
                // [0, 1] * (b - a) = [0, b - a]
                // [0, b - a] + a = [a, b]
                temp.Add(rnm.NextDouble() * (max - min) + min);
            }

            return temp;
        }
    }
}
