using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem.Polynomials
{
    public class PolySet
    {
        public List<HashSet<Polynomial>> degreeSets;

        public PolySet()
        {
            degreeSets = new List<HashSet<Polynomial>>();
        }

        public HashSet<Polynomial> calcDegree(int degree)
        {
            if(degree == 0)
            {
                HashSet<Polynomial> h = new HashSet<Polynomial>();
                return h;
            }
            else if (degree == 1)
            {
                HashSet<Polynomial> h = new HashSet<Polynomial>();

                Polynomial x = new Polynomial(1, 1);
                Polynomial invx = x.pInvert();

                h.Add(x);
                h.Add(invx);

                return h;
            }
            else
            {
                HashSet<Polynomial> accum = new HashSet<Polynomial>();
                HashSet<Polynomial> temp = new HashSet<Polynomial>();

                for (int d1 = 1; d1 < degree; d1++)
                {
                    temp = crossProduct(degreeSets[d1], degreeSets[degree - d1]);
                    accum.UnionWith(temp);
                }

                accum = massInvert(accum);
                return accum;
            }
        }

        public HashSet<Polynomial> crossProduct(HashSet<Polynomial> left, HashSet<Polynomial> right)
        {
            HashSet<Polynomial> crossProduct = new HashSet<Polynomial>();

            Polynomial[] leftList = left.ToArray();
            Polynomial[] rightList = right.ToArray();

            Polynomial temp;

            for(int i = 0; i < leftList.Length; i++)
            {
                for(int j = 0; j < rightList.Length; j++)
                {
                    temp = leftList[i].multiply(rightList[j]);
                    crossProduct.Add(temp);
                }
            }

            return crossProduct;
        }

        public HashSet<Polynomial> massInvert(HashSet<Polynomial> baseSet)
        {
            HashSet<Polynomial> res = new HashSet<Polynomial>(baseSet);

            Polynomial[] l = baseSet.ToArray();

            for (int i = 0; i < l.Length; i++)
            {
                res.Add(l[i].pInvert());
            }

            return res;
        }

        public void display(HashSet<Polynomial> baseSet, StreamWriter sw)
        {
            Polynomial[] b = baseSet.ToArray();

            for (int i = 0; i < b.Length; i++)
            {
                sw.WriteLine(b[i].ToString());
            }
        }
    }
}
