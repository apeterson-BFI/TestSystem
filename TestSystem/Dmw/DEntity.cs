using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Dmw
{
    public class DEntity
    {
        public double phase;
        public double inc;
        public string name;
        public string archetype;
        public int x;
        public int y;
        public int status;  // 0-nominal, 1-remove

        /// <summary>
        /// Constructs entity randomly placed in the rectangular region provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="archetype"></param>
        /// <param name="xmin">Min x bound of the region</param>
        /// <param name="xmax">Max x bound of the region</param>
        /// <param name="ymin">Min y bound of the region</param>
        /// <param name="ymax">Max y bound of the region</param>
        /// <param name="phase">Starting phase [0-1)</param>
        /// <param name="inc">Phase change per t [0-1]</param>
        public DEntity(string name, string archetype, int xmin, int xmax, int ymin, int ymax, double phase, double inc)
        {
            this.name = name;
            this.archetype = archetype;

            Random rnm = new Random();
            x = rnm.Next(xmin, xmax + 1);
            y = rnm.Next(ymin, ymax + 1);

            this.phase = phase;
            this.inc = inc;
            status = 0;
        }

        /// <summary>
        /// Constructs entity in specific location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="archetype"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="phase">Starting phase [0-1)</param>
        /// <param name="inc">Phase changes per t [0-1]</param>
        public DEntity(string name, string archetype, int x, int y, double phase, double inc)
        {
            this.name = name;
            this.archetype = archetype;
            this.x = x;
            this.y = y;
            this.phase = phase;
            this.inc = inc;
            status = 0;
        }

        /// <summary>
        /// Default constructor for use by classes that extend this one
        /// Name is built based on the current time/date
        /// </summary>
        public DEntity()
        {
            name = DateTime.Now.ToString();
            name = name.Replace('/', 'e');
            name = name.Replace(':', 'a');
            archetype = "Entity";
            phase = 0.0;
            inc = 0.001;

            Random rnm = new Random();
            x = rnm.Next(0, 100);
            y = rnm.Next(0, 100);
            status = 0;
        }

        /// <summary>
        /// Update DEntity
        /// </summary>
        public virtual void Run()
        {
            phase += inc;
            if (phase >= 1.0)
                status = 1;
        }

        /// <summary>
        /// Reports on the status of the entity
        /// </summary>
        /// <returns>The relevant aspects of the entity</returns>
        public override string ToString()
        {
            return (name + " of type " + archetype + " is " + (status == 0 ? "alive" : "dead") + " and " +
                (phase < 0.2 ? "very young" : (phase < 0.4 ? "young" : (phase < 0.6 ? "mature" : (phase < 0.8 ? "old" : "ancient")))) +
                " and located at (" + x + "," + y + ")");
        }
    }
}
