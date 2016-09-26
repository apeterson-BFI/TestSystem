using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class extree
    {
        public extree left;
        public extree right;
        public expr_type type;
        public double val;
        public bool unary;
        public double true_score;
        public static int num_math_types = Enum.GetValues(typeof(expr_type)).Length;
        public static UnaryEvalTranslatorLib uEvalLib = new UnaryEvalTranslatorLib();
        public static BinaryEvalTranslatorLib bEvalLib = new BinaryEvalTranslatorLib();
        private static Random rnm = new Random();

        public extree()
        {

        }

        public extree(expr_type ty, double v)
        {
            true_score = double.PositiveInfinity;
            type = ty;
            val = v;
            unary = IsUnary(ty);
        }

        // Return pure +/-, set abs dif
        public void ScoreSqrt(extree sqref, double x_start, double x_end, double x_prec)
        {
            true_score = 0;
            double x = x_start;
            int v = (int)(Math.Round((x_end - x_start) / x_prec, 0));
            double ero;

            try
            {
                for (int i = 0; i <= v; i++)
                {
                    ero = (eval(eval(x)) - sqref.eval(x));
                    true_score += ero * ero;
                    x += x_prec;
                }
            }
            catch (ArithmeticException)
            {
                true_score = double.PositiveInfinity;
            }

            if (double.IsNaN(true_score))
                true_score = double.PositiveInfinity;
        }

        public double eval(double x)
        {
            if (unary)
            {
                if(!uEvalLib.Exists(type))
                {
                    throw new ArgumentException("Invalid Operation (Unary)");
                }

                if (double.IsNaN(val))
                {
                    return (uEvalLib.Eval(type, x));
                }
                else
                {
                    return(uEvalLib.Eval(type, val));
                }
            }
            else
            {
                if (left == null || right == null)
                {
                    throw new ArgumentException("Binary Operator has one or more null parameters");
                }

                if(type == expr_type.compose)
                {
                    return(left.eval(right.eval(x)));
                }
                else if (!bEvalLib.Exists(type))
                {
                    throw new ArgumentException("Invalid Operation");
                }
                else
                {
                    return (bEvalLib.Eval(type, left.eval(x), right.eval(x)));
                }
            }
        }

        public static bool IsUnary(expr_type type)
        {
            return !(type == expr_type.plus || type == expr_type.minus || type == expr_type.times || type == expr_type.pow ||
                type == expr_type.root || type == expr_type.divides || type == expr_type.compose);
        }

        public static int CompareScores(extree t1, extree t2)
        {
            if (t1.true_score == double.NaN)
                return 1;

            if (t2.true_score == double.NaN)
                return -1;
            
            if (t1.true_score == double.PositiveInfinity)
            {
                if (t2.true_score == double.PositiveInfinity)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (t2.true_score == double.PositiveInfinity)
            {
                return -1;
            }
            else
            {
                return (Math.Sign(t1.true_score - t2.true_score));
            }
        }

        public override string ToString()
        {
            string temp = "";

            if (IsUnary(type))
            {
                temp = GetUnarySignature(temp);
            }
            else
            {
                temp = GetBinarySignature(temp);
            }

            return temp;
        }
        
        public string GetBinarySignature(string temp)
        {
            switch (type)
            {
                case expr_type.compose:
                    temp += "°";
                    break;
                case expr_type.divides:
                    temp += "/";
                    break;
                case expr_type.minus:
                    temp += "-";
                    break;
                case expr_type.plus:
                    temp += "+";
                    break;
                case expr_type.pow:
                    temp += "^";
                    break;
                case expr_type.times:
                    temp += "*";
                    break;
                case expr_type.root:
                    temp += "root";
                    break;
            }

            temp = "(" + left.ToString() + " " + temp + " " + right.ToString() + ")";
            return temp;
        }

        public string GetUnarySignature(string temp)
        {
            switch (type)
            {   
                // specify special cases
                case expr_type.neg:
                    temp += "- ";
                    break;
                case expr_type.recip:
                    temp += "1 / ";
                    break;
                case expr_type.constant:
                    break;  // constant adds nothing. It is a placeholder for a simple value
                default:
                    temp += Enum.GetName(typeof(expr_type), type) + " ";
                    break;
            }

            if (double.IsNaN(val))
                temp += "x ";
            else
                temp += String.Format("{0:0.0000}", val) + " ";
            return temp;
        }

        public static extree CreateExtreeFromBinaryLayout(BinaryLayout bl)
        {
            // plus is the first binary oper_type
            // ln is the first unary oper_type
            // -1 is the first value
            extree et;

            if (bl.IsBinary)
            {
                et = new extree(expr_type.plus, 0.0);
                et.left = CreateExtreeFromBinaryLayout(bl.Left);
                et.right = CreateExtreeFromBinaryLayout(bl.Right);
            }
            else
            {
                et = new extree(expr_type.ln, -1.0);
            }

            return et;
        }

        public extree CopyExtree()
        {
            extree e = new extree();
            e.type = type;
            e.val = val;
            e.unary = IsUnary(type);
            e.left = (left != null ? left.CopyExtree() : null);
            e.right = (right != null ? right.CopyExtree() : null);

            return e;
        }

        public extree NextEnumerableValue(BinaryLayout bl)
        {
            extree e = CopyExtree();

            if (IsUnary(e.type))
            {
                if (IsLastUnaryType(e.type)) // carry
                {
                    e.type = expr_type.ln;

                    if (double.IsNaN(e.val))
                    {
                        return null;    // last val
                    }
                    else if (e.val >= 5.0)
                    {
                        e.val = double.NaN;
                    }
                    else
                    {
                        e.val += 1.0;
                    }
                }
                else
                {
                    int tn = (int)(e.type);
                    expr_type ty;

                    for (int i = tn + 1; i < num_math_types; i++)
                    {
                        ty = ((expr_type)(i));

                        if (IsUnary(ty))
                        {
                            e.type = ty;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (IsLastBinaryType(e.type))
                {
                    e.type = expr_type.plus;
                    e.left = e.left.NextEnumerableValue(bl.Left);

                    if (e.left == null)  // last on the left
                    {
                        e.left = CreateExtreeFromBinaryLayout(bl.Left);
                        e.right = e.right.NextEnumerableValue(bl.Right);

                        extree temp_er = e.right.NextEnumerableValue(bl.Right);

                        if (temp_er == null)
                        {
                            return null;    // last on both
                        }
                        else
                        {
                            e.right = temp_er;
                        }
                    }
                }
                else
                {
                    int tn = (int)(e.type);
                    expr_type ty;

                    for (int i = tn + 1; i < num_math_types; i++)
                    {
                        ty = ((expr_type)(i));

                        if (!IsUnary(ty))
                        {
                            e.type = ty;
                            break;
                        }
                    }
                }
            }

            return e;
        }

        public bool IsLastEnumerableValueTree()
        {
            // ceil is the last unary
            // 5 is the last value
            // mod is the last binary

            if (IsUnary(type))
            {
                return (IsLastUnaryType(type) && IsLastValueType(val));
            }
            else
            {
                return (IsLastBinaryType(type) && left.IsLastEnumerableValueTree() && right.IsLastEnumerableValueTree());
            }
        }

        public extree Simplify()
        {
            extree s = CopyExtree();

            if (IsUnary(s.type))
            {
                if (double.IsNaN(s.val)) // unary(x)
                {
                    return s;           // nothing can be done localling with f(x), only in grouping with binaries
                }
                else                     // unary(a) = b
                {
                    return (new extree(expr_type.constant, uEvalLib.Eval(s.type, s.val)));  // resolve the calculation
                }
            }
            else // binary
            {
                s.left = s.left.Simplify();
                s.right = s.right.Simplify();

                if (s.left.type == expr_type.constant && s.right.type == expr_type.constant && !double.IsNaN(s.left.val) && !double.IsNaN(s.right.val))  // f(a,b) -> c
                {
                    // no x so eval works, avoiding bEval bugs
                    return (new extree(expr_type.constant, eval(0)));
                }
                else if (s.type == expr_type.compose && (s.right.type == expr_type.constant && double.IsNaN(s.right.val))) // f(x) o x -> f(x)
                {
                    return (s.left.CopyExtree());
                }
                else if (s.type == expr_type.compose && (s.left.type == expr_type.constant && double.IsNaN(s.left.val))) // x o f(x) -> f(x)
                {
                    return (s.right.CopyExtree());
                }
                else if (s.type == expr_type.compose && (s.left.type == expr_type.constant && !double.IsNaN(s.left.val))) // a o f(x) -> a
                {
                    return (s.left.CopyExtree());
                }
                else if (s.type == expr_type.compose && (s.right.type == expr_type.constant && !double.IsNaN(s.right.val))) // f(x) o a -> f(a) -> b
                {
                    return (new extree(expr_type.constant, s.left.eval(s.right.val)));
                }
                else
                {
                    return (s);
                }
            }
        }

        public static bool IsLastUnaryType(expr_type ty)
        {
            if (!IsUnary(ty))
                throw new ArgumentException("This is not a unary operation type");
            else
            {
                return (ty == expr_type.powsqrt2);    
            }
        }

        public static bool IsLastBinaryType(expr_type ty)
        {
            if (IsUnary(ty))
                throw new ArgumentException("This is not a binary operation type");
            else
            {
                return (ty == expr_type.root);
            }
        }

        public static bool IsLastValueType(double val)
        {
            return (double.IsNaN(val));
        }

        public static extree RandomExtreeFromLayout(BinaryLayout b)
        {
            extree e;

            if (b.IsBinary)
            {
                e = new extree(extree.GetRandomBinary(), 0.0);
                e.left = RandomExtreeFromLayout(b.Left);
                e.right = RandomExtreeFromLayout(b.Right);
                return e;
            }
            else
            {
                int k = rnm.Next(6);
                double v;

                if(k == 0)
                    v = double.NaN;
                else
                {
                    v = rnm.NextDouble() * 5.0;
                }

                e = new extree(extree.GetRandomUnary(), v);

                return e;
            }
        }

        public static expr_type GetRandomUnary()
        {
            int uenum;

            while(! IsUnary((expr_type)(uenum = rnm.Next(num_math_types))))
            {
                ;
            }

            return((expr_type) uenum);
        }

        public static expr_type GetRandomBinary()
        {
            int benum;

            while(IsUnary((expr_type)(benum = rnm.Next(num_math_types))))
            {
                ;
            }

            return((expr_type) benum);
        }

        public bool Equals(extree t)
        {
            if (this.type != t.type)
                return false;

            if (this.unary && t.unary)
            {
                if (this.val == t.val)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (!this.unary && !t.unary)
            {
                return (this.left.Equals(t.left) && this.right.Equals(t.right));
            }
            else
            {
                return false;
            }
        }

        // numerical nth deriv to prec-level h
        // computes (D_n(x + h, n - 1, h) - D_n(x - h, n - 1, h)) / 2h. The case with n = 1 is terminal. The result is simply the eval in place of D_n
        public double D_n(double x, int n, double h)
        {
            if (n == 0)
            {
                return(eval(x));
            }
            else if (n == 1)
            {
                return ((eval(x + h) - eval(x - h)) / 2.0 / h);
            }
            else if (n > 1)
            {
                return ((D_n(x + h, n - 1, h) - D_n(x - h, n - 1, h)) / 2.0 / h);
            }
            else
            {
                throw new ArgumentException("bad arg");
            }
        }
    }

    public enum expr_type
    {
        plus,
        minus,
        times,
        divides,
        pow,
        ln,
        exp,
        sin,
        cos,
        tan,
        constant,
        neg,
        recip,
        asin,
        acos,
        atan,
        compose,
        sqrt,
        abs,
        cosh,
        sinh,
        tanh,
        root,
        powinvsqrt2,
        powsqrt2
    }
}
