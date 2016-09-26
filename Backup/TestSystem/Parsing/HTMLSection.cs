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
namespace TestSystem
{
    public class HTMLSection
    {
        public int fontSize;
        public PrAlignment align;
        public bool isBold;
        public bool isItalic;
        public bool isUnderline;
        public bool isStrikethrough;
        public string content;
        public BreakLevel breakLevel;

        public HTMLSection()
        {
            fontSize = -1;
            align = PrAlignment.NoAlignment;
            content = "";
            breakLevel = BreakLevel.NoBreak;
        }

        // runs entity transforms on content
        public void HTMLTransform()
        {
            content.Replace("&lt;", "<");
            content.Replace("&gt;", ">");
            content.Replace("&quot;", @"""");
            content.Replace("&apos;", "'");
            content.Replace("&amp;", "&");
        }
    }
}
