using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    public class ExprExhaustive : ExprEvolver
    {
        public static int extreeCount = Enum.GetValues(typeof(expr_type)).Length;

        public int max_tier_depth;
        public BinaryLayout binaryLayout;
        public extree bestExpr;
        public double bestScore;
        public extree currentExpr;
        public double currentScore;

        public ExprExhaustive(extree refxp, double start, double end, double prec, int max_tier_depth)
        {
            this.reference = refxp;
            this.xmin = start;
            this.xmax = end;
            this.xprec = prec;
            this.terminal_score = 0.01;
            this.max_tier_depth = max_tier_depth;
            binaryLayout = new BinaryLayout();
            current_step = 1;
        }

        public override extree ProcessNSteps(int n, bool msg)
        {
            // Range of value contents. -1, 0, 1, 2, 3, 4, 5, x
            // Range of operators: increment by type of node. Keep incrementing until you get a unary or binary
            // When at max (extree_count - 1) we've reached end

            bool done = false;
            bool top_level = true;

            bestScore = double.MaxValue;
            currentExpr = extree.CreateExtreeFromBinaryLayout(binaryLayout);

            while (!done && current_step < n)
            {
                currentExpr.ScoreSqrt(reference, xmin, xmax, xprec);
                currentScore = currentExpr.true_score;

                if (!double.IsNaN(currentScore) && currentScore < bestScore)
                {
                    bestExpr = currentExpr.CopyExtree();
                    bestScore = currentScore;

                    if (msg)
                    {
                        Console.Out.WriteLine("Exhaustive Evol At Step #" + current_step + ":");
                        Console.Out.WriteLine("Best Expression: " + bestExpr.ToString());
                        Console.Out.WriteLine("With Error Score: " + bestScore);
                    }

                    if (bestScore < terminal_score)
                    {
                        return bestExpr;
                    }
                }

                extree temp = currentExpr.NextEnumerableValue(binaryLayout);

                if (temp == null)   // end of current layout's enumerables
                {
                    BinaryLayout temp2 = binaryLayout.NextLayoutInTier(top_level, max_tier_depth);

                    top_level = false;

                    if (temp2 == null)   // end of enumeration
                        return (bestExpr);

                    if (temp2.IsBinary && (temp2.Left == null || temp2.Right == null))
                    {
                        throw new Exception("Binary Layout Constraint Failure. Binary expr missing 1 or more children");
                    }
                    else if (!temp2.IsBinary && (temp2.Left != null || temp2.Right != null))
                    {
                        throw new Exception("Binary Layout Constraint Failure. Unary expr with children");
                    }

                    binaryLayout = temp2.CopyLayout();
                    currentExpr = extree.CreateExtreeFromBinaryLayout(binaryLayout);
                }
                else
                {
                    currentExpr = temp;
                    current_step++;
                }
            }

            return (bestExpr);
        }

        public override extree Process_Until_Terminal()
        {
            return(ProcessNSteps(Int32.MaxValue, true));
        }
    }
}
