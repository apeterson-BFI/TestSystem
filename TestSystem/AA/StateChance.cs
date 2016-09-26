using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.AA
{
    public class StateChance
    {
        public double probability;
        public int attq;
        public int defq;
        private int adhash;

        // Assert: d must be <1024
        public StateChance(int a, int d, double p)
        {
            probability = p;
            attq = a;
            defq = d;
            adhash = GetStateHash(attq, defq);
        }

        public void Increase(double pmargin)
        {
            probability += pmargin;
        }

        public void MultScale(double pscale)
        {
            probability *= pscale;
        }

        public void DivScale(double pscale)
        {
            probability /= pscale;
        }

        public static int GetStateHash(int att, int def)
        {
            return (att * 1024 + def);
        }

        public StateList TransitionStates(double p_a, double p_d)
        {
            StateList stl = new StateList();

            StateChance temp;

            for (int i = 0; i <= attq; i++)
            {
                for (int j = 0; j <= defq; j++)
                {
                    temp = MakeTransition(i, attq, j, defq, p_a, p_d);

                    stl.AddState(temp);
                }
            }

            int h = GetStateHash(attq, defq);

            // Status Quo State to be removed
            if (stl.ContainsKey(h))
            {
                stl.Remove(h);
                stl.Normalize();
            }

            foreach (KeyValuePair<int, StateChance> kc in stl)
            {
                kc.Value.probability *= probability;
            }

            return stl;
        }

        public StateChance MakeTransition(int k_a, int n_a, int k_d, int n_d, double p_a, double p_d)
        {
            // Return the current att and def minus the amount of selected destructions by the other side (k_a decrements defq, k_d decrements attq)
            // With the binomial cross-product probability
            return (new StateChance(Math.Max(0, n_a - k_d), Math.Max(0, n_d - k_a), PTransition(k_a, n_a, p_a) * PTransition(k_d, n_d, p_d))); 
        }

        public static double PTransition(int k, int n, double p)
        {
            double temp = (double)(Fact(n) / (Fact(k) * Fact(n - k)));  // n! / (k! * (n - k)!)

            temp *= Math.Pow(p, (double)k);
            temp *= Math.Pow(1.0 - p, (double)(n - k));

            return temp;
        }

        public static int Fact(int n)
        {
            int mult = 1;

            for (int i = 1; i <= n; i++)
            {
                mult *= i;
            }

            return mult;
        }

        public bool IsEndState()
        {
            return (attq == 0 || defq == 0);
        }
    }
}
