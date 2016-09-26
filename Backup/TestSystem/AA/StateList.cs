using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.AA
{
    public class StateList : System.Collections.Generic.Dictionary<int, StateChance>
    {
        // Get the StateChance instance for att/def pair
        public StateChance GetState(int att, int def)
        {
            return (this[StateChance.GetStateHash(att, def)]);
        }



        // Get the associated probability for att/def pair
        public double GetStateP(int att, int def)
        {
            return (GetState(att, def).probability);
        }

        // Get the sum of all state probabilities
        // This should be close to 1 in a normalized probability distribution
        public double SumAllP()
        {
            return (this.Sum(x => x.Value.probability));
        }

        // Get the number of states that are not final
        // When this goes to zero, the iterative calculation can stop
        public int SumAllNonFinal()
        {
            return (this.Sum(x => (x.Value.IsEndState() ? 0 : 1)));
        }

        public void Normalize()
        {
            double psum = SumAllP();

            foreach (KeyValuePair<int, StateChance> scp in this)
            {
                scp.Value.DivScale(psum);
            }
        }

        public void AddState(int att, int def, double p)
        {
            int hash = StateChance.GetStateHash(att, def);

            if (this.ContainsKey(hash))
            {
                StateChance sc = this[hash];
                sc.probability += p;
            }
            else
            {
                this.Add(hash, new StateChance(att, def, p));
            }
        }

        public void AddState(StateChance sc)
        {
            int hash = StateChance.GetStateHash(sc.attq, sc.defq);

            if (this.ContainsKey(hash))
            {
                StateChance t = this[hash];
                t.probability += sc.probability;
            }
            else
            {
                this.Add(hash, new StateChance(sc.attq, sc.defq, sc.probability));
            }
        }

        public void RemoveState(int att, int def)
        {
            int hash = StateChance.GetStateHash(att, def);

            if (this.ContainsKey(hash))
            {
                this.Remove(hash);
            }
        }

        public void Merge(StateList stl)
        {
            foreach (KeyValuePair<int, StateChance> kc in stl)
            {
                this.AddState(kc.Value);
            }
        }

        public double PVictory(double p_a, double p_d, double prunep, int a, int d)
        {
            this.Clear();
            this.Add(StateChance.GetStateHash(a, d), new StateChance(a, d, 1.0));

            while (SumAllNonFinal() > 0)
            {
                Step(p_a, p_d, prunep);
            }

            double attp = 0.0;

            foreach (KeyValuePair<int, StateChance> kc in this)
            {
                if (kc.Value.attq > 0 && kc.Value.defq == 0)
                    attp += kc.Value.probability;
            }

            return attp;
        }

        public void Step(double p_a, double p_d, double prunep)
        {
            StateList stl = new StateList();

            foreach (KeyValuePair<int, StateChance> kc in this)
            {
                if (kc.Value.IsEndState())
                    stl.AddState(kc.Value);

                stl.Merge(kc.Value.TransitionStates(p_a, p_d));
            }

            stl.Normalize();

            this.Clear();
            this.Merge(stl);
        }
    }
}
