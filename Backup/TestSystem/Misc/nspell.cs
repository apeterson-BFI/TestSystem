using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class nspell
    {
        public static int Read(string s)
        {
            Dictionary<string, int> tags = SetupCatchValueTags();

            if(s.IndexOf("negative") != -1)
            {
                return(-Read(s.Remove(s.IndexOf("negative"), "negative".Length).TrimStart(null)));
            }

            int a3 = 0; // hundred trap accumulator
            int a2 = 0; // thousand/million/billion trap accumulator
            int a1 = 0; // general accumulator

            string[] tokens = s.Split(null);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (!tags.ContainsKey(tokens[i]))
                    throw new ArgumentException("Bad numerical token: value match not found");

                if (a3 != 0)
                {
                    // Hundreds has come up with the hundreds accumulator just set.
                    // So add the a3 accumulator multiplied by 100 to the a2 accumulator
                    if (tags[tokens[i]] == 100)
                    {
                        a2 += a3 * 100;

                        // and reset a3
                        a3 = 0;
                    }
                    // Otherwise, add the straight value of a3 to a2, reset a3 and process normally. 
                    else
                    {
                        a2 += a3;
                        a3 = 0;

                        // If a multiplier tag comes in, add to the primary accumulator and reset a2
                        if (tags[tokens[i]] >= 1000)
                        {
                            a1 += a2 * tags[tokens[i]];
                            a2 = 0;
                        }
                        // else add to a2
                        else
                        {
                            a2 += tags[tokens[i]];
                        }
                    }
                }
                else
                {
                    // If a multiplier tag comes in, add to the primary and reset a2
                    if (tags[tokens[i]] >= 1000)
                    {
                        if (a2 == 0)
                        {
                            a1 += tags[tokens[i]];
                        }
                        else
                        {
                            a1 += a2 * tags[tokens[i]];
                            a2 = 0;
                        }
                    }
                    // Non-A3 eligible token val
                    else if (tags[tokens[i]] >= 10)
                    {
                        a2 += tags[tokens[i]];
                    }
                    // A3 elibible
                    else
                    {
                        a3 = tags[tokens[i]];
                    }
                }
            }

            a1 += a3;
            a1 += a2;

            return (a1);
        }

        public static string Spell(int v)
        {
            if (v < 0)
                return ("negative " + Spell(-v));

            if (v < 1000)
                return (SpellSmall(v));

            string[] tnames = new string[] { "one", "thousand", "million", "billion" };
            List<int> tvals = GroupByThousands(v);

            string temp = "";

            for (int i = tvals.Count - 1; i >= 0; i--)
            {
                if (tvals[i] != 0)
                {
                    if (i != 0)
                    {
                        temp += Spell(tvals[i]).ToString() + " " + tnames[i] + " ";
                    }
                    else
                    {
                        temp += Spell(tvals[0]);
                    }
                }
            }

            return (temp);
        }

        private static string SpellSmall(int v)
        {
            string[] nnum = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] teennum = new string[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensnum = new string[] { "zero", "ten", "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            string hn = "hundred";

            if (v == 0)
            {
                return nnum[0];
            }
            else if (v % 100 == 0)
            {
                return (SpellSmall(v / 100) + " " + hn);
            }

            int hs = v / 100;
            int hr = v % 100;

            string hunds = "";
            string rems = "";

            if (hs != 0)
            {
                hunds = SpellSmall(hs * 100) + " ";
            }

            if (hr < 10)
            {
                rems = nnum[hr];
            }
            else if (hr < 20)
            {
                rems = teennum[hr - 10];
            }
            else if (hr % 10 == 0)
            {
                rems = tensnum[hr / 10];
            }
            else
            {
                rems = tensnum[hr / 10] + " " + nnum[hr % 10];
            }

            return (hunds + rems);
        }

        public static List<int> GroupByThousands(int v)
        {
            List<int> thousandGroups = new List<int>();
            int temp = v;

            while (temp > 0)
            {
                thousandGroups.Add(temp % 1000);
                temp = temp / 1000;
            }

            return thousandGroups;
        }

        public static Dictionary<string, int> SetupCatchValueTags()
        {
            Dictionary<string, int> sval = new Dictionary<string, int>();

            for (int i = 0; i < 20; i++)
            {
                sval.Add(Spell(i), i);
            }

            for (int i = 20; i < 100; i += 10)
            {
                sval.Add(Spell(i), i);
            }

            sval.Add("hundred", 100);
            sval.Add("thousand", 1000);
            sval.Add("million", 1000 * 1000);
            sval.Add("billion", 1000 * 1000 * 1000);

            return sval;
        }
    }
}
