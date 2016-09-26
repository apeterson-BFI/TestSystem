using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class DelgSolver
    {
        public RealFunction fx;
        public const double iota = 0.0001;

        public DelgSolver(RealFunction fx)
        {
            this.fx = fx;
        }

        public double FindZero()
        {
            return (NewtonMethod(0.0, 0.1));
        }

        public double NewtonMethod(double ini, double tol)
        {
            double slope;
            double x = ini;
            double y = fx(x);

            while (y > tol)
            {
                slope = D(fx, x, iota);
                x = x - y / slope;
                y = fx(x);
            }

            return x;
        }

        public static double D(RealFunction fx, double x, double io)
        {
            return (fx(x + io / 2.0) - fx(x - io / 2.0) / io);
        }

        public double FindEqual(double rhValue)
        {
            RealFunction rf = (x => fx(x) - rhValue);

            DelgSolver dg = new DelgSolver(rf);

            return (FindZero());
        }
    }

    public delegate double RealFunction(double x);
}
