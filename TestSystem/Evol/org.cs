using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.Evol
{
    public class Org
    {
        public int id;

        public extree basis;
        public OrgType orgType;
        public bool alive;
        public bool harvested;

        public OpMode opMode;

        public double x;
        public double y;

        public int wins;

        public int secsToModeChange;

        public static int maxID = 0;

        public Org(extree basis, OrgType orgType, double x, double y)
        {
            this.id = maxID;
            maxID++;

            this.basis = basis;
            this.orgType = orgType;
            this.alive = true;
            this.harvested = false;
            this.x = x;
            this.y = y;
            this.wins = 0;

            if(orgType == OrgType.combiner)
            {
                opMode = OpMode.corpsefind;
                secsToModeChange = 100000;
            }
            else
            {
                opMode = OpMode.flee;
                secsToModeChange = 3;
            }
        }

        public Org(Org o)
        {
            this.id = maxID;
            maxID++;

            this.basis = new extree(o.basis);
            this.orgType = o.orgType;
            this.x = o.x;
            this.y = o.y;
            this.wins = 0;

            this.alive = true;
            this.harvested = false;

            this.opMode = OpMode.flee;
            this.secsToModeChange = 3;
        }

        public void mutate(Random rnm, double jitter)
        {
            basis.mutate(rnm, jitter);
        }

        public static Org synthesize(string text, double x, double y)
        {
            var tokens = text.Split(' ');

            var strStack = new Stack<string>();
            var treeStack = new Stack<extree>();

            // We want the first item to be pushed onto the stack last
            // So that we pop it off first
            for(int i = tokens.Length - 1; i >= 0; i--)
            {
                strStack.Push(tokens[i]);
            }

            var ex = extree.synthesize(strStack);

            Org o = new Org(ex, OrgType.basic, x, y);
            return o;
        }
    }

    public enum OrgType
    {
        basic,
        combiner
    }

    public enum OpMode
    {
        pursue,
        flee,
        corpsefind
    }
}
