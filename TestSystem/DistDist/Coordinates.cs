using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem
{
    public class Coordinates
    {
        public double x { get; set; }
        public double y { get; set; }

        public Coordinates(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinates()
        {
            this.x = 0.0;
            this.y = 0.0;
        }

        public Coordinates(Random rnd, double maxX, double maxY)
        {
            x = rnd.NextDouble() * maxX;
            y = rnd.NextDouble() * maxY;
        }

        public static double distance(Coordinates left, Coordinates right)
        {
            double dx = left.x - right.x;
            double dy = left.y - right.y;

            return (Math.Sqrt(dx * dx + dy * dy));
        }
    }
}
