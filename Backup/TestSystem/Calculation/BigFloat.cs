using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem.Calculation
{
    public struct BigFloat
    {
        public static long multLimit = (1 << 30);
        public static BigFloat bigZero = new BigFloat(0, 0);

        public long significand;
        public int exponent;

        public BigFloat(long si, int xp)
        {
            significand = si;
            exponent = xp;
        }

        public BigFloat(int val)
        {
            significand = (long)val;
            exponent = 0;
        }

        public BigFloat(BigFloat d)
        {
            significand = d.significand;
            exponent = d.exponent;
        }

        public static BigFloat Add(BigFloat left, BigFloat right)
        {
            int sh;
            long shsign1;
            long shsign2;
            int grexp;
            long res;

            if (left.significand == 0)
            {
                return right;
            }
            else if (right.significand == 0)
            {
                return left;
            }

            // Scale the value with the lower exponent to the higher exponent by reducing the significant.
            // Store the greater fo the two exponents, which will be used in the output unless more scaling occurs.
            if (left.exponent < right.exponent)
            {
                // Scale the lesser left value
                sh = right.exponent - left.exponent;

                // Can't shift more than 63 (111111 in binary) places
                if (sh > 63)
                {
                    return right;
                }

                shsign1 = left.significand >> sh;
                shsign2 = right.significand;
                grexp = right.exponent;
            }
            else if (left.exponent > right.exponent)
            {
                sh = left.exponent - right.exponent;

                // Can't shift more than 63 (111111 in binary) places
                if (sh > 63)
                {
                    return left;
                }

                shsign1 = right.significand >> sh;
                shsign2 = left.significand;
                grexp = left.exponent;
            }
            else
            {
                shsign1 = left.significand;
                shsign2 = right.significand;
                grexp = left.exponent;
            }

            // we consider overflow possible if any argument is half or more of Min or Max val.
            if (shsign1 > long.MaxValue / 2L || shsign2 > long.MaxValue / 2L
                || shsign1 < long.MinValue / 2L && shsign2 < long.MinValue / 2L)
            {
                // if we could have overflow then divide both values by 2, and add 1 to the final exponent
                grexp++;

                shsign1 >>= 1;
                shsign2 >>= 1;
            }

            res = shsign1 + shsign2;

            BigFloat r;
            r.significand = res;
            r.exponent = grexp;

            return(r);
        }

        public static int qshift(long val, long limit)
        {
            long q = val;
            long c = limit;
            long nc = -limit;

            int counter = 0;

            while (q > c || q < nc)
            {
                q >>= 1;
                counter++;
            }

            return counter;
        }

        public static BigFloat Multiply(BigFloat left, BigFloat right)
        {
            long shsign1;
            long shsign2;

            // we consider overflow possible if any argument uses more than half of the non-sign bits
            // i.e. 2^30 or greater, or -2^30 or lesser
            // We scale down to 2^30 th for such values.

            if (left.significand == 0)
            {
                return bigZero;
            }

            if (right.significand == 0)
            {
                return bigZero;
            }

            shsign1 = left.significand;
            shsign2 = right.significand;

            int s1 = qshift(shsign1, multLimit);
            int s2 = qshift(shsign2, multLimit);

            shsign1 >>= s1;
            shsign2 >>= s2;

            BigFloat r;

            r.significand = shsign1 * shsign2;
            r.exponent = left.exponent + right.exponent + s1 + s2;

            return r;
        }

        public static BigFloat operator /(BigFloat left, BigFloat right)
        {
            long shsign1;
            long shsign2;
            int grexp;
            long res;

            long lposlimit;
            long lneglimit;
            long rposlimit;
            long rneglimit;

            shsign1 = left.significand;
            shsign2 = right.significand;
            grexp = left.exponent - right.exponent;

            if (shsign1 == 0)
            {
                return bigZero;
            }

            if (shsign2 == 0)
            {
                throw new ArithmeticException("Divide by zero");
            }

            lposlimit = 1L;
            lposlimit <<= 62;
            lneglimit = -1L;
            lneglimit <<= 62;
            rposlimit = 1L;
            rposlimit <<= 30;
            rneglimit = -1L;
            rneglimit <<= 30;

            while (shsign1 < lposlimit && shsign1 > lneglimit)
            {
                shsign1 <<= 1;
                grexp--;
            }

            while (shsign2 > rposlimit || shsign2 < rneglimit)
            {
                shsign2 >>= 1;
                grexp--;
            }

            res = shsign1 / shsign2;

            BigFloat r;
            r.significand = res;
            r.exponent = grexp;

            return r;
        }

        public bool IsZero()
        {
            return (significand == 0);
        }

        public override string ToString()
        {
            if (significand == 0)
            {
                return ("0.00000E0");
            }

            int tenxp = 0;
            double valacc = Math.Abs(Convert.ToDouble(significand));
            double vsign = Math.Sign(significand);

            // Take the exponent * Log10(2) + Log10(significand)
            // Whole number part is the E
            // 10 ^ Remainder is the base 10 significand
            double logtemp = Math.Log10(2.0) * exponent + Math.Log10(valacc);
            double lgten = Math.Floor(logtemp);

            tenxp += (int)lgten;

            logtemp = logtemp - lgten;
            logtemp = Math.Pow(10.0, logtemp);
            logtemp = logtemp * vsign;

            return (logtemp.ToString("0.00000") + "E" + tenxp.ToString());
         }
    }
}
