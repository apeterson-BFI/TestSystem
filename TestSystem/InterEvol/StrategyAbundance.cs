using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.InterEvol
{
    public class StrategyAbundance
    {
        public double population { get; private set; }
        public Strategy strategy { get; private set; }
        public int rank { get; set; }

        public StrategyAbundance(Strategy strategy)
        {
            this.population = 1.0;
            this.strategy = strategy;
        }

        public void update(double modifier)
        {
            population *= modifier;
        }

        public override string ToString()
        {
            return string.Format("pop {0:0.000} : {1}", population, strategy);
        }
    }
}
