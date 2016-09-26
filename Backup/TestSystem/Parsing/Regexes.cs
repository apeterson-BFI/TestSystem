using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestSystem
{
    class Regexes
    {
        // Regex namespace declaration syntax
        static bool RegexTest(string reg_string, string sample)
        {
            return (new Regex(reg_string).Match(sample).Success);
        }

        static void Tester()
        {
            Console.WriteLine(RegexTest(@"[^.]+(\.[^.]+)*", "System.Text.RegularExpressions").ToString());
            Console.WriteLine(RegexTest(@"[^.]+(\.[^.]+)*", "System.").ToString()); // Returned true instead of false
        }
    }
}
