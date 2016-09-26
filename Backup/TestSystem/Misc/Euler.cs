using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numeric;

namespace TestSystem
{
    public class Euler
    {
        // 1: Sum the numbers divisible by 3 or 5 that are less than 1000
        public static int EP_1(int max, int[] mults)
        {
            int acc = 0;

            for (int i = 2; i <= max; i++)
            {
                for (int j = 0; j < mults.Length; j++)
                {
                    if (i % mults[j] == 0)
                    {
                        acc += i;
                        break;
                    }
                }
            }

            return acc;
        }

        // 2: Sum the even fibonacci numbers <= 4 million
        public static int EP_2(int max, int div)
        {
            int acc = 2;
            int v1 = 1;
            int v2 = 2;
            int test;

            test = v1 + v2;

            while (test <= max)
            {
                if (test % div == 0)
                    acc += test;

                v1 = v2;
                v2 = test;
                test = v1 + v2;
            }

            return acc;
        }

        // 2: Largest Prime Factor
        public static long EP_3(long n)
        {
            if (n == 1L)
                return 1L;

            if (n % 2L == 0L)
            {
                return Math.Max(2L, EP_3(n / 2L));
            }
            else if (n % 3L == 0L)
            {
                return Math.Max(3L, EP_3(n / 3L));
            }

            if (n % 5L == 0L)
            {
                return (Math.Max(5L, EP_3(n / 5L)));
            }

            for (long f = 7; f <= Convert.ToInt64(Math.Floor(Math.Sqrt((double)n))); f += 6)
            {
                if (n % f == 0)
                {
                    return (Math.Max(f, EP_3(n / f)));
                }
                if (n % (f + 4L) == 0L)
                {
                    return (Math.Max(f + 4, EP_3(n / (f + 4L))));
                }
            }

            // Get here only if nothing found in loop, therefore prime.

            return (n);
        }

        public static int EP_4(int min, int max)
        {
            int temp;
            int best = -1;

            for (int i = min; i <= max; i++)
            {
                for (int j = min; j <= max; j++)
                {
                    temp = i * j;

                    if (IsPalindrome(temp) && temp > best)
                    {
                        best = temp;
                    }
                }
            }

            return best;
        }


        public static bool IsPalindrome(int n)
        {
            string ns = Convert.ToString(n);

            while (ns.Length > 1)
            {
                if (ns[0] != ns[ns.Length - 1])
                    return false;
                else
                {
                    ns = ns.Substring(1, ns.Length - 2);
                }
            }

            return true;
        }

        /* 2520 is the smallest number that can be divided by each of the numbers from 1 to 10 without any remainder.
         *   
         * What is the smallest number that is evenly divisible by all of the numbers from 1 to 20
         */
        public static long EP_5(long n1, long n2)
        {
            long lcd = 1L;

            for (long i = n1; i <= n2; i++)
            {
                lcd = (lcd * i) / GCD(lcd, i);
            }

            return lcd;
        }

        public static long GCD(long a, long b)
        {
            if (b == 0L)
                return a;

            return (GCD(b, a % b));
        }

        // Find the difference between the sum of the squares of the first one hundred natural numbers and the square of the sum.
        public static long EP_6(int n1, int n2)
        {
            int ssqr = 0;
            int sqrs = 0;

            for (int i = n1; i <= n2; i++)
            {
                sqrs += i;
                ssqr += i * i;
            }

            sqrs = sqrs * sqrs;

            return (sqrs - ssqr);
        }

        /* By listing the first six prime numbers: 2, 3, 5, 7, 11, and 13, we can see that the 6th prime is 13.
         * What is the 10001st prime number?
         */
        public static int EP_7(int n, int max_sieve)
        {
            bool[] sieve = new bool[max_sieve + 1];
            List<int> primes = new List<int>();
            int max_test = Convert.ToInt32(Math.Floor(Math.Sqrt(Convert.ToDouble(max_sieve))));

            for (int i = 2; i < sieve.Length; i++)
            {
                sieve[i] = true;
            }

            for (int i = 2; i <= max_test; i++)
            {
                if (sieve[i])
                {
                    for (int j = 2 * i; j <= max_sieve; j += i)
                    {
                        sieve[j] = false;
                    }
                }
            }

            int found = 0;
            int index = 2;

            while (found < n)
            {
                if (sieve[index])
                    found++;

                index++;
            }

            return (index - 1);
        }

        public static int EP_8()
        {
            string numstring = "";

            numstring += "73167176531330624919225119674426574742355349194934";
            numstring += "96983520312774506326239578318016984801869478851843";
            numstring += "85861560789112949495459501737958331952853208805511";
            numstring += "12540698747158523863050715693290963295227443043557";
            numstring += "66896648950445244523161731856403098711121722383113";
            numstring += "62229893423380308135336276614282806444486645238749";
            numstring += "30358907296290491560440772390713810515859307960866";
            numstring += "70172427121883998797908792274921901699720888093776";
            numstring += "65727333001053367881220235421809751254540594752243";
            numstring += "52584907711670556013604839586446706324415722155397";
            numstring += "53697817977846174064955149290862569321978468622482";
            numstring += "83972241375657056057490261407972968652414535100474";
            numstring += "82166370484403199890008895243450658541227588666881";
            numstring += "16427171479924442928230863465674813919123162824586";
            numstring += "17866458359124566529476545682848912883142607690042";
            numstring += "24219022671055626321111109370544217506941658960408";
            numstring += "07198403850962455444362981230987879927244284909188";
            numstring += "84580156166097919133875499200524063689912560717606";
            numstring += "05886116467109405077541002256983155200055935729725";
            numstring += "71636269561882670428252483600823257530420752963450";

            int max = 0;
            int temp = 1;

            for (int i = 0; i + 4 < numstring.Length; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    temp *= Convert.ToInt32(numstring[i + j].ToString());
                }

                if (temp > max)
                    max = temp;

                temp = 1;
            }

            return max;
        }
    }
}
