using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class ExprNewtonMethod : ExprEvolver
    {
        public double xdif;
        public double xch;
        public int degree;
        public extree current;
        public List<double> currentVal;
        public extree best;
        public extree update;
        public int lastIR;
        public Random rnm;
        public int stepR;
        public bool pubmsg;
        public double reset_best_score;

        public ExprNewtonMethod(extree refxp, double start, double end, double prec, double term, int steps, int degree, double xdif, double xch, int stepR)
        {
            lastIR = 0;
            reference = refxp;
            xmin = start;
            xmax = end;
            xprec = prec;
            this.stepR = stepR;
            this.xdif = xdif;
            this.xch = xch;
            this.degree = degree;
            terminal_score = term;
            current_step = 1;

            reset_best_score = double.PositiveInfinity;
            rnm = new Random();
            
            currentVal = new List<double>();

            for (int i = 0; i <= degree; i++)
            {
                currentVal.Add(0.0);
            }

            current = GetExFromVals(currentVal);
        }

        public extree GetExFromVals(List<double> vals)
        {
            extree temp = new extree(expr_type.constant, double.NaN);
            extree sumx;
            extree tempW;
            extree tempN;

            if (vals.Count == 0)
            {
                return temp;
            }
            else if (vals.Count == 1)
            {
                tempW = new extree(expr_type.constant, vals[0]);

                
                return tempW;
            }
            else
            {
                tempW = new extree(expr_type.plus, double.NaN);
                sumx = tempW;
                tempW.left = new extree(expr_type.constant, vals[0]);

                for(int i = 1; i < vals.Count; i++)
                {
                    tempW.right = new extree(expr_type.plus, double.NaN);
                    tempW = tempW.right;

                    tempN = new extree(expr_type.times, double.NaN);
                    tempN.left = new extree(expr_type.constant, vals[i]);
                    tempN.right = new extree(expr_type.pow, double.NaN);
                    tempN.right.left = new extree(expr_type.constant, double.NaN);
                    tempN.right.right = new extree(expr_type.constant, (double) i);

                    tempW.left = tempN;
                }

                tempW.right = new extree(expr_type.constant, 0.0);

                return sumx;
            }
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            pubmsg = msg;

            for (int i = 0; i < n; i++)
            {
                Step();
                stepN++;
            }

            return (current);
        }

        public override void Step()
        {
            if (stepN - lastIR > stepR)
            {
                lastIR = stepN;
                for (int i = 0; i < currentVal.Count; i++)
                {
                    currentVal[i] = rnm.NextDouble() * 1.3 - 0.3;
                }

                current = GetExFromVals(currentVal);
                reset_best_score = double.PositiveInfinity;
            }

            double[] D = new double[degree + 1];
            List<double> newvals = new List<double>(currentVal);
            extree exnew;
            double ps;
            double ms;

            for (int i = 0; i < currentVal.Count; i++)
            {
                newvals = new List<double>(currentVal);
                newvals[i] = currentVal[i] + xdif;

                exnew = GetExFromVals(newvals);

                current.ScoreSqrt(reference, xmin, xmax, xprec);
                exnew.ScoreSqrt(reference, xmin, xmax, xprec);

                ps = current.true_score - exnew.true_score;

                if (ps > 1000)
                    ps = 1000;

                if (ps < -1000)
                    ps = -1000;

                newvals = new List<double>(currentVal);
                newvals[i] = currentVal[i] - xdif;

                exnew = GetExFromVals(newvals);
                exnew.ScoreSqrt(reference, xmin, xmax, xprec);

                ms = exnew.true_score - current.true_score;

                if (ms > 1000)
                    ms = 1000;

                if (ms < -1000)
                    ms = -1000;

                D[i] = (ps + ms) / 2;
            }

            double td = 0.0;
            double divisor;

            for (int i = 0; i < D.Length; i++)
            {
                td += D[i] * D[i];
            }

            td = Math.Sqrt(td);

            divisor = td / xch;

            for (int i = 0; i < D.Length; i++)
            {
                D[i] = D[i] / divisor;
            }

            newvals = new List<double>(currentVal);

            for (int i = 0; i < D.Length; i++)
            {
                newvals[i] += D[i];
            }

            currentVal = newvals;
            current = GetExFromVals(currentVal);

            current.ScoreSqrt(reference, xmin, xmax, xprec);

            if (current.true_score < best_current_score)
            {
                lastIR = stepN;
                
                if(pubmsg)
                {
                    Console.WriteLine("Best at Step " + stepN + ": " + current.true_score);
                    Console.WriteLine(current.ToString());
                }

                best_current_score = current.true_score;
                reset_best_score = current.true_score;
                best = current;
            }
            else if (current.true_score < reset_best_score)
            {
                reset_best_score = current.true_score;
                lastIR = stepN;
            }
        }
    }
}
