using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestSystem
{
    public class Huffman
    {
        public string in_filename;
        public string out_filename;
        public List<LTree> letter_trees;
        public Dictionary<char, int> occd;
        public int total_write;
        public List<int> stored_compress;

        public Huffman(string in_filename, string out_filename)
        {
            this.in_filename = in_filename;
            this.out_filename = out_filename;

            letter_trees = new List<LTree>();
            occd = new Dictionary<char, int>();
            InitChars();
            stored_compress = new List<int>();
        }

        public void Compress()
        {
            BuildOccurences();
            BuildLetterTree();
            WriteEncoding();
            WriteOutput();
        }

        public void WriteOutput()
        {
            BinaryWriter bw = new BinaryWriter(File.Create(out_filename));
            
            // Write bit count
            bw.Write(total_write);

            // Write occurences by char
            for (int i = 0; i < 256; i++)
            {
                if (occd.ContainsKey((char)i))
                {
                    bw.Write((char)i);
                    bw.Write(occd[(char)i]);
                }
            }

            // Following zero signals that tree occurences section is over
            bw.Write((char)0);

            for (int i = 0; i < stored_compress.Count; i++)
            {
                bw.Write(stored_compress[i]);
            }

            bw.Close();
        }

        public void WriteEncoding()
        {
            StreamReader sr = new StreamReader(in_filename);
            int bys = 0;
            LTree full_tree = letter_trees[0];
            int bit_index = 0;
            int r;
            LTree l;
            
            total_write = 0;

            while ((r = sr.Read()) != -1)
            {
                if (r > 255)
                    continue;

                l = full_tree;

                while (l.left != null && l.right != null)
                {
                    if (l.left.letter.IndexOf((char)r) != -1)
                    {
                        bys = bys ^ (1 << (31 - bit_index));
                        l = l.left;
                        bit_index++;
                        total_write++;

                        if (bit_index == 32)
                        {
                            bit_index = 0;
                            stored_compress.Add(bys);
                            bys = 0;
                        }
                    }
                    else if (l.right.letter.IndexOf((char)r) != -1)
                    {
                        bys = bys ^ (1 << (31 - bit_index));
                        l = l.right;
                        bit_index++;
                        total_write++;

                        if (bit_index == 32)
                        {
                            bit_index = 0;
                            stored_compress.Add(bys);
                            bys = 0;
                        }
                    }
                    else
                    {
                        throw new Exception("Malformed Compression Tree");
                    }
                }
            }
        }

        private void BuildLetterTree()
        {
            int worsti1;
            int worsti2;
            int worsto1;
            int worsto2;

            while (letter_trees.Count > 1)
            {
                worsti1 = -1;
                worsti2 = -1;
                worsto1 = Int32.MaxValue;
                worsto2 = Int32.MaxValue;

                for (int i = 0; i < letter_trees.Count; i++)
                {
                    if (letter_trees[i].occurence < worsto1)
                    {
                        worsto1 = letter_trees[i].occurence;
                        worsti1 = i;
                    }
                }

                for (int i = 0; i < letter_trees.Count; i++)
                {
                    if (i == worsti1)
                        continue;

                    if (letter_trees[i].occurence < worsto2)
                    {
                        worsto2 = letter_trees[i].occurence;
                        worsti2 = i;
                    }
                }

                LTree c = new LTree(letter_trees[worsti1].letter + letter_trees[worsti2].letter, worsto1 + worsto2);
                c.left = new LTree(letter_trees[worsti1]);
                c.right = new LTree(letter_trees[worsti2]);

                letter_trees.RemoveAt(worsti1);

                if (worsti2 > worsti1)
                {
                    letter_trees.RemoveAt(worsti2 - 1);
                }
                else
                {
                    letter_trees.RemoveAt(worsti2);
                }

                letter_trees.Add(c);
            }
        }

        private void BuildOccurences()
        {
            StreamReader sr = new StreamReader(in_filename);

            int r;

            while ((r = sr.Read()) >= 0)
            {
                if (r < 256 && r >= 0)
                {
                    letter_trees[r].occurence++;

                    if (occd.ContainsKey((char)r))
                        occd[((char)r)] = occd[((char)r)] + 1;
                    else
                        occd.Add((char)r, 1);
                }
            }

            sr.Close();
        }

        private void InitChars()
        {
            for (int i = 0; i < 255; i++)
            {
                letter_trees.Add(new LTree((char)i, 0));
            }
        }
    }
}
