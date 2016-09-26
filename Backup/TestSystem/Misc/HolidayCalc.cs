using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class HolidayCalc
    {
        public static void ListChristmasDOWs()
        {
            DateTime dt = DateTime.Parse("12/25/2008");

            for (int i = 0; i < 20; i++)
            {
                Console.Out.WriteLine("Christmas " + dt.Year + " is on " + HolidayCalc.GetDayOfWeek(dt));
                dt = new DateTime(dt.Year + 1, dt.Month, dt.Day);
            }
        }

        public static string GetDayOfWeek(DateTime dt)
        {
            int[] MonthLen = new int[13] {0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int year = 2009;
            int day_of_week = 4;
            string[] dow_name = new string[8] { "BAD", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // Returns # of days in year y
            Func<int, int> year_len =   (y =>
                                        ((y % 4 == 0)
                                        ? ((y % 100 == 0)
                                        ? ((y % 400 == 0)
                                        ? 366                       // year % 400 == 0 : Is a leap year 
                                        : 365)                      // year % 100 == 0 AND year % 400 != 0 : Is not a leap year. Century exception
                                        : 366)                      // year % 4 == 0 AND year % 100 != 0 : Is a leap year, not a century exception
                                        : 365));                    // year % 4 != 0 : Base case, not a leap year

            // Returns number of days between 01/01/y1 to 01/01/y2
            // negative if y1 > y2
            Func<int, int, int> year_range =    ((y1, y2) =>
                                                ((y1 < y2)
                                                ? Enumerable.Range(y1, y2 - y1).Aggregate<int, int>(0, ((ds, y) => ds + year_len(y)))
                                                : ((y1 != y2)
                                                ? -Enumerable.Range(y2, y1 - y2).Aggregate<int, int>(0, ((ds, y) => ds + year_len(y)))
                                                : 0)));

            int yoffsetdays = year_range(dt.Year, 2009);
            
            // Does the range inclusive of the min value but exclusive of the max include 2 (representing february)
            Func<int, int, bool> febInclusive = ((m1, m2) => Math.Min(m1, m2) <= 2 && Math.Max(m1, m2) > 2);

            // Returns the number of days between months in a given year : excepting Febuaries with 29 days
            // p1 is m_from, p2 is m_to
            Func<int, int, int> base_month_range =  ((m1, m2) =>
                                                    ((m1 < m2)
                                                    ? MonthLen.ToList<int>().GetRange(m1, m2 - m1).Sum()
                                                    : ((m1 != m2)
                                                    ? -MonthLen.ToList<int>().GetRange(m2, m1 - m2).Sum()
                                                    : 0)));

            // Incorporates 29 day februaries into the base formula
            Func<int, int, int, int>  month_range = ((m1, m2, y) => 
                                                    ((febInclusive(m1, m2)                                          // if we are covering February
                                                    && year_len(y) == 366)                                          // and it is a leap year
                                                    ? base_month_range(m1, m2) + Math.Sign(base_month_range(m1, m2))// The abs value needs to be +1
                                                    : base_month_range(m1, m2)));                                   // Else its just the same as before
            
            int dow_offset = 0;

            dow_offset -= (dt.Day - 1);
            dow_offset += month_range(dt.Month, 1, dt.Year);
            dow_offset += year_range(dt.Year, year);

            // from today to reference day is dow_offset days
            // today's dow + dow_offset = ref (mod 7[1-7])
            // today's dow = ref - dow_offset (mod 7[1-7])
            int temp = day_of_week - dow_offset;
            temp = temp % 7;

            if (temp <= 0)
                temp += 7;

            return (dow_name[temp]);
        }
    }
}
