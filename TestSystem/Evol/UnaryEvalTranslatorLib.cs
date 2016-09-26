using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class UnaryEvalTranslatorLib
    {
        public Dictionary<expr_type, Func<double, double>> delgLookup;

        public UnaryEvalTranslatorLib()
        {
            delgLookup = new Dictionary<expr_type, Func<double, double>>();

            delgLookup.Add(expr_type.constant, (d => d));
            delgLookup.Add(expr_type.exp, Math.Exp);
            delgLookup.Add(expr_type.ln, Math.Log);
            delgLookup.Add(expr_type.neg, (d => -d));
            delgLookup.Add(expr_type.recip, (d => 1.0 / d));
            delgLookup.Add(expr_type.cos, Math.Cos);
            delgLookup.Add(expr_type.sin, Math.Sin);
            delgLookup.Add(expr_type.tan, Math.Tan);
            delgLookup.Add(expr_type.asin, Math.Asin);
            delgLookup.Add(expr_type.acos, Math.Acos);
            delgLookup.Add(expr_type.atan, Math.Atan);
            delgLookup.Add(expr_type.sqrt, Math.Sqrt);
            delgLookup.Add(expr_type.abs, Math.Abs);
            delgLookup.Add(expr_type.cosh, Math.Cosh);
            delgLookup.Add(expr_type.sinh, Math.Sinh);
            delgLookup.Add(expr_type.tanh, Math.Tanh);
            delgLookup.Add(expr_type.powinvsqrt2, (d => Math.Pow(d, Math.Sqrt(2.0) / 2.0)));
            delgLookup.Add(expr_type.powsqrt2, (d => Math.Pow(d, Math.Sqrt(2.0))));
        }

        public Func<double, double> GetFunction(expr_type ty)
        {
            if (!delgLookup.ContainsKey(ty))
                return null;
            else
                return delgLookup[ty];
        }

        public bool Exists(expr_type ty)
        {
            return (delgLookup.ContainsKey(ty));
        }

        public double Eval(expr_type ty, double val)
        {
            return(GetFunction(ty)(val));
        }
    }
}
