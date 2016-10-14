using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Evol;

namespace TestSystem.Evol
{
    public class BinaryEvalTranslatorLib
    {
        public BinaryEvalTranslatorLib()
        {

        }

        public double Eval(expr_type ty, double d1, double d2)
        {
            switch (ty)
            {
                case expr_type.divides:
                    return d1 / d2;
                case expr_type.minus:
                    return d1 - d2;
                case expr_type.times:
                    return d1 * d2;
                case expr_type.plus:
                    return d1 + d2;
                case expr_type.pow:
                    return Math.Pow(d1,d2);
                case expr_type.root:
                    return Math.Pow(d1, 1.0 / d2);
                case expr_type.mean:
                    return (d1 + d2) / 2.0;
                case expr_type.gmean:
                    return Math.Sqrt(d1 * d2);
                default:
                    return double.MaxValue;
            }
        }
    }
}
