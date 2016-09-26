using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Forms;
using PdfSharp.Pdf;
using System.Linq;

namespace TestSystem
{
    public class HTMLSectionParser
    {
        public List<HTMLTag> activeTags;
        public List<HTMLSection> sections;
        public bool done;
        public int stringIndex;
        public string html;
        public HTMLSection currentSection;
        public int jumpIndex;
        public bool contentReadingMode;
        public int orderedListIndexNumber;
        public ListMode listMode;

        public HTMLSectionParser()
        {

        }

        public static void ParseTest()
        {
            HTMLSectionParser p = new HTMLSectionParser();
            p.ProcessSections(@"C:\example1.txt");

            StreamWriter sw = new StreamWriter(File.OpenWrite(@"c:\example2.txt"));

            for (int sectionN = 1; sectionN < p.sections.Count; sectionN++)
            {
                HTMLSection s = p.sections[sectionN];
                sw.WriteLine("Section# " + sectionN);
                sw.WriteLine("Content: " + s.content);
                sw.WriteLine((s.isBold ? "bold " : " ") + (s.isItalic ? "italic " : " ") + (s.isStrikethrough ? "strikethrough " : " ") + (s.isUnderline ? "underline " : " ") + "At " + s.fontSize + " pt");
                sw.WriteLine(Enum.GetName(typeof(BreakLevel), s.breakLevel));
                sw.WriteLine(Enum.GetName(typeof(PrAlignment), s.align));
                sw.WriteLine();
            }

            sw.Close();
        }

        public void ProcessSections(string infile)
        {
            StreamReader sr = new StreamReader(File.OpenRead(infile));
            html = sr.ReadToEnd();
            sr.Close();
            html = html.Replace("&nbsp;", " ");
            
            activeTags = new List<HTMLTag>();
            sections = new List<HTMLSection>();

            done = false;
            stringIndex = 0;
            char indexChar;
            string temp;

            while (!done && stringIndex < html.Length)
            {
                indexChar = html[stringIndex];

                if (indexChar == '<')
                {
                    jumpIndex = html.IndexOf('>', stringIndex) + 1;
                    temp = html.Substring(stringIndex, jumpIndex - stringIndex); // get the contents of <>

                    if (currentSection != null && currentSection.content != "")  // add the old
                    {
                        if (temp.ToLower() == @"<br />") // break level applies to exiting section
                        {
                            currentSection.breakLevel = BreakLevel.LineBreak;
                        }
                        else if (temp.ToLower() == @"</p>")
                        {
                            currentSection.breakLevel = BreakLevel.ParagraphBreak;
                        }
                        else if (temp.ToLower().StartsWith(@"</h"))
                        {
                            currentSection.breakLevel = BreakLevel.ParagraphBreak;
                        }
                        else if (temp.ToLower().StartsWith("<li"))
                        {
                            currentSection.breakLevel = BreakLevel.LineBreak;
                        }
                        else if (temp.ToLower() == "</ol>")
                        {
                            currentSection.breakLevel = BreakLevel.LineBreak;
                        }
                        else if (temp.ToLower() == "</ul>")
                        {
                            currentSection.breakLevel = BreakLevel.LineBreak;
                        }

                        currentSection.HTMLTransform();
                        sections.Add(currentSection);
                    }

                    ProcessTagDeclaration(temp);    // bring in the new tag

                    HTMLTag ht;

                    if (activeTags.Count > 0)
                    {
                        ht = activeTags.Aggregate<HTMLTag>(HTMLTag.CombineTagProperties);
                    }
                    else
                    {
                        ht = new HTMLTag(); // default
                    }

                    currentSection = new HTMLSection();
                    currentSection.align = (ht.align == PrAlignment.NoAlignment ? PrAlignment.Left : ht.align);
                    currentSection.isBold = ht.isBold;
                    currentSection.isItalic = ht.isItalic;
                    currentSection.isStrikethrough = ht.isStrikethrough;
                    currentSection.isUnderline = ht.isUnderline;
                    currentSection.fontSize = (ht.FontSize == -1 ? 12 : ht.FontSize);
                    currentSection.content = "";

                    if (activeTags.Count > 0 && activeTags[activeTags.Count - 1].TagName == "li")  // last tag
                    {
                        if (listMode == ListMode.OrderedList)
                        {
                            currentSection.content = orderedListIndexNumber.ToString() + ".  ";
                            orderedListIndexNumber++;
                        }
                        else
                        {
                            currentSection.content = "*  ";
                        }
                    }

                    contentReadingMode = true;
                    stringIndex = jumpIndex;
                }
                else if (contentReadingMode)
                {
                    currentSection.content += indexChar.ToString(); // add the character into the content
                    stringIndex++;
                }
            }     
        }

        public void ProcessTagDeclaration(string temp)
        {
            if (temp.EndsWith("/>")) // self closing tag
                return;
            else if (temp.StartsWith("</"))
            {
                contentReadingMode = false;
                string t = temp.ToLower();
                string tagname = temp.Substring(2, temp.IndexOf('>') - 2);

                for (int i = activeTags.Count - 1; i >= 0; i--) // last first traversal
                {
                    if (activeTags[i].TagName == tagname)
                    {
                        activeTags.Remove(activeTags[i]);
                    }
                }
            }
            else
            {
                HTMLTag ttag = new HTMLTag(temp);

                if (ttag.TagName == "ol")
                {
                    orderedListIndexNumber = 1;
                    listMode = ListMode.OrderedList;
                }
                else if (ttag.TagName == "ul")
                {
                    orderedListIndexNumber = -1;
                    listMode = ListMode.UnorderedList;
                }

                activeTags.Add(ttag);
            }
        }

        static void NBSPTransform(string infile, string oufile)
        {
            StreamReader sr = new StreamReader(File.OpenRead(infile));
            string html = sr.ReadToEnd();
            sr.Close();
            html = html.Replace("&nbsp;", " ");
            StreamWriter sw = new StreamWriter(File.OpenWrite(oufile));
            sw.Write(html);
            sw.Close();
        }
    }
}
