using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class LTree
    {
        public int occurence;
        public string letter;
        public LTree left;
        public LTree right;

        public LTree(char l, int o)
        {
            letter = "" + l;
            occurence = o;
        }

        public LTree(string l, int o)
        {
            letter = l;
            occurence = o;
        }

        // DEEP COPY, used for remaking trees during HUFFMAN
        public LTree(LTree lt)
        {
            letter = lt.letter;
            occurence = lt.occurence;

            if (lt.left != null && lt.right != null)
            {
                left = new LTree(lt.left);
                right = new LTree(lt.right);
            }
        }
    }
}
