using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class DWXT
    {
        int a;
        int b;

        public DWXT(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public void Clash(Dictionary<int, int> aplace, Dictionary<int, int> bplace)
        {
            int suma = 0;
            int sumb = 0;

            foreach (KeyValuePair<int, int> kvp in aplace)
            {
                suma += kvp.Value;
            }

            foreach (KeyValuePair<int, int> kvp in bplace)
            {
                sumb += kvp.Value;
            }

            if (a < suma)
                a = suma;
            else if (a > suma)
                throw new ArgumentException("Invalid aplace: excess");

            if (b < sumb)
                b = sumb;
            else if (b > sumb)
                throw new ArgumentException("Invalid bplace: excess");

            int al = 0;
            int bl = 0;

            foreach (KeyValuePair<int, int> kvp in aplace)
            {
                if (!bplace.ContainsKey(kvp.Key))
                {
                    double loss = (Math.Log((double)kvp.Value, 2.0) * 0.05) * (double)(Math.Max((kvp.Value - 0 * 10), 0));

                    int lossn = (int)Math.Ceiling(loss);
                    al += lossn;

                    al += kvp.Value / 20;
                }
                else
                {
                    int bv = bplace[kvp.Key];

                    if (kvp.Value > bv * 10)
                    {
                        double loss = (Math.Log((double)kvp.Value, 2.0) * 0.05) * (double)(Math.Max((kvp.Value - bv * 10), 0));
                        int lossn = (int)Math.Floor(loss);
                        al += lossn;
                    }
                    else if (bv > kvp.Value * 10)
                    {
                        double loss = (Math.Log((double)bv, 2.0) * 0.05) * (double)(Math.Max((bv - kvp.Value * 10), 0));
                        int lossn = (int)Math.Floor(loss);
                        bl += lossn;
                    }

                    al += kvp.Value / 20;
                    bl += bv / 20;

                    al += bv / 6;
                    bl += kvp.Value / 6;
                }
            }

            foreach (KeyValuePair<int, int> kvp in bplace)
            {
                if (!aplace.ContainsKey(kvp.Key))
                {
                    double loss = (Math.Log((double)kvp.Value, 2.0) * 0.05) * (double)(Math.Max((kvp.Value - 0 * 10), 0));

                    int lossn = (int)Math.Ceiling(loss);
                    bl += lossn;

                    bl += kvp.Value / 20;
                }
            }

            a -= al;
            b -= bl;
        }
    }
}
