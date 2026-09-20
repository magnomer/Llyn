using System;
using System.Globalization;
using System.Text;

namespace Llyn.Infrastructure;

public static class LFolioLine
{
    public static string LFolioLineFormat(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        StringBuilder safe = new StringBuilder(text.Length);

        foreach (char letter in text)
        {
            switch (letter)
            {
                case '&':
                    safe.Append("&amp;");
                    break;
                case '<':
                    safe.Append("&lt;");
                    break;
                case '>':
                    safe.Append("&gt;");
                    break;
                case '"':
                    safe.Append("&quot;");
                    break;
                case '\r':
                    break;
                case '\n':
                    safe.Append("</w:t><w:br/><w:t xml:space=\"preserve\">");
                    break;
                case '\t':
                    safe.Append("</w:t><w:tab/><w:t xml:space=\"preserve\">");
                    break;
                default:
                    safe.Append(letter);
                    break;
            }
        }

        return safe.ToString();
    }

    public static void LFolioLineAppend(StringBuilder body, string style, string text)
    {
        ArgumentNullException.ThrowIfNull(body);

        body.Append("<w:p><w:pPr><w:pStyle w:val=\"").Append(style).Append("\"/></w:pPr>")
            .Append("<w:r><w:t xml:space=\"preserve\">")
            .Append(LFolioLineFormat(text))
            .Append("</w:t></w:r></w:p>");
    }

    public static void LFolioLineAppend(
        StringBuilder body, string style, string mark, string text)
    {
        ArgumentNullException.ThrowIfNull(body);

        body.Append("<w:p><w:pPr><w:pStyle w:val=\"").Append(style).Append("\"/></w:pPr>")
            .Append("<w:r><w:rPr><w:b/></w:rPr><w:t xml:space=\"preserve\">")
            .Append(LFolioLineFormat(mark))
            .Append("</w:t></w:r><w:r><w:t xml:space=\"preserve\">")
            .Append(LFolioLineFormat(text))
            .Append("</w:t></w:r></w:p>");
    }

    public static void LFolioLineDraw(
        StringBuilder body, int place, long width, long height)
    {
        ArgumentNullException.ThrowIfNull(body);

        string cx = width.ToString(CultureInfo.InvariantCulture);
        string cy = height.ToString(CultureInfo.InvariantCulture);
        string id = place.ToString(CultureInfo.InvariantCulture);

        body.Append("<w:p><w:pPr><w:pStyle w:val=\"Plate\"/></w:pPr><w:r><w:drawing>")
            .Append("<wp:inline distT=\"0\" distB=\"0\" distL=\"0\" distR=\"0\">")
            .Append("<wp:extent cx=\"").Append(cx).Append("\" cy=\"").Append(cy).Append("\"/>")
            .Append("<wp:effectExtent l=\"0\" t=\"0\" r=\"0\" b=\"0\"/>")
            .Append("<wp:docPr id=\"").Append(id).Append("\" name=\"Image ").Append(id).Append("\"/>")
            .Append("<wp:cNvGraphicFramePr><a:graphicFrameLocks noChangeAspect=\"1\"/>")
            .Append("</wp:cNvGraphicFramePr>")
            .Append("<a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/")
            .Append("drawingml/2006/picture\">")
            .Append("<pic:pic><pic:nvPicPr><pic:cNvPr id=\"").Append(id)
            .Append("\" name=\"Image ").Append(id).Append("\"/><pic:cNvPicPr/></pic:nvPicPr>")
            .Append("<pic:blipFill><a:blip r:embed=\"rId").Append((place + 100).ToString(CultureInfo.InvariantCulture))
            .Append("\"/><a:stretch><a:fillRect/></a:stretch></pic:blipFill>")
            .Append("<pic:spPr><a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"").Append(cx)
            .Append("\" cy=\"").Append(cy).Append("\"/></a:xfrm>")
            .Append("<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom></pic:spPr>")
            .Append("</pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing></w:r></w:p>");
    }
}
