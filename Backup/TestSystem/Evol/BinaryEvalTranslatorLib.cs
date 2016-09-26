using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Evol;

namespace TestSystem.Evol
{
    public class BinaryEvalTranslatorLib
    {
        public Dictionary<expr_type, Func<double, double, double>> delgLookup;

        public BinaryEvalTranslatorLib()
        {
            delgLookup = new Dictionary<expr_type, Func<double, double, double>>();

            delgLookup.Add(expr_type.divides, ((d1, d2) => d1 / d2));
            delgLookup.Add(expr_type.minus, ((d1, d2) => d1 - d2));
            delgLookup.Add(expr_type.times, ((d1, d2) => d1 * d2));
            delgLookup.Add(expr_type.plus, ((d1, d2) => d1 + d2));
            delgLookup.Add(expr_type.pow, Math.Pow);
            delgLookup.Add(expr_type.root, ((d1, d2) => Math.Pow(d1, 1.0 / d2)));
        }

        public Func<double, double, double> GetFunction(expr_type ty)
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

        public double Eval(expr_type ty, double d1, double d2)
        {
            if (ty == expr_type.compose)
                throw new ArgumentException("Can not eval on compose");

            return (GetFunction(ty)(d1, d2));
        }
    }
}
