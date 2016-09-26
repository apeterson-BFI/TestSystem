using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace TestSystem
{
    static class MiscStatic
    {
        public static void WriterStats(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            int character_count = 0;
            int word_count = 0;
            int line_count = 0;
            int paragraph_count = 0;
            int page_count = 0;

            string[] splits;
            string line = "";

            while ((line = sr.ReadLine()) != null)
            {
                if (line == "")
                    paragraph_count++;

                character_count += line.Length;

                splits = line.Split(null);

                for (int i = 0; i < splits.Length; i++)
                {
                    if (splits[i].Length == 1 && Char.IsPunctuation(splits[i][0]))
                        continue;
                    else
                        word_count++;
                }

                line_count++;
            }

            page_count = (int)Math.Ceiling((((double)line_count) / 80.0));

            Console.WriteLine("Characters: " + character_count);
            Console.WriteLine("Words: " + word_count);
            Console.WriteLine("Lines: " + line_count);
            Console.WriteLine("Paragraphs: " + paragraph_count);
            Console.WriteLine("Pages: " + page_count);
        }

        public static double Solve(double[] poly)
        {
            double best_guess = 0.0;
            double best_error = Eval(poly, best_guess);
            double t1;
            double t2;

            while (Math.Abs(best_error) >= 0.001)
            {
                t1 = Eval(poly, best_guess + 0.001);
                t2 = Eval(poly, best_guess - 0.001);
                t1 = (t1 - t2) / 0.002;

                if (t1 == 0)
                {
                    best_guess = new Random().NextDouble() * 20.0 - 10.0;
                }
                else
                {
                    best_guess = best_guess - best_error / t1;
                }

                best_error = Eval(poly, best_guess);
            }

            return best_guess;
        }

        public static double Eval(double[] poly, double x)
        {
            double temp = 0.0;
            double m = 1.0;

            for (int i = 0; i < poly.Length; i++)
            {
                if (i == 0)
                    temp += poly[i];
                else
                {
                    m = 1.0;

                    for (int j = 0; j < i; j++)
                    {
                        m *= x;
                    }

                    m *= poly[i];

                    temp += m;
                }
            }

            return temp;
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
                return (typeof(object));
            else if (types.Length == 1)
                return (types[0]);

            // Copy the parameter so we can substitute base class types in the array without messing up the caller
            Type[] temp = new Type[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                temp[i] = types[i];
            }

            bool checkPass = false;

            Type tested = null;

            while (!checkPass)
            {
                tested = temp[0];

                checkPass = true;

                for (int i = 1; i < temp.Length; i++)
                {
                    if (tested.Equals(temp[i]))
                        continue;
                    else
                    {
                        // If the tested common basetype (current) is the indexed type's base type
                        // then we can continue with the test by making the indexed type to be its base type
                        if (tested.Equals(temp[i].BaseType))
                        {
                            temp[i] = temp[i].BaseType;
                            checkPass = false;
                            continue;
                        }
                        // If the tested type is the indexed type's base type, then we need to change all indexed types
                        // before the current type (which are all identical) to be that base type and restart this loop
                        else if (tested.BaseType.Equals(temp[i]))
                        {
                            for (int j = 0; j <= i - 1; j++)
                            {
                                temp[j] = temp[j].BaseType;
                            }

                            checkPass = false;
                            break;
                        }
                        // The indexed type and the tested type are not related
                        // So make everything from index 0 up to and including the current indexed type to be their base type
                        // because the common base type must be further back
                        else
                        {
                            for (int j = 0; j <= i; j++)
                            {
                                temp[j] = temp[j].BaseType;
                            }

                            checkPass = false;
                            break;
                        }
                    }
                }

                // If execution has reached here and checkPass is true, we have found our common base type, 
                // if checkPass is false, the process starts over with the modified types
            }

            // There's always at least object
            return tested;
        }
    }
}
