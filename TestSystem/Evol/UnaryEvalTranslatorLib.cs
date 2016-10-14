using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class UnaryEvalTranslatorLib
    {
        private double sqrt2;
        private double invsqrt2;

        public UnaryEvalTranslatorLib()
        {
            sqrt2 = Math.Sqrt(2.0);
            invsqrt2 = Math.Sqrt(2.0) / 2.0;
        }

        public double Eval(expr_type ty, double val)
        {
            switch (ty)
            {
                case expr_type.constant:
                    return val;
                case expr_type.exp:
                    return Math.Exp(val);
                case expr_type.ln:
                    return Math.Log(val);
                case expr_type.neg:
                    return -val;
                case expr_type.recip:
                    return 1.0 / val;
                case expr_type.cos:
                    return Math.Cos(val);
                case expr_type.sin:
                    return Math.Sin(val);
                case expr_type.tan:
                    return Math.Tan(val);
                case expr_type.asin:
                    return Math.Asin(val);
                case expr_type.acos:
                    return Math.Acos(val);
                case expr_type.atan:
                    return Math.Atan(val);
                case expr_type.sqrt:
                    return Math.Sqrt(val);
                case expr_type.abs:
                    return Math.Abs(val);
                case expr_type.cosh:
                    return Math.Cosh(val);
                case expr_type.sinh:
                    return Math.Sinh(val);
                case expr_type.tanh:
                    return Math.Tanh(val);
                case expr_type.powinvsqrt2:
                    return Math.Pow(val, invsqrt2);
                case expr_type.powsqrt2:
                    return Math.Pow(val, sqrt2);
                default:
                    return double.MaxValue;
            }
        }
    }
}
