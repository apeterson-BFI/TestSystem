using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem
{
    public class Collatz
    {
        public BigInteger current;
        public BigInteger three;
        public BigInteger two;
        public BigInteger one;

        public int stepN;

        public string testVal;
        public int radix;

        public static int record = 0;

        public Collatz(string testVal, int radix)
        {
            this.testVal = testVal;
            this.radix = radix;
            current = new BigInteger(testVal, radix);
            three = new BigInteger(3L);
            two = new BigInteger(2L);
            one = new BigInteger(1L);
        }

        public void process(StreamWriter sw)
        {
            stepN = 1;

            while (!current.Equals(one))
            {
                step(sw);
                stepN++;

                if (stepN > record)
                {
                    record = stepN;
                    Console.WriteLine("New duration record: " + record + " for init value: " + testVal);
                    sw.WriteLine("New duration record: " + record + " for init value: " + testVal);
                }
            }
        }

        public void step(StreamWriter sw)
        {
            BigInteger rem = current % two;

            if (rem.Equals(one))
            {
                do3x1();
            }
            else
            {
                dohalf();
            }
        }

        public void do3x1()
        {
            current = current * three;
            current = current + one;
        }

        public void dohalf()
        {
            current = current / two;
        }
    }
}
