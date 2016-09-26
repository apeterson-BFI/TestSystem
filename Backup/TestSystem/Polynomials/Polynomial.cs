using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Polynomials
{
    public class Polynomial
    {
        private List<int> coefficients;

        public Polynomial()
        {
            coefficients = new List<int>();
        }

        public Polynomial(List<int> coefficients)
        {
            this.coefficients = coefficients;
        }

        public Polynomial(int coeff, int degree)
        {
            this.coefficients = new List<int>();

            for (int i = 0; i < degree; i++)
            {
                this.coefficients.Add(0);
            }

            this.coefficients.Add(coeff);
        }

        public Polynomial negate()
        {
            Polynomial res = new Polynomial();

            List<int> c = res.coefficients;

            for (int i = 0; i < coefficients.Count; i++)
            {
                c.Add(-coefficients[i]);
            }

            return res;
        }

        public Polynomial add(Polynomial other)
        {
            Polynomial res = new Polynomial();

            List<int> l = this.coefficients;
            List<int> r = other.coefficients;

            List<int> c = res.coefficients;

            int lmax = l.Count - 1;
            int rmax = r.Count - 1;
            int totmax = Math.Max(lmax, rmax);

            for (int i = 0; i <= totmax; i++)
            {
                if (lmax < i)
                {
                    c.Add(r[i]);
                }
                else if (rmax < i)
                {
                    c.Add(l[i]);
                }
                else
                {
                    c.Add(l[i] + r[i]);
                }
            }

            return res;
        }

        public Polynomial multiply(Polynomial other)
        {
            Polynomial res = new Polynomial();

            List<int> l = this.coefficients;
            List<int> r = other.coefficients;

            List<int> c = res.coefficients;
            
            int lDegree = l.Count - 1;
            int rDegree = r.Count - 1;
            int totalDegree = lDegree + rDegree;

            int accum = 0;

            for (int d = 0; d <= totalDegree; d++)
            {
                accum = 0;

                for (int i = 0; i <= lDegree; i++)
                {
                    if (d - i < 0 || d - i > rDegree)
                    {
                        continue;
                    }

                    accum += l[i] * r[d - i];
                }

                c.Add(accum);
            }

            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            bool p = obj is Polynomial;

            if (!p)
            {
                return false;
            }

            Polynomial other = (Polynomial)obj;

            List<int> l = coefficients;
            List<int> r = other.coefficients;

            if (l == null && r == null)
            {
                return true;
            }
            else if (l == null || r == null)
            {
                return false;
            }

            if (l.Count != r.Count)
            {
                return false;
            }

            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] != r[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Polynomial pInvert()
        {
            Polynomial p1 = new Polynomial(1, 0);
            Polynomial p2 = this.negate();

            Polynomial res = p1.add(p2);

            return res;
        }

        public int getDegree()
        {
            return coefficients.Count - 1;
        }

        public override string ToString()
        {
            string accum = "";

            int m = 0;

            int c;

            for (int i = 0; i < coefficients.Count; i++)
            {
                c = coefficients[i];

                if (c == 0)
                {
                    continue;
                }

                if (m != 0)
                {
                    if (c > 0)
                    {
                        accum += " + ";
                    }
                    else
                    {
                        accum += " - ";
                        c = -c;
                    }
                }

                if (i == 0)
                {
                    accum += c.ToString();
                }
                else
                {
                    if (c == 1)
                    {
                        accum += "x^" + i.ToString();
                    }
                    else
                    {
                        accum += c.ToString() + " x^" + i.ToString();
                    }
                }

                m++;
            }

            return accum;
        }
    }
}
