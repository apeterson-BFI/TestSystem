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
        public const double sizepenalty = 0.01; 

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

        public extree(extree other)
        {
            this.type = other.type;
            this.true_score = double.PositiveInfinity;
            this.val = other.val;
            this.unary = other.unary;

            this.left = (other.left == null ? null : new extree(other.left));
            this.right = (other.right == null ? null : new extree(other.right));
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

            true_score *= (1 + sizepenalty * (double)nodeCount());

            if (double.IsNaN(true_score))
                true_score = double.PositiveInfinity;
        }

        public extree simplify()
        {
            // can only simplify binary expressions or unary expressions of constants
            if (left == null || right == null)
            {
                if (double.IsNaN(val))
                {
                    return this;
                }
                // evaluate functions on constants
                // replace unary function on constant with just constant value
                else
                {
                    double res = eval(0.0);

                    extree e = new extree(expr_type.constant, res);
                    return e;
                }
            }
            // a binary operation of constants
            else if (left.unary && right.unary && !double.IsNaN(left.val) && !double.IsNaN(right.val))
            {
                double res = eval(0.0);

                extree e = new extree(expr_type.constant, res);
                return e;
            }
            // rule 3 - expr o constant -> eval expr : x -> constant
            else if (type == expr_type.compose && IsUnary(right.type) && !double.IsNaN(right.val))
            {
                extree ee = new extree(expr_type.constant, left.eval(right.eval(right.val)));
                return ee;
            }
            // rule 5 - constant o expr -> constant
            else if (type == expr_type.compose && IsUnary(left.type) && !double.IsNaN(left.val))
            {
                return left;
            }
            // rule 4 - expr o x -> expr
            else if (type == expr_type.compose && right.type == expr_type.constant && double.IsNaN(right.val))
            {
                return left;
            }
            // rule 6 - x o expr -> expr
            else if (type == expr_type.compose && left.type == expr_type.constant && double.IsNaN(left.val))
            {
                return right;
            }
            // rule 7 - expr - expr -> 0
            else if (type == expr_type.minus && left.Equals(right))
            {
                extree ee = new extree(expr_type.constant, 0.0);
                return ee;
            }
            // rule 8 - expr / expr -> 1
            else if (type == expr_type.divides && left.Equals(right))
            {
                extree ee = new extree(expr_type.constant, 1.0);
                return ee;
            }
            // rule: expr * 0 = 0
            else if (type == expr_type.times && (left.type == expr_type.constant && left.val == 0.0 || right.type == expr_type.constant && right.val == 0.0))
            {
                extree ee = new extree(expr_type.constant, 0.0);
                return ee;
            }
            // rule: expr + 0 = expr
            else if (type == expr_type.plus && (left.type == expr_type.constant && left.val == 0.0 || right.type == expr_type.constant && right.val == 0.0))
            {
                if (left.type == expr_type.constant && left.val == 0.0)
                {
                    return right;
                }
                else if (right.type == expr_type.constant && right.val == 0.0)
                {
                    return left;
                }
                else
                {
                    return recursiveSimplify();
                }
            }
            // expr * 1 = expr
            else if (type == expr_type.times && (left.type == expr_type.constant && left.val == 1.0 || right.type == expr_type.constant && right.val == 1.0))
            {
                if (left.type == expr_type.constant && left.val == 1.0)
                {
                    return right;
                }
                else if (right.type == expr_type.constant && right.val == 1.0)
                {
                    return left;
                }
                else
                {
                    return recursiveSimplify();
                }
            }
            // expr / 1 = expr
            else if (type == expr_type.divides && right.type == expr_type.constant && right.val == 1.0)
            {
                return left;
            }
            // abs is idempotent: i.e. Abs(Abs(x)) = Abs(x)
            else if (type == expr_type.compose && left.type == expr_type.abs && double.IsNaN(left.val) && right.type == expr_type.abs && double.IsNaN(right.val))
            {
                return left;
            }
            // -x o -x = x
            else if (type == expr_type.compose && left.type == expr_type.neg && double.IsNaN(left.val) && right.type == expr_type.neg && double.IsNaN(right.val))
            {
                extree x = new extree(expr_type.constant, double.NaN);
                return x;
            }
            // binary node with no substitutions found
            else
            {
                return recursiveSimplify();
            }
        }

        private extree recursiveSimplify()
        {
            extree leftSimp = left.simplify();
            extree rightSimp = right.simplify();

            extree simple = new extree();
            simple.type = type;
            simple.left = leftSimp;
            simple.right = rightSimp;
            simple.unary = false;
            simple.val = val;
            simple.true_score = true_score;

            return simple;
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
                    return double.NaN;
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

        public int nodeCount()
        {
            if (IsUnary(type))
            {
                return 1;
            }
            else if (left != null && right != null)
            {
                return 1 + left.nodeCount() + right.nodeCount();
            }
            else
            {
                return 10000;
            }
        }

        public static bool IsUnary(expr_type type)
        {
            return !(type == expr_type.plus || type == expr_type.minus || type == expr_type.times || type == expr_type.pow ||
                type == expr_type.root || type == expr_type.divides || type == expr_type.compose || type == expr_type.mean || type == expr_type.gmean);
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

        public void mutate(Random rnm, double jitter)
        {
            if(!double.IsNaN(val) && !double.IsInfinity(val))
            {
                val += rnm.NextDouble() * jitter * 2.0 - jitter;    // val += [-jitter, +jitter]
            }

            if(left != null)
            {
                left.mutate(rnm, jitter);
            }

            if(right != null)
            {
                right.mutate(rnm, jitter);
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
                    temp += "o";
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
                case expr_type.mean:
                    temp += "mean";
                    break;
                case expr_type.gmean:
                    temp += "gmean";
                    break;
            }

            string lstr = (left == null ? "" : left.ToString());
            string rstr = (right == null ? "" : right.ToString());

            temp = "(" +  lstr + " " + temp + " " + rstr + ")";
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
                if ((double.IsNaN(this.val) && double.IsNaN(t.val)) || this.val == t.val)
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

        public static extree synthesize(Stack<string> tokens)
        {
            // initially tree is empty and tokens are full.
            // eventually tree will be full and tokens empty

            if(tokens.Count == 0)
            {
                return null;
            }

            string token = tokens.Pop();

            extree e;
            extree ce;

            switch(token)
            {
                case "plus":
                    e = new extree(expr_type.plus, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "minus":
                    e = new extree(expr_type.minus, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "times":
                    e = new extree(expr_type.times, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "divides":
                    e = new extree(expr_type.divides, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "pow":
                    e = new extree(expr_type.pow, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "mean":
                    e = new extree(expr_type.mean, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "gmean":
                    e = new extree(expr_type.gmean, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "compose":
                    e = new extree(expr_type.compose, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;
                case "root":
                    e = new extree(expr_type.root, 0.0);
                    e.left = synthesize(tokens);
                    e.right = synthesize(tokens);                    
                    return e;

                case "ln":
                    e = new extree(expr_type.ln, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "exp":
                    e = new extree(expr_type.exp, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "sin":
                    e = new extree(expr_type.sin, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "cos":
                    e = new extree(expr_type.cos, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "tan":
                    e = new extree(expr_type.tan, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "constant":
                    e = new extree(expr_type.constant, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "neg":
                    e = new extree(expr_type.neg, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "recip":
                    e = new extree(expr_type.recip, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "asin":
                    e = new extree(expr_type.asin, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "acos":
                    e = new extree(expr_type.acos, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "atan":
                    e = new extree(expr_type.atan, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "sqrt":
                    e = new extree(expr_type.sqrt, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "abs":
                    e = new extree(expr_type.abs, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "cosh":
                    e = new extree(expr_type.cosh, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "sinh":
                    e = new extree(expr_type.sinh, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "tanh":
                    e = new extree(expr_type.tanh, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "powinvsqrt2":
                    e = new extree(expr_type.powinvsqrt2, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }
                case "powsqrt2":
                    e = new extree(expr_type.powsqrt2, 0.0);

                    ce = synthesize(tokens);

                    if(ce.type != expr_type.constant)
                    {
                        Console.WriteLine("Synthesize error");
                        return e;
                    }
                    else
                    {
                        e.val = ce.val;
                        return e;
                    }

                case "x":
                    e = new extree(expr_type.constant, double.NaN);
                    return e;

                default:
                    double v;

                    if(double.TryParse(token, out v))
                    {
                        e = new extree(expr_type.constant, v);
                        return e;
                    }
                    else
                    {
                        Console.WriteLine("Synthesize error");
                        e = new extree(expr_type.constant, 0.0);
                        return e;
                    }
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
        powsqrt2,
        mean,
        gmean
    }
}
