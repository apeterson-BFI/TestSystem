using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem.Calculation
{
    public class SeriesInexact
    {
        public int currentArraySize;
        public int arraySizeIncrement;
        public BigFloat[,] cauchyArray;

        public void CalcQuadratic(List<BigFloat> initialTerms, int terms, string path)
        {
            currentArraySize = 1000;
            arraySizeIncrement = 500;

            StreamWriter sw = File.CreateText(path);

            cauchyArray = new BigFloat[currentArraySize, currentArraySize];

            if (initialTerms == null || initialTerms.Count == 0)
            {
                return;
            }

            for (int i = 0; i < initialTerms.Count; i++)
            {
                sw.WriteLine(initialTerms[i].ToString() + ", ");
                Console.WriteLine("a(" + i + ") = " + initialTerms[i].ToString());
                cauchyArray[1,i] = initialTerms[i];
            }

            for (int i = 2; i < initialTerms.Count; i++)
            {
                for (int j = 0; j < initialTerms.Count; j++)
                {
                    cauchyArray[i,j] = CalculateCauchyTerm(i-1, j);
                }
            }
           
            BigFloat nextval;
            int nextIndex = initialTerms.Count;
        
            while(nextIndex <= terms)
            {
                nextval = CalculateNextSeriesValue(nextIndex);

                sw.Write(nextval.ToString() + ", ");
                Console.WriteLine("a(" + nextIndex + ") = " + nextval.ToString());
                nextIndex++;
            }

            sw.Close();
        }

        public BigFloat CalculateCauchyTerm(int prevRow, int column)
        {
            BigFloat accum;
            accum.significand = 0;
            accum.exponent = 0;

            BigFloat temp;

            for (int i = 0; i <= column; i++)
            {
                if (cauchyArray[1,i].significand == 0 || cauchyArray[prevRow, column - i].significand == 0)
                {
                    continue;
                }

                temp = BigFloat.Multiply(cauchyArray[1,i], cauchyArray[prevRow, column - i]);
                accum = BigFloat.Add(accum, temp);
            }

            return accum;
        }

        public void RedimCauchy()
        {
            int nextDimSize = currentArraySize + arraySizeIncrement;

            BigFloat[,] tempa = new BigFloat[nextDimSize, nextDimSize];

            for (int i = 0; i < currentArraySize; i++)
            {
                for (int j = 0; j < currentArraySize; j++)
                {
                    tempa[i, j] = cauchyArray[i, j];
                }
            }

            currentArraySize = nextDimSize;
            cauchyArray = tempa;
        }

        public BigFloat CalculateNextSeriesValue(int n)
        {
            if (n == currentArraySize)
            {
                RedimCauchy();
            }

            // add n'th value to each existing cauchy row.
            for (int i = 2; i < n; i++)
            {
                cauchyArray[i,n] = CalculateCauchyTerm(i-1, n);
            }

            // add nth row.
  
            for (int j = 0; j <= n; j++)
            {
                cauchyArray[n, j] = CalculateCauchyTerm(n-1, j);
            }

            BigFloat accum;
            accum.significand = 0;
            accum.exponent = 0;

            BigFloat temp;
            temp.significand = 0;
            temp.exponent = 0;

            for (int i = 2; i < n; i++)
            {
                temp = BigFloat.Multiply(cauchyArray[1,i], cauchyArray[i,n]);
                accum = BigFloat.Add(accum, temp);
            }

            accum.exponent--;
            accum.significand = -1 * accum.significand;

            cauchyArray[1,n] = accum; 

            return accum;
        }
    }
}
