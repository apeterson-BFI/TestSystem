using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    class Isum
    {
        static double[] SumRecurse(double[] poly)
        {
            double[] poly2 = SumExpand(poly);

            for (int i = 0; i < poly2.Length; i++)
            {
                Console.Out.WriteLine("x" + i + " = " + FriendlyFractionPrint(poly2[i]));
            }

            return poly2;
        }

        static string FriendlyFractionPrint(double val)
        {
            string temp = "";
            double tol = 0.0001;

            int capture = -1;
            double k1 = -1;
            double k2 = -1;

            for (int i = 1; i < 48; i++)
            {
                k1 = val * i;
                k2 = Math.Round(k1);

                if (Math.Abs(k2 - k1) < tol)
                {
                    capture = i;
                    break;
                }
            }

            if (capture == -1)
                return val.ToString();

            int num = Convert.ToInt32(k2);
            
            temp = (capture == 1 ? num.ToString() : num + "/" + capture);

            return temp;
        }

        static void LinearSolve(double[,] fmatrix)
        {
            MSolver ms = new MSolver(fmatrix);
            double[] s = ms.Solve();

            for (int i = 0; i < s.Length; i++)
            {
                Console.Out.WriteLine("x" + i + " = " + s[i]);
            }
        }

        static double[,] MakePowMatrix(int n)
        {
            double[,] temp = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j == 0)
                    {
                        temp[i, j] = 1.0;
                    }

                    temp[i, j] = Math.Pow((double)i, (double)j);
                }
            }

            return temp;
        }

        static double[] SumExpand(double[] polynomial)
        {
            double[,] temp = MakeSumExpansionMatrix(polynomial);

            MSolver ms = new MSolver(temp);
            double[] s = ms.Solve();

            return s;
        }

        static double[,] MakeSumExpansionMatrix(double[] polynomial)
        {
            int n = polynomial.Length + 1;

            double[,] temp = MakePowMatrix(n);

            double[] temp2 = PolySumSample(polynomial);

            for (int i = 0; i < n; i++)
            {
                temp[i, n] = temp2[i];
            }

            return temp;
        }

        static double[] PolySumSample(double[] polynomial)
        {
            double[] samples = new double[polynomial.Length + 1];

            double accum = 0.0;

            for (int i = 0; i < samples.Length; i++)
            {
                accum = 0.0;

                for (int j = 0; j <= i; j++)
                {
                    accum += PolyEval(polynomial, j);
                }

                samples[i] = accum;
            }

            return samples;
        }

        static double PolyEval(double[] polynomial, double x)
        {
            double accum = 0.0;

            for (int i = 0; i < polynomial.Length; i++)
            {
                if (i == 0)
                    accum += polynomial[0];
                else
                    accum += polynomial[i] * Math.Pow(x, i);
            }

            return accum;
        }
    }

    class MSolver
    {
        double[,] matrix;
        int x;
        int y;
        double[] solution;

        public MSolver(double[,] matrix)
        {
            // x matrix should be one bigger in dimension with last column holding equation rh constants

            if (matrix.GetLength(1) - matrix.GetLength(0) != 1)
                throw new ArgumentException("Bad Matrix");

            this.matrix = matrix;

            x = matrix.GetLength(1);
            y = matrix.GetLength(0);
        }

        public double[] Solve()
        {
            for (int i = 0; i < y; i++)
            {
                ZeroOutColumn(i);
            }

            solution = new double[y];

            for (int j = 0; j < y; j++)
            {
                solution[j] = matrix[j,x - 1];
            }

            return solution;
        }

        public void ZeroOutColumn(int c)
        {
            int row = c;

            while (matrix[row,c] == 0.0)
            {
                row++;
            }
            
            // Swap rows if a row other than the diagonal entry had the first non-zero
            if (row != c)
            {
                double tempf = 0.0;

                for (int i = 0; i < x; i++)
                {
                    tempf = matrix[c,i];
                    matrix[c,i] = matrix[row,i];
                    matrix[row,i] = tempf;
                }
            }

            // Now matrix element c, c contains a non-zero, so we can start working.

            double r = 0.0;
            double[] z;

            for (int i = 0; i < y; i++)
            {
                if (i == c)
                    continue;

                r = matrix[i,c] / matrix[c,c];
                z = new double[x];

                for (int ii = 0; ii < x; ii++)
                {
                    z[ii] = matrix[c, ii] * r;
                }

                for (int ii = 0; ii < x; ii++)
                {
                    matrix[i, ii] = matrix[i, ii] - z[ii];
                }
            }

            // Now that column is zeroed out, but we need to scale the triangular cell

            double scale = matrix[c, c];

            for (int ii = 0; ii < x; ii++)
            {
                matrix[c, ii] = matrix[c, ii] / scale;
            }

            if (matrix[c,c] != 1)
                throw new ArgumentException("Programming Bug: Scaling after Zeroing Failed");
        }
    }
}
