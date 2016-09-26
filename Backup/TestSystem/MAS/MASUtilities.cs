using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem
{
    public class MASUtilities
    {
        public static void ConvertDDPCsvToFixed()
        {
            StreamReader sr = new StreamReader(@"C:\ach.csv");
            StreamWriter sw = new StreamWriter(@"C:\ach.txt");

            string line;
            string outline = "";

            while ((line = sr.ReadLine()) != null)
            {
                outline = "";
                char[] chars = new char[1];
                chars[0] = ',';

                string[] lineparts = line.Split(chars);

                outline += FormatRecord(3 - 1, lineparts[9].Replace('"', ' ').Trim(null), "{0:##}", true);
                outline += FormatRecord(10 - 3, lineparts[10].Replace('"', ' ').Trim(null), "{0:#######}", false);
                outline += FormatRecord(27 - 10, lineparts[11].Replace('"', ' ').Trim(null), "{0, -17}", false);
                outline += FormatRecord(37 - 27, lineparts[12].Replace('"', ' ').Trim(null), "{0, -9}", false);
                outline += FormatRecord(45 - 37, lineparts[13].Replace('"', ' ').Trim(null), "{0,8:#####.00}", true);
                outline += FormatRecord(47 - 45, lineparts[14].Replace('"', ' ').Trim(null), "{0,-2:##}", false);
                outline += FormatRecord(62 - 47, lineparts[15].Replace('"', ' ').Trim(null), "{0}", false);
                outline += FormatRecord(-1, lineparts[16].Replace('"', ' ').Trim(null), "{0}", false);
                sw.WriteLine(outline);
            }

            sw.Flush();
        }

        public static void StripCrystalStyleCSV(string filenamer, string filenamew)
        {
            StreamReader sr = new StreamReader(File.OpenRead(filenamer));
            StreamWriter sw = new StreamWriter(File.OpenWrite(filenamew));

            string r;
            string[] spl;
            string w;

            // In each line, the first for splitted parts are dropped and the last is, thus each line should give 6 or more parts (4 + 1 + n) where n is the amount of true columns)
            while ((r = sr.ReadLine()) != null)
            {
                spl = r.Split(new char[] { ',' });

                if (spl.Length < 6)
                    throw new Exception("Invalid Input File");

                // we want to go from 4 (inc.) to length - 1 (exc.) therefore 4, length - 5 should work, since the element start + length is excluded
                w = String.Join(",", spl, 4, spl.Length - 5);
                sw.WriteLine(w);
            }

            sr.Close();
            sw.Flush();
            sw.Close();
        }

        public static string FormatRecord(int mandatoryLength, string val, string format, bool isNumeric)
        {
            object temp = val;
            string tempS;

            if (isNumeric)
            {
                tempS = String.Format(format, Convert.ToDouble(val));
            }
            else
            {
                tempS = string.Format(format, temp);
            }

            if (mandatoryLength == -1)
                return (tempS);
            else if (tempS.Length > mandatoryLength)
            {
                tempS = tempS.Substring(0, mandatoryLength);
                return (tempS);
            }
            else
                return (string.Format("{0, " + (-mandatoryLength) + "}", tempS));
        }
    }
}
