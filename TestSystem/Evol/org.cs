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
