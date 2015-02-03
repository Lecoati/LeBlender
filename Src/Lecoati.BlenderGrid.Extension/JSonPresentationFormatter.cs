using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Lecoati.BlenderGrid.Extension
{
    public class JSonPresentationFormatter
    {
        public string Format(object obj)
        {
            var serializer = new JavaScriptSerializer();
            var text = serializer.Serialize(obj);

            return Format(text);
        }

        public string Format(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.Replace(System.Environment.NewLine, string.Empty).Replace("\t", string.Empty);

            var offset = 0;
            var output = new StringBuilder();
            Action<StringBuilder, int> tabs = (sb, pos) => { for (var i = 0; i < pos; i++) { sb.Append("\t"); } };
            Func<string, int, Nullable<Char>> previousNotEmpty = (s, i) =>
            {
                if (string.IsNullOrEmpty(s) || i <= 0) return null;

                Nullable<Char> prev = null;

                while (i > 0 && prev == null)
                {
                    prev = s[i - 1];
                    if (prev.ToString() == " ") prev = null;
                    i--;
                }

                return prev;
            };
            Func<string, int, Nullable<Char>> nextNotEmpty = (s, i) =>
            {
                if (string.IsNullOrEmpty(s) || i >= (s.Length - 1)) return null;

                Nullable<Char> next = null;
                i++;

                while (i < (s.Length - 1) && next == null)
                {
                    next = s[i++];
                    if (next.ToString() == " ") next = null;
                }

                return next;
            };

            for (var i = 0; i < text.Length; i++)
            {
                var chr = text[i];

                if (chr.ToString() == "{")
                {
                    offset++;
                    output.Append(chr);
                    output.Append(System.Environment.NewLine);
                    tabs(output, offset);
                }
                else if (chr.ToString() == "}")
                {
                    offset--;
                    output.Append(System.Environment.NewLine);
                    tabs(output, offset);
                    output.Append(chr);

                }
                else if (chr.ToString() == ",")
                {
                    output.Append(chr);
                    output.Append(System.Environment.NewLine);
                    tabs(output, offset);
                }
                else if (chr.ToString() == "[")
                {
                    output.Append(chr);

                    var next = nextNotEmpty(text, i);

                    if (next != null && next.ToString() != "]")
                    {
                        offset++;
                        output.Append(System.Environment.NewLine);
                        tabs(output, offset);
                    }
                }
                else if (chr.ToString() == "]")
                {
                    var prev = previousNotEmpty(text, i);

                    if (prev != null && prev.ToString() != "[")
                    {
                        offset--;
                        output.Append(System.Environment.NewLine);
                        tabs(output, offset);
                    }

                    output.Append(chr);
                }
                else
                    output.Append(chr);
            }

            return output.ToString().Trim();
        }
    }
}