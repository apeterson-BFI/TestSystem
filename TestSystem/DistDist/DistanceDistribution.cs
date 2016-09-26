using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestSystem
{
    public class DistanceDistribution
    {
        public List<Coordinates> particles;
        public double[] distances;
        public Coordinates active;

        public DistanceDistribution(int n, Random rnm)
        {
            particles = new List<Coordinates>(n);
            distances = new double[n - 1];
            active = null;

            for (int i = 0; i < n; i++)
            {
                particles.Add(new Coordinates(rnm, 1.0, 1.0));
            }

            active = particles[0];
        }

        public double[] getDistances()
        {
            double best = double.MaxValue;
            Coordinates bestParticle = null;

            double dist = 0.0;

            int j = 0;

            while (particles.Count > 1)
            {
                best = double.MaxValue;
                bestParticle = null;

                particles.Remove(active);

                for (int i = 0; i < particles.Count; i++)
                {
                    dist = Coordinates.distance(active, particles[i]);

                    if (dist < best)
                    {
                        best = dist;
                        bestParticle = particles[i];
                    }
                }

                active = bestParticle;
                distances[j] = best;
                j++;
            }

            return distances;
        }

        public static void TestDistribution(int n, int s)
        {
            Random rnm = new Random();

            double[] distances = new double[n-1];
            double[] tempdist;


            for (int i = 0; i < n-1; i++)
            {
                distances[i] = 0.0;
            }

            for (int i = 0; i < s; i++)
            {
                DistanceDistribution dd = new DistanceDistribution(n, rnm);

                tempdist = dd.getDistances();

                for (int j = 0; j < n-1; j++)
                {
                    distances[j] += tempdist[j];
                }
            }

            for (int i = 0; i < n-1; i++)
            {
                distances[i] /= (double)s;
            }

            StreamWriter sw = File.CreateText(@"C:\users\aap\desktop\distances.txt");

            for (int i = 0; i < n-1; i++)
            {
                sw.WriteLine("{0},{1:0.00000}", i+1, distances[i]);
            }

            sw.Flush();
            sw.Close();
        }

    }
}
