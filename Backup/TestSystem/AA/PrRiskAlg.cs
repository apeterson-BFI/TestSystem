using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.AA
{
    public class PrRiskAlg
    {
        public static double InfantryOnlyP(int att, int def)
        {
            StateList stl = new StateList();

            // 1 / 6 Att , 2 / 6 Def
            return(stl.PVictory(1.0 / 6.0, 2.0 / 6.0, 0.0001, att, def));   
        }
    }
}
