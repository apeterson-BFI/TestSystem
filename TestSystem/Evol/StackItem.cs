using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSystem.Evol
{
    public class StackItem
    {
        public expr_type? op { get; set; }
        public double? val { get; set; }
    }
}
