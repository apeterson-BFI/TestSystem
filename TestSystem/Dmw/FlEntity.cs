using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Dmw
{
    public class FlEntity : DEntity
    {
        public int mrate;
        public double mchance;
        public int minterval;
        public int mphase;
        public int spersion;
        public List<FlEntity> s;

        /// <summary>
        /// Constructs a FlEntity at a random location within the bounding rectangle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="archetype"></param>
        /// <param name="xmin">Mininum x of the bound</param>
        /// <param name="xmax">Maximum x of the bound</param>
        /// <param name="ymin">Minimum y of the bound</param>
        /// <param name="ymax">Maximum y of the bound</param>
        /// <param name="phase"></param>
        /// <param name="inc"></param>
        /// <param name="mrate">Quantity of m attempts</param>
        /// <param name="mchance">Chance of m</param>
        /// <param name="minterval">Frequency of m possibility checks</param>
        public FlEntity(string name, string archetype, int xmin, int xmax, int ymin, int ymax, double phase, double inc, int mrate, double mchance, int minterval)
        {
            spersion = 5;   // default
            s = new List<FlEntity>();
            this.name = name;
            this.archetype = archetype;

            Random rnm = new Random();
            x = rnm.Next(xmin, xmax + 1);
            y = rnm.Next(ymin, ymax + 1);

            this.phase = phase;
            this.inc = inc;
            this.mrate = mrate;
            this.mchance = mchance;
            this.minterval = minterval;
            mphase = 0;
        }

        /// <summary>
        /// Constructs a FlEntity at the given location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="archetype"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="phase"></param>
        /// <param name="inc"></param>
        /// <param name="mrate">Quantity of m attempts</param>
        /// <param name="mchance">Chance of m</param>
        /// <param name="minterval">Frequency of m possibility checks</param>
        public FlEntity(string name, string archetype, int x, int y, double phase, double inc, int mrate, double mchance, int minterval)
        {
            spersion = 5;
            s = new List<FlEntity>();
            this.name = name;
            this.archetype = archetype;
            this.x = x;
            this.y = y;
            this.phase = phase;
            this.inc = inc;
            this.mrate = mrate;
            this.mchance = mchance;
            this.minterval = minterval;
            mphase = 0;
        }

        /// <summary>
        /// Default constructor for use with extending classes
        /// </summary>
        public FlEntity()
        {
            spersion = 5;   // default
            s = new List<FlEntity>();
        }

        public override void Run()
        {
            base.Run();
            mphase++;

            Random rnm = new Random();

            if (mphase == minterval)
            {
                for (int i = 0; i < mrate; i++)
                {
                    if (rnm.NextDouble() < mchance)
                    {
                        FlEntity temp;

                        s.Add((temp = new FlEntity(name + rnm.Next(), archetype, x - 5, x + 5, y - 5, y + 5, 0, inc, mrate, mchance, minterval)));

                        // Remove if on same spot
                        if (temp.x == x && temp.y == y)
                            s.Remove(temp);
                    }
                }
            }

            mphase = 0;
        }

        public virtual List<FlEntity> TakeC()
        {
            List<FlEntity> temp = s;

            s = new List<FlEntity>();

            return temp;
        }
    }
}
