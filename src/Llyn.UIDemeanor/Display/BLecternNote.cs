using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternNote
{
    private const int BLecternNoteIndent = 2;

    private readonly BLocalizationCatalog _bLecternNoteCatalog;

    private readonly CAtelier _bLecternNoteAtelier;

    private readonly Dictionary<string, string> _bLecternNoteLink = new(StringComparer.Ordinal);

    internal BLecternNote(BLocalizationCatalog catalog, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);

        _bLecternNoteCatalog = catalog;
        _bLecternNoteAtelier = atelier;
    }

    internal IReadOnlyList<BLecternLine> BLecternNoteDraw(
        CLectern shown, IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, int width, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        _bLecternNoteLink.Clear();
        if (!shown.CLecternNoted || shown.CLecternNote.Count == 0)
        {
            return [];
        }

        string dash = BLecternHeader.BLecternGlyphRead(glyphs, "Rule", 1, "-");
        string bar = BLecternHeader.BLecternGlyphRead(glyphs, "Tree", 2, "|") + " ";
        int room = Math.Max(width - BLecternNoteIndent, 1);
        int count = Math.Max(room / Math.Max(measure.BCanvasWidthResolve(dash), 1), 1);
        BLecternLine rule = new([(string.Concat(Enumerable.Repeat(dash, count)), "muted", null)], BLecternNoteIndent);
        Func<IReadOnlyList<CMarkdownSpan>, string, List<(string, string, string?)>> spans = (parts, plain) =>
        [
            .. parts.Select(span =>
            {
                string? item = null;
                if (span.CMarkdownSpanAddress is not null)
                {
                    item = "Note" + _bLecternNoteLink.Count.ToString(CultureInfo.InvariantCulture);
                    _bLecternNoteLink.Add(item, span.CMarkdownSpanAddress.AbsoluteUri);
                }

                string role = item is not null ? "link"
                    : span.CMarkdownSpanCode ? "code"
                    : span.CMarkdownSpanBold || span.CMarkdownSpanItalic ? "emphasis"
                    : plain;
                return (span.CMarkdownSpanText, role, item);
            }),
        ];
        List<BLecternLine> lines =
        [
            new([(string.Empty, "text", null)], 0),
            new([(_bLecternNoteCatalog.BLocalizationTextRead("Display.Note"), "heading", null)], 0),
        ];
        CMarkdownBlock? before = null;
        foreach (CMarkdownBlock block in shown.CLecternNote)
        {
            if (before is not null && !(before.CMarkdownBlockListed && block.CMarkdownBlockListed))
            {
                lines.Add(new BLecternLine([(string.Empty, "text", null)], 0));
            }

            before = block;
            if (block.CMarkdownBlockHeaded)
            {
                lines.Add(new BLecternLine(spans(block.CMarkdownBlockSpan, "heading"), BLecternNoteIndent));
                if (block.CMarkdownBlockLevel == 1)
                {
                    lines.Add(rule);
                }
            }
            else if (block.CMarkdownBlockListed)
            {
                lines.Add(new BLecternLine(
                    [(block.CMarkdownBlockMark + " ", "muted", null), .. spans(block.CMarkdownBlockSpan, "text")],
                    BLecternNoteIndent + (2 * block.CMarkdownBlockLevel)));
            }
            else if (block.CMarkdownBlockQuoted)
            {
                int inside = Math.Max(room - measure.BCanvasWidthResolve(bar), 1);
                foreach (BLecternLine cut in new BLecternLine(spans(block.CMarkdownBlockSpan, "muted"), 0)
                             .BLecternLineDivide(inside, measure))
                {
                    lines.Add(new BLecternLine([(bar, "muted", null), .. cut.BLecternLinePieces], BLecternNoteIndent));
                }
            }
            else if (block.CMarkdownBlockFenced)
            {
                foreach (string raw in block.CMarkdownBlockText.TrimEnd('\n').Split('\n'))
                {
                    string code = raw.TrimEnd('\r', ' ');
                    string body = code.TrimStart(' ');
                    int leading = Math.Min(code.Length - body.Length, room - 1);
                    int left = room - leading;
                    body = measure.BCanvasWidthResolve(body) > left ? measure.BCanvasWidthClamp(body, left) : body;
                    lines.Add(new BLecternLine([(body, "code", null)], BLecternNoteIndent + leading));
                }
            }
            else if (block.CMarkdownBlockRuled)
            {
                lines.Add(rule);
            }
            else
            {
                lines.Add(new BLecternLine(spans(block.CMarkdownBlockSpan, "text"), BLecternNoteIndent));
            }
        }

        return lines;
    }

    internal string? BLecternNoteOpen(string item)
    {
        _bLecternNoteAtelier.CAtelierLocationOpen(BLecternNoteRead(item)!);
        return null;
    }

    internal bool BLecternNoteCheck(string? item)
    {
        return BLecternNoteRead(item) is not null;
    }

    internal string? BLecternNoteRead(string? item)
    {
        return item is not null && _bLecternNoteLink.TryGetValue(item, out string? address) ? address : null;
    }
}
