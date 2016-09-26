using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Evol
{
    // Constraints
    // 1. If IsBinary is false, left and right must be null
    // 2. If IsBinary is true, left and right must not be null
    public class BinaryLayout
    {
        private bool isBinary;
        private BinaryLayout left;
        private BinaryLayout right;
        private static Random rnm = new Random();
        public double p;

        // Unary creation
        public BinaryLayout()
        {
            isBinary = false;
            left = null;
            right = null;
        }

        // Binary creation
        public BinaryLayout(BinaryLayout l, BinaryLayout r)
        {
            if (l == null || r == null)
                throw new ArgumentNullException();

            isBinary = true;
            left = l;
            right = r;
        }

        // Full Depth Copy
        public BinaryLayout CopyLayout()
        {
            BinaryLayout c = new BinaryLayout();
            c.isBinary = isBinary;

            if (isBinary)
            {
                c.left = left.CopyLayout();
                c.right = right.CopyLayout();
            }

            return c;
        }

        // Generates the next layout in a list of possibilities for binary layout arrangements.
        // Staying within the tier means removing other binary connections to generate new ones.
        // The A to BAA transition only occurs directly if on the topLevel, otherwise it is done as
        // part of the next level up's transitions.
        public BinaryLayout NextLayoutInTier(bool topLevel, int maxLayers)
        {
            BinaryLayout c = CopyLayout();

            // A to BAA transition [TopLevel only]
            if (!c.isBinary && topLevel)
            {
                c.isBinary = true;
                c.left = new BinaryLayout();
                c.right = new BinaryLayout();
                return c;
            }
            else if(!c.isBinary && !topLevel)
            {
                throw new Exception("Bad BinaryLayout code in NextLayoutInTier");
            }

            // BAA to BBA transition
            if (c.isBinary && !c.left.isBinary && !c.right.isBinary)
            {
                c.left = new BinaryLayout(new BinaryLayout(), new BinaryLayout());
                return c;
            }

            // BBA to BAB
            if (c.isBinary && c.left.isBinary && !c.right.isBinary)
            {
                if (c.left.IsInLastState(maxLayers - 1))
                {
                    // reset lefts subs to state 0 to prepare to go to BAB from BBA 
                    c.left.isBinary = false;
                    c.left.left = null;
                    c.left.right = null;
                    c.right = new BinaryLayout(new BinaryLayout(), new BinaryLayout());
                    // now we have [BA[BAA]]
                    return c;
                }
                // left is not done yet so we have to drill down to get the next configuration
                else
                {
                    c.left = c.left.NextLayoutInTier(false, maxLayers - 1);    // c.left becomes the next state in left
                    return c;
                }
            }

            // BAB to BBB
            if (c.isBinary && !c.left.isBinary && c.right.isBinary)
            {
                if (c.right.IsInLastState(maxLayers - 1))
                {
                    // add left back in so we have BBB
                    c.left = new BinaryLayout(new BinaryLayout(), new BinaryLayout());
                    // reset the state of right to a BAA model to begin the recursive process again under the higher order BBB
                    c.right = new BinaryLayout(new BinaryLayout(), new BinaryLayout());

                    return c;
                }
                else
                {
                    c.right = c.right.NextLayoutInTier(false, maxLayers - 1);
                    return c;
                }
            }

            // BBB to END-STATE (return null)
            if (c.isBinary && c.left.isBinary && c.right.isBinary)
            {
                if (c.left.IsInLastState(maxLayers - 1) && c.right.IsInLastState(maxLayers - 1))
                {
                    return null;    // we are done
                }
                // reset right, increment left
                else if (c.right.IsInLastState(maxLayers - 1))
                {
                    c.left = c.left.NextLayoutInTier(false, maxLayers - 1);
                    c.right = new BinaryLayout(new BinaryLayout(), new BinaryLayout()); // clear right and increment left. this is like a carry add loop through all L + R
                    return c;
                }
                // incremeent right only
                else
                {
                    c.right = c.right.NextLayoutInTier(false, maxLayers - 1);
                    return c;
                }
            }

            // should be unreachable
            throw new Exception("Unreachable code reached");
        }

        public bool IsInLastState(int maxLayers)
        {
            if (maxLayers < 1)
                throw new ArgumentException("Bad maxLayers, must be >=1");
            if (maxLayers == 1)
            {
                if (!isBinary)
                    return true;
                else
                    return false;
            }
            else if (maxLayers == 2)
            {
                if (isBinary == true && !left.isBinary && !right.isBinary)
                    return true;
                else
                    return false;
            }
            else
            {
                return (left.isBinary && right.isBinary && left.IsInLastState(maxLayers - 1) && right.IsInLastState(maxLayers - 1));
            }
        }

        public static BinaryLayout GetNTierInitialLayout(int n)
        {
            BinaryLayout c;
            BinaryLayout temp;

            c = new BinaryLayout();
            temp = c;
            int levelReached = 1;

            while (levelReached < n)
            {
                //  temp is unary here.
                // make temp binary and then assign temp to temp.left
                temp = new BinaryLayout(new BinaryLayout(), new BinaryLayout());
                temp = temp.left;
                levelReached++;
            }

            return c;
        }

        public static BinaryLayout RandomLayout(double pBinary, double decay)
        {
            if(pBinary >= 1.0)
                throw new ArgumentException("bad pBinary");

            if(decay < 0.0)
                throw new ArgumentException("bad decay");

            double r = rnm.NextDouble();

            if (r < pBinary)
            {
                return (new BinaryLayout(RandomLayout(pBinary - decay, decay), RandomLayout(pBinary - decay, decay)));
            }
            else
            {
                return (new BinaryLayout()); // unary
            }
        }

        public int DepthNodeCount(int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            else if (isBinary)
            {
                return (left.DepthNodeCount(depth - 1) + right.DepthNodeCount(depth - 1));
            }
            else
            {
                return 0;
            }
        }

        public bool IsBinary
        {
            get { return isBinary; }
        }

        public BinaryLayout Left
        {
            get { return left; }
        }

        public BinaryLayout Right
        {
            get { return right; }
        }

        public override string ToString()
        {
            if (!isBinary)
            {
                return ("A");
            }
            else
            {
                return ("[B" + left.ToString() + right.ToString() + "]");
            }
        }
    }
}
