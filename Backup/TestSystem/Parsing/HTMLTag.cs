using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    public class HTMLTag
    {
        public string TagName;
        public int FontSize;
        public PrAlignment align;
        public bool isBold;
        public bool isItalic;
        public bool isUnderline;
        public bool isStrikethrough;

        public HTMLTag()
        {
            FontSize = -1;
            align = PrAlignment.NoAlignment;
            TagName = "";
        }

        public HTMLTag(string tagDeclareString) // tagDeclareString should be everything between the < and the > inclusive.
        {
            FontSize = -1;
            align = PrAlignment.NoAlignment;
            TagName = "";

            string temp = tagDeclareString.ToLower();   // no content here, so it doesn't matter

            if (temp == "<em>")
            {
                align = PrAlignment.NoAlignment;
                TagName = "em";
                isItalic = true;
            }
            else if (temp == "<strong>")
            {
                align = PrAlignment.NoAlignment;
                TagName = "strong";
                isBold = true;
            }
            else if (temp.StartsWith("<span"))
            {
                TagName = "span";
                ProcessStyles(temp.Substring(5).Trim());
            }
            else if (temp.StartsWith("<p"))
            {
                TagName = "p";
                ProcessStyles(temp.Substring(2).Trim());
            }
            else if (temp.StartsWith("<h") && Char.IsDigit(temp[2]))
            {
                TagName = temp.Substring(1, 2);
                isBold = true;

                char hval = temp[2];
                
                // FontSizes = H1 = 24, H2 = 18, H3 = 14, H4 = 12, H5 = 10, H6 = 8
                switch (hval)
                {
                    case '1':
                        FontSize = 24;
                        break;
                    case '2':
                        FontSize = 18;
                        break;
                    case '3':
                        FontSize = 14;
                        break;
                    case '4':
                        FontSize = 12;
                        break;
                    case '5':
                        FontSize = 10;
                        break;
                    case '6':
                        FontSize = 8;
                        break;
                }

                ProcessStyles(temp.Substring(3).Trim());
            }
            else if (temp.StartsWith("<li"))
            {
                TagName = "li";
            }
            else if (temp.StartsWith("<ul"))
            {
                TagName = "ul";
            }
            else if (temp.StartsWith("<ol"))
            {
                TagName = "ol";
            }
        }

        public void ProcessStyles(string tagContents) // strip the < and the tag name from the string, trim whitespace, 
        {                                             // and leave everything after that up to the tag close
            int styleTextStart = tagContents.IndexOf(@"style=""") + 7;

            if (styleTextStart == 6)    // no style
                return;

            int styleTextEnd = tagContents.IndexOf(@"""", styleTextStart);
            string styleText = tagContents.Substring(styleTextStart, styleTextEnd + 1 - styleTextStart);

            string[] styles = styleText.Split(new char[] { ';' });
            string styleDecType;
            string styleDecValue;

            if (styles == null || styles.Length == 0)
                return;

            for (int i = 0; i < styles.Length; i++)
            {
                string[] parts = styles[i].Split(new char[] { ':' });

                if (parts == null || parts.Length < 2)
                {
                    continue;
                }

                styleDecType = parts[0].Trim();
                styleDecValue = parts[1].Trim();

                switch (styleDecType)
                {
                    case "font-size":
                        switch (styleDecValue)
                        {
                            case "xx-large":
                                FontSize = 36;
                                break;
                            case "x-large":
                                FontSize = 24;
                                break;
                            case "large":
                                FontSize = 18;
                                break;
                            case "medium":
                                FontSize = 14;
                                break;
                            case "small":
                                FontSize = 12;
                                break;
                            case "x-small":
                                FontSize = 10;
                                break;
                            case "xx-small":
                                FontSize = 8;
                                break;
                            default:
                                FontSize = -1;
                                break;
                        }
                        break;
                    case "text-align":
                        switch (styleDecValue)
                        {
                            case "left":
                                align = PrAlignment.Left;
                                break;
                            case "center":
                                align = PrAlignment.Center;
                                break;
                            case "right":
                                align = PrAlignment.Right;
                                break;
                            case "justify":
                                align = PrAlignment.Justify;
                                break;
                            default:
                                align = PrAlignment.NoAlignment;
                                break;
                        }
                        break;
                    case "text-decoration":
                        switch (styleDecValue)
                        {
                            case "underline":
                                isUnderline = true;
                                break;
                            case "line-through":
                                isStrikethrough = true;
                                break;    
                        }
                        break;
                }
            }
        }

        public static HTMLTag CombineTagProperties(HTMLTag tagA, HTMLTag tagB)
        {
            HTMLTag t = new HTMLTag();

            // the new Tag overrides alignment if its has an alignment
            if (tagB.align != PrAlignment.NoAlignment)
                t.align = tagB.align;
            // Otherwise the tag the old Tag's alignment
            else
                t.align = tagA.align;

            t.FontSize = (tagB.FontSize != -1 ? tagB.FontSize : tagA.FontSize);

            t.isBold = tagA.isBold | tagB.isBold;
            t.isItalic = tagA.isItalic | tagB.isItalic;
            t.isStrikethrough = tagA.isStrikethrough | tagB.isStrikethrough;
            t.isUnderline = tagA.isUnderline | tagB.isUnderline;

            return t;
        }
    }
}
