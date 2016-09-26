using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem.LabelParse
{
    public class LabelParser
    {
        public static string startpath = @"c:\users\aap\desktop\boxlaabels";

        public List<string> filesDone;

        public LabelParser()
        {
            filesDone = new List<string>();


        }

        public void process(StreamWriter sw, DirectoryInfo curr)
        {
            if (curr == null)
            {
                return;
            }

            DirectoryInfo[] subdirs = curr.GetDirectories();
            FileInfo[] subfiles = curr.GetFiles();

            if (subdirs != null)
            {
                for (int i = 0; i < subdirs.Length; i++)
                {
                    process(sw, subdirs[i]);
                }
            }

            if (subfiles != null)
            {
                for (int i = 0; i < subfiles.Length; i++)
                {
                    processFile(sw, subfiles[i]);
                }
            }


        }

        public void processFile(StreamWriter sw, FileInfo f)
        {
            // System.Console.WriteLine(f.FullName);

            if (f.Extension != ".prc")
            {
                return;
            }

            byte curr = 0;
            byte last = 0;
            byte third = 0;

            List<byte> buffer = new List<byte>() ;
            bool bufferActive = false;
            
            BinaryReader b = new BinaryReader(f.Open(FileMode.Open));

            last = b.ReadByte();

            try
            {
                while(true)
                {
                    third = last;
                    last = curr;
                    curr = b.ReadByte();

                    // pattern recognized
                    if (!bufferActive && third == (byte) 0xFF  && last == (byte)0x0B && curr == (byte)0x30)
                    {
                        bufferActive = true;
                        buffer = new List<byte>();
                        buffer.Add(0x30);
                    }
                    else if (bufferActive)
                    {
                        // Correct bar code terminal
                        if (curr == (byte)0x02)
                        {
                            Console.WriteLine("GOOD");
                            emitBuffer(sw, buffer, f);
                            bufferActive = false;
                        }
                        else
                        {
                            if (curr >= 48 && curr <= 57)
                            {
                                Console.Write((curr - 48).ToString());
                            }

                            buffer.Add(curr);
                        }
                    }
                }
            }
            catch(EndOfStreamException)
            {
                b.Close();
            }
        }

        public void emitBuffer(StreamWriter sw, List<byte> buffer, FileInfo f)
        {
            if (filesDone.Contains(f.FullName))
            {
                return;
            }

            string s = "";
            byte temp = 0;
            int ntemp = 0;

            for (int i = 0; i < buffer.Count; i++)
            {
                temp = buffer[i];

                if (temp < 48 || temp > 57)
                {
                    continue;
                }

                ntemp = temp - 48;

                s += ntemp.ToString();
            }

            if (s.Length == 11)
            {
                sw.WriteLine(s + " " + f.FullName);
                filesDone.Add(f.FullName);
            }
        }
    }
}
