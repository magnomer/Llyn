using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioNote
{
    public static void LFolioNoteAppend(StringBuilder body, string? markdown)
    {
        ArgumentNullException.ThrowIfNull(body);

        int count = 0;
        int last = -1;

        foreach (LMarkdownBlock block in LMarkdown.LMarkdownParse(markdown))
        {
            bool numbered = block.LMarkdownBlockKind == LMarkdownKind.LMarkdownKindNumber;
            count = numbered && block.LMarkdownBlockLevel == last ? count + 1 : 1;
            last = numbered ? block.LMarkdownBlockLevel : -1;

            switch (block.LMarkdownBlockKind)
            {
                case LMarkdownKind.LMarkdownKindHeading:
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"NoteHeading\"/></w:pPr>")
                        .Append(LFolioNoteFormat(block.LMarkdownBlockSpan))
                        .Append("</w:p>");
                    break;
                case LMarkdownKind.LMarkdownKindBullet:
                case LMarkdownKind.LMarkdownKindNumber:
                    string left = (360 * (block.LMarkdownBlockLevel + 1)).ToString(CultureInfo.InvariantCulture);
                    string mark = numbered ? count.ToString(CultureInfo.InvariantCulture) + "." : "•";
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"Note\"/><w:ind w:left=\"").Append(left)
                        .Append("\" w:hanging=\"360\"/></w:pPr><w:r><w:t>").Append(mark)
                        .Append("</w:t></w:r><w:r><w:tab/></w:r>")
                        .Append(LFolioNoteFormat(block.LMarkdownBlockSpan))
                        .Append("</w:p>");
                    break;
                case LMarkdownKind.LMarkdownKindQuote:
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"NoteQuote\"/><w:ind w:left=\"360\"/></w:pPr>")
                        .Append(LFolioNoteFormat(block.LMarkdownBlockSpan))
                        .Append("</w:p>");
                    break;
                case LMarkdownKind.LMarkdownKindCode:
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"NoteCode\"/></w:pPr>")
                        .Append("<w:r><w:t xml:space=\"preserve\">")
                        .Append(LFolioLine.LFolioLineFormat(block.LMarkdownBlockText))
                        .Append("</w:t></w:r></w:p>");
                    break;
                case LMarkdownKind.LMarkdownKindRule:
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"Note\"/><w:pBdr>")
                        .Append("<w:bottom w:val=\"single\" w:sz=\"6\" w:space=\"1\" w:color=\"auto\"/>")
                        .Append("</w:pBdr></w:pPr></w:p>");
                    break;
                default:
                    body.Append("<w:p><w:pPr><w:pStyle w:val=\"Note\"/></w:pPr>")
                        .Append(LFolioNoteFormat(block.LMarkdownBlockSpan))
                        .Append("</w:p>");
                    break;
            }
        }
    }

    private static string LFolioNoteFormat(IReadOnlyList<LMarkdownSpan> spans)
    {
        StringBuilder runs = new StringBuilder();

        foreach (LMarkdownSpan span in spans)
        {
            runs.Append("<w:r><w:rPr>");

            if (span.LMarkdownSpanCode)
            {
                runs.Append("<w:rFonts w:ascii=\"Consolas\" w:hAnsi=\"Consolas\" w:cs=\"Consolas\"/>");
            }

            if (span.LMarkdownSpanBold)
            {
                runs.Append("<w:b/>");
            }

            if (span.LMarkdownSpanItalic)
            {
                runs.Append("<w:i/>");
            }

            if (span.LMarkdownSpanLink.Length > 0)
            {
                runs.Append("<w:u w:val=\"single\"/>");
            }

            runs.Append("</w:rPr><w:t xml:space=\"preserve\">")
                .Append(LFolioLine.LFolioLineFormat(span.LMarkdownSpanText))
                .Append("</w:t></w:r>");

            if (span.LMarkdownSpanLink.Length > 0
                && !string.Equals(span.LMarkdownSpanLink, span.LMarkdownSpanText, StringComparison.Ordinal))
            {
                runs.Append("<w:r><w:t xml:space=\"preserve\"> (")
                    .Append(LFolioLine.LFolioLineFormat(span.LMarkdownSpanLink))
                    .Append(")</w:t></w:r>");
            }
        }

        return runs.ToString();
    }
}
