using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Evol;

namespace TestSystem.Evol
{
    public class ExprGenlP : ExprEvolver
    {
        public extree bestExpr;
        public int maxD;
        public int minD;
        public double maxCoef;
        public double minCoef;
        public double minPow;
        public double maxPow;
        public static Random rnm = new Random();
        public List<List<double>> powCol;
        public List<List<double>> coefCol;
        public List<double> sc;
        public static double Hb = 1.0;

        public ExprGenlP(extree reference, double xmin, double xmax, double xprec, double term, int maxD, int minD, double maxCoef, double minCoef, double minPow, double maxPow)
        {
            this.reference = reference;
            this.xmin = xmin;
            this.xmax = xmax;
            this.xprec = xprec;
            this.terminal_score = term;
            this.stepN = 1;
            this.maxCoef = maxCoef;
            this.minCoef = minCoef;
            bestExpr = null;
            best_current_score = double.MaxValue;
            this.maxD = maxD;
            this.minD = minD;
            this.maxPow = maxPow;
            this.minPow = minPow;
            this.powCol = new List<List<double>>();
            this.coefCol = new List<List<double>>();
            this.sc = new List<double>();
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            extree temp;

            while (stepN < n && best_current_score > terminal_score)
            {
                temp = GetRandGenlP();

                if (temp.true_score < best_current_score)
                {
                    best_current_score = temp.true_score;
                    bestExpr = temp;

                    if (msg)
                    {
                        Console.Out.WriteLine("GenlP Evol At Step #" + stepN + ":");
                        Console.Out.WriteLine("Best Expression: " + bestExpr.ToString());
                        Console.Out.WriteLine("With Error Score: " + best_current_score);
                    }
                }

                // weighted step
                List<double> cs = new List<double>();
                List<double> ps = new List<double>();
                double ca = 0.0;
                double pa = 0.0;
                double sa = 0.0;

                for (int i = 0; i < maxD; i++)
                {
                    ca = 0.0;
                    pa = 0.0;
                    sa = 0.0;

                    for (int j = 0; j < coefCol.Count; j++)
                    {
                        if (coefCol[j].Count < i)
                        {
                            ca += coefCol[j][i] / sc[j];
                            pa += powCol[j][i] / sc[j];
                            sa += sc[j];
                        }
                    }

                    cs.Add(ca * sa / coefCol.Count / coefCol.Count + (rnm.NextDouble() - 0.5) * 2.0 * Hb);
                    ps.Add(pa * sa / coefCol.Count / coefCol.Count + (rnm.NextDouble() - 0.5) * 2.0 * Hb);
                }

                extree t2 = ConvertToExtree(ps, cs);
                t2.ScoreSqrt(reference, xmin, xmax, xprec);

                if (t2.true_score < best_current_score)
                {
                    best_current_score = t2.true_score;
                    bestExpr = t2;
                    coefCol.Add(cs);
                    powCol.Add(ps);
                    sc.Add(best_current_score);

                    if (msg)
                    {
                        Console.Out.WriteLine("GenlP Evol At Step #" + stepN + ":");
                        Console.Out.WriteLine("Best Expression: " + bestExpr.ToString());
                        Console.Out.WriteLine("With Error Score: " + best_current_score);
                    }
                }

                stepN++;

                if (powCol.Count > 100)
                {
                    List<double> ttp = powCol[100];
                    List<double> ttc = coefCol[100];
                    double tts = sc[100];

                    powCol = new List<List<double>>();
                    coefCol = new List<List<double>>();
                    sc = new List<double>();
                    powCol.Add(ttp);
                    coefCol.Add(ttc);
                    sc.Add(tts);
                }
            }

            return bestExpr;
        }

        public extree GetRandGenlP()
        {
            return(GetRandGenlP(maxCoef, minCoef, maxPow, minPow, maxD, minD));
        }

        public extree GetRandGenlP(double maxC, double minC, double maxP, double minP, int mxD, int mnD)
        {
            int d;

            if (mxD == mnD)
                d = mxD;
            else
                d = rnm.Next(mxD - mnD + 1) + mnD;

            List<double> pows = new List<double>();
            List<double> coeffs = new List<double>();

            for (int i = 0; i < d; i++)
            {
                coeffs.Add(rnm.NextDouble() * (maxC - minC) + minC);
                pows.Add(rnm.NextDouble() * (maxP - minP) + minP);
            }

            powCol.Add(pows);
            coefCol.Add(coeffs);

            extree temp = ConvertToExtree(pows, coeffs);
            temp.ScoreSqrt(reference, xmin, xmax, xprec);
            sc.Add(temp.true_score);

            return (temp);
        }

        public extree ConvertToExtree(List<double> pow, List<double> coeff)
        {
            extree builder = new extree(expr_type.plus, 0.0);
            extree temp2;

            builder.left = BuildPTerm(pow[0], coeff[0]); 
            temp2 = builder;

            for (int i = 1; i < pow.Count - 1; i++)
            {
                temp2.right = new extree(expr_type.plus, 0.0);
                temp2 = temp2.right;
                temp2.left = BuildPTerm(pow[i], coeff[i]);
            }

            temp2.right = BuildPTerm(pow[pow.Count - 1], coeff[coeff.Count - 1]);

            return builder;
        }

        public extree BuildPTerm(double n, double a)
        {
            extree tpow = new extree(expr_type.times, 0.0);
            tpow.right = new extree(expr_type.constant, a);
            tpow.left = new extree(expr_type.pow, 0.0);
            tpow.left.left = new extree(expr_type.constant, double.NaN);
            tpow.left.right = new extree(expr_type.constant, n);
            return tpow;
        }
    }
}
