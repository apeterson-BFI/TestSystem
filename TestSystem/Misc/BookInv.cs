using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Misc
{
    public class BookInv
    {
        double c;
        double s;
        double e;
        double r;
        double v;
        double rr;
        double rv;
        static Random rnm = new Random();
        static int[][] p_out = new int[][] { new int[] { 1000, 5 }, new int[] { 1000, 950, 100, 50 }, new int[] { 1000, 800, 400, 200 }, new int[] { 1000, 950, 900 } };
        static BookInv[][] r_out = new BookInv[][] {
                new BookInv[] {new BookInv(0.16, 0.0, 0.0, 0.0, 0.0), new BookInv(0.05, -0.10, 0.0, 0.0, 0.0)}, 
                new BookInv[] {new BookInv(0.3, 1.0, -1.0, 0.0, 0.0), new BookInv(0.1, 0.0, 0.2, 0.0, 0.0), 
                               new BookInv(0.1, 0.0, -0.3, 0.0, 0.0), new BookInv(0.0, 0, -1.0, 0.4, 0.0)},
                new BookInv[] {new BookInv(0.0, 0.0, 1.75, -1.0, 0.0), new BookInv(0.0, 0.0, 0.0, 0.75, 0.0), new BookInv(0.0, 0.0, 0.0, -1.0, 0.2),
                               new BookInv(0.0, 0.0, 0.0, -1.0, 0.0)},
                new BookInv[] {new BookInv(0.0, 0.0, 0.0, 22.0, -1.0), new BookInv(0.0, 0.0, 0.0, 0.0, 21.0), new BookInv(0.0, 0.0, 0.0, 0.0, -1.0)}};

        public BookInv(double c, double s, double e, double r, double v)
        {
            this.c = c;
            this.s = s;
            this.e = e;
            this.r = r;
            this.v = v;
        }

        public BookInv(double c, double s, double e, double r, double rr, double v, double rv)
        {
            this.c = c;
            this.s = s;
            this.e = e;
            this.r = r;
            this.v = v;
            this.rr = rr;
            this.rv = rv;
        }

        public void Process()
        {
            if (s != 0)
                ProcessTransformSet(r_out[0], p_out[0], s);
            if (e != 0)
                ProcessTransformSet(r_out[1], p_out[1], e);
            if (r != 0)
                ProcessTransformSet(r_out[2], p_out[2], r);
            if (v != 0)
                ProcessTransformSet(r_out[3], p_out[3], v);

            if (rr <= c)
            {
                c -= rr;
                r += rr;
            }

            if (rv <= c)
            {
                c -= rv;
                v += rv;
            }
        }

        public double Total()
        {
            return (c + s + e + r + v);
        }

        public void ProcessTransformSet(BookInv[] tset, int[] pset, double amt)
        {
            int rl = rnm.Next(1, 1001);  // 1 - 1000
            int index = pset.Length - 1;

            while (rl > pset[index])
            {
                index--;
            }

            BookInv temp = tset[index];

            c += temp.c * amt;
            s += temp.s * amt;
            e += temp.e * amt;
            r += temp.r * amt;
            v += temp.v * amt;
        }
    }
}
