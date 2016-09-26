using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Dmw
{
    public class AEntity : DEntity
    {
        public AEntity c;
        public int rphase;
        public int rinterval;
        public int mbl;
        public int ctype;   // (-1) - Any (DEntity), 0 - FlEntity, 1 - AEntity, ctype > 1 - AEntity(ctype = outer.ctype - 1)
        public double cdec;
        public double cinc;
        public double cstatus;

        public AEntity()
        {
            c = new AEntity();
            rphase = 0;
            rinterval = 45;
            mbl = 2;
            ctype = -1;
            cdec = -0.01;
            cinc = 1.0;
            cstatus = 1.0;
        }

        public AEntity(string name, string archetype, int xmin, int xmax, int ymin, int ymax, double phase, double inc, int rphase, int rinterval, int mbl, int ctype,
                        double cdec, double cinc, double cstatus)
        {
            c = new AEntity();
            this.name = name;
            this.archetype = archetype;

            Random rnm = new Random();
            x = rnm.Next(xmin, xmax + 1);
            y = rnm.Next(ymin, ymax + 1);

            this.phase = phase;
            this.inc = inc;
            this.rphase = rphase;
            this.rinterval = rinterval;
            this.mbl = mbl;
            this.ctype = ctype;
            this.cdec = cdec;
            this.cinc = cinc;
            this.cstatus = cstatus;
        }

        public override void Run()
        {
            base.Run();
            
            if (rphase != 0)
                rphase++;
        }

        public virtual AEntity TakeC()
        {
            AEntity temp = c;
            c = null;

            return temp;
        }
    }
}
