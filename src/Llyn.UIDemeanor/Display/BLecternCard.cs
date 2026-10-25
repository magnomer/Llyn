using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternCard
{
    private readonly BLocalizationCatalog _bLecternCatalog;

    private readonly CDisplay _bLecternDisplay;

    private readonly Dictionary<string, CLeaf> _bLecternCardFold = new(StringComparer.Ordinal);

    private readonly Dictionary<string, (CLeaf, int)> _bLecternCardSee = new(StringComparer.Ordinal);

    private readonly Dictionary<string, (CLeafChip?, long?)> _bLecternCardChip = new(StringComparer.Ordinal);

    private readonly BLecternMention _bLecternCardMention;

    private CLecternCard _bLecternCardShown = new([], [], false, false);

    internal BLecternCard(BLocalizationCatalog catalog, CDisplay display, BLecternMention mention)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(mention);

        _bLecternCatalog = catalog;
        _bLecternDisplay = display;
        _bLecternCardMention = mention;
    }

    internal (CCompassPart, int)? BLecternCardSought { get; set; }

    internal void BLecternCardRefine()
    {
        _bLecternCardShown = _bLecternDisplay.CDisplayCard.CDisplayCardRead();
    }

    internal void BLecternCardClear()
    {
        _bLecternCardFold.Clear();
        _bLecternCardSee.Clear();
        _bLecternCardChip.Clear();
        _bLecternCardMention.BLecternMentionClear();
    }

    internal IReadOnlyDictionary<(CCompassPart, int?), int> BLecternCardDraw(
        CCompassPart part,
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int width,
        BCanvasWidth measure,
        List<BLecternLine> lines,
        ICollection<int> meanings)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(meanings);

        Dictionary<(CCompassPart, int?), int> places = [];
        CLecternCard shown = _bLecternCardShown;
        Func<CStateWording, string> word = wording => wording.CStateWordingKey is string key
            ? _bLecternCatalog.BLocalizationTextRead(key)
            : wording.CStateWordingText;
        Func<CLeaf, string?> fold = leaf =>
        {
            if (!leaf.CLeafStored)
            {
                return null;
            }

            string item = (leaf.CLeafCollocated ? "FoldCollocation" : "FoldMeaning")
                + leaf.CLeafId.ToString(CultureInfo.InvariantCulture);
            _ = _bLecternCardFold.TryAdd(item, leaf);
            return item;
        };
        if (part == CCompassPart.CCompassPartMeaning && shown.CLecternCardDefined)
        {
            string heading = _bLecternCatalog.BLocalizationTextRead("Display.MeaningPlural");
            lines.Add(new BLecternLine([(string.Empty, "text", null)], 0));
            places[(part, null)] = lines.Count;
            lines.Add(new BLecternLine([(heading, "heading", null)], 0));
            for (int card = 0; card < shown.CLecternCardMeanings.Count; card++)
            {
                places[(part, card)] = lines.Count;
                meanings.Add(lines.Count);
                BLecternLeafDraw(shown.CLecternCardMeanings[card], glyphs, width, measure, word, fold, lines);
            }
        }

        if (part == CCompassPart.CCompassPartCollocation && shown.CLecternCardCollocated)
        {
            string heading = _bLecternCatalog.BLocalizationTextRead("Display.CollocationOther");
            lines.Add(new BLecternLine([(string.Empty, "text", null)], 0));
            places[(part, null)] = lines.Count;
            lines.Add(new BLecternLine([(heading, "heading", null)], 0));
            for (int card = 0; card < shown.CLecternCardOther.Count; card++)
            {
                places[(part, card)] = lines.Count;
                BLecternRowDraw(shown.CLecternCardOther[card], 2, glyphs, width, measure, word, fold, lines);
            }
        }

        return places;
    }

    internal string? BLecternFoldToggle(string item)
    {
        CLeaf leaf = _bLecternCardFold[item];
        _ = _bLecternDisplay.CDisplayCard.CDisplayFoldToggle(leaf.CLeafCollocated, leaf.CLeafId, !leaf.CLeafFolded);
        return item;
    }

    internal bool BLecternFoldCheck(string? item)
    {
        return item is not null && _bLecternCardFold.ContainsKey(item);
    }

    internal string? BLecternSeeOpen(string item)
    {
        (CLeaf leaf, int index) = _bLecternCardSee[item];
        bool opened = _bLecternDisplay.CDisplayCard.CDisplaySeeOpen(leaf.CLeafSee[index].CLeafSeeId);
        string fold = "FoldCollocation" + leaf.CLeafSee[index].CLeafSeeId.ToString(CultureInfo.InvariantCulture);
        return opened && _bLecternCardFold.ContainsKey(fold) ? fold : null;
    }

    internal bool BLecternSeeCheck(string? item)
    {
        return item is not null && _bLecternCardSee.ContainsKey(item);
    }

    internal string? BLecternChipOpen(string item)
    {
        (CLeafChip? chip, long? target) = _bLecternCardChip[item];
        _ = _bLecternDisplay.CDisplayRoute.CDisplayChipOpen(chip, target);
        return null;
    }

    internal bool BLecternChipCheck(string? item)
    {
        return item is not null && _bLecternCardChip.ContainsKey(item);
    }

    private void BLecternLeafDraw(
        CLeaf leaf,
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int width,
        BCanvasWidth measure,
        Func<CStateWording, string> word,
        Func<CLeaf, string?> fold,
        List<BLecternLine> lines)
    {
        bool titled = !leaf.CLeafTitle.CStateWordingMuted;
        string caption = titled ? word(leaf.CLeafTitle)
            : leaf.CLeafFolded ? word(leaf.CLeafMeaning)
            : _bLecternCatalog.BLocalizationTextRead("Display.Meaning");
        string rank = leaf.CLeafPosition.ToString(CultureInfo.CurrentCulture) + " ";
        string mark = BLecternHeader.BLecternGlyphRead(
            glyphs, "Fold", leaf.CLeafFolded ? 1 : 0, leaf.CLeafFolded ? ">" : "v");
        string head = $"{mark} {caption}";
        string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
        int span = width - measure.BCanvasWidthResolve(rank);
        if (measure.BCanvasWidthResolve(head) > span && span > measure.BCanvasWidthResolve(ellipsis))
        {
            head = measure.BCanvasWidthClamp(head, span - measure.BCanvasWidthResolve(ellipsis)) + ellipsis;
        }

        lines.Add(new BLecternLine(
            [
                (rank, "text", null),
                (head, titled ? "heading" : leaf.CLeafFolded ? "text" : "muted", fold(leaf)),
            ],
            0));
        if (leaf.CLeafFolded)
        {
            return;
        }

        if (!leaf.CLeafMeaning.CStateWordingMuted)
        {
            lines.Add(new BLecternLine([(word(leaf.CLeafMeaning), "text", null)], 2));
        }

        BLecternBodyDraw(leaf, 2, glyphs, word, lines);
        if (leaf.CLeafHomed)
        {
            lines.Add(new BLecternLine([(leaf.CLeafHeading, "muted", null)], 2));
            foreach (CLeaf row in leaf.CLeafCollocation)
            {
                BLecternRowDraw(row, 2, glyphs, width, measure, word, fold, lines);
            }
        }

        for (int index = 0; index < leaf.CLeafSee.Count; index++)
        {
            string item = "See" + _bLecternCardSee.Count.ToString(CultureInfo.InvariantCulture);
            _bLecternCardSee.Add(item, (leaf, index));
            lines.Add(new BLecternLine([(leaf.CLeafSee[index].CLeafSeeText, "link", item)], 2));
        }
    }

    private void BLecternRowDraw(
        CLeaf leaf,
        int indent,
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int width,
        BCanvasWidth measure,
        Func<CStateWording, string> word,
        Func<CLeaf, string?> fold,
        List<BLecternLine> lines)
    {
        string mark = BLecternHeader.BLecternGlyphRead(
            glyphs, "Fold", leaf.CLeafFolded ? 1 : 0, leaf.CLeafFolded ? ">" : "v");
        string head = $"{mark} {word(leaf.CLeafPhrase)}";
        List<(string, string, string?)> pieces = [(head, "text", fold(leaf))];
        if (leaf.CLeafFolded)
        {
            List<string> markers = [.. leaf.CLeafFlag];
            if (leaf.CLeafQuoted)
            {
                markers.Add(leaf.CLeafExample);
            }

            if (leaf.CLeafShared)
            {
                markers.Add(leaf.CLeafAlso);
            }

            string tail = markers.Count > 0 ? " " + string.Join(" ", markers) : string.Empty;
            int room = width - indent - measure.BCanvasWidthResolve(head) - measure.BCanvasWidthResolve(tail) - 2;
            if (!leaf.CLeafMeaning.CStateWordingMuted && room > 0)
            {
                string meaning = word(leaf.CLeafMeaning);
                string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
                if (measure.BCanvasWidthResolve(meaning) > room)
                {
                    meaning = measure.BCanvasWidthClamp(
                        meaning, Math.Max(0, room - measure.BCanvasWidthResolve(ellipsis))) + ellipsis;
                }

                pieces.Add(("  " + meaning, "muted", null));
            }

            pieces.Add((tail, "muted", null));
            lines.Add(new BLecternLine(pieces, indent));
            return;
        }

        lines.Add(new BLecternLine(pieces, indent));
        if (!leaf.CLeafMeaning.CStateWordingMuted)
        {
            lines.Add(new BLecternLine([(word(leaf.CLeafMeaning), "text", null)], indent + 2));
        }

        BLecternBodyDraw(leaf, indent + 2, glyphs, word, lines);
    }

    private void BLecternBodyDraw(
        CLeaf leaf,
        int indent,
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        Func<CStateWording, string> word,
        List<BLecternLine> lines)
    {
        List<(string, string, string?)> chips = [];
        Action<string, string, CLeafChip?, long?> chip = (text, role, origin, target) =>
        {
            string? item = origin is { CLeafChipStored: false }
                ? null
                : "Chip" + _bLecternCardChip.Count.ToString(CultureInfo.InvariantCulture);
            if (item is not null)
            {
                _bLecternCardChip.Add(item, (origin, target));
            }

            chips.Add((chips.Count > 0 ? "  " : string.Empty, "text", null));
            chips.Add((text, role, item));
        };

        foreach (CLeafChip held in leaf.CLeafSituation)
        {
            chip($"[{word(held.CLeafChipWording)}]", "situation", held, null);
        }

        foreach (CLeafChip held in leaf.CLeafRegister)
        {
            chip($"{{{word(held.CLeafChipWording)}}}", "register", held, null);
        }

        string arrow = BLecternHeader.BLecternGlyphRead(glyphs, "Arrow", 0, "->");
        foreach (CTranslationTarget target in leaf.CLeafTranslation)
        {
            chip(
                $"{arrow} {target.CTranslationTargetHeadword} {target.CTranslationTargetLanguage}",
                "link",
                null,
                target.CTranslationTargetId);
        }

        if (chips.Count > 0)
        {
            lines.Add(new BLecternLine(chips, indent));
        }

        string bullet = BLecternHeader.BLecternGlyphRead(glyphs, "Dot", 0, "-");
        foreach (CLeafLine sentence in leaf.CLeafSentence)
        {
            List<(string, string, string?)> pieces = [($"{bullet} ", "text", null)];
            if (sentence.CLeafLineHead.Length > 0)
            {
                pieces.Add((sentence.CLeafLineHead + " ", "muted", null));
            }

            pieces.AddRange(_bLecternCardMention.BLecternMentionDraw(sentence));
            if (sentence.CLeafLineCitation.Length > 0)
            {
                pieces.Add(("  " + sentence.CLeafLineCitation, "muted", null));
            }

            lines.Add(new BLecternLine(pieces, indent));
            foreach (CGlossDraft gloss in sentence.CLeafLineGloss
                         .Where(static gloss => !gloss.CGlossDraftWording.CStateWordingMuted))
            {
                lines.Add(new BLecternLine(
                    [($"{gloss.CGlossDraftLanguage}  {word(gloss.CGlossDraftWording)}", "muted", null)],
                    indent + 2));
            }
        }

        chips = [];
        foreach (CLeafChip held in leaf.CLeafTag)
        {
            chip($"#{word(held.CLeafChipWording)}", "link", held, null);
        }

        if (chips.Count > 0)
        {
            lines.Add(new BLecternLine(chips, indent));
        }

        string missing = " " + _bLecternCatalog.BLocalizationTextRead("Display.Missing");
        string picture = BLecternHeader.BLecternGlyphRead(glyphs, "Media", 0, "P");
        foreach (CImageDraft image in leaf.CLeafImage.Where(static image => !image.CImageDraftEmpty))
        {
            string gone = image.CImageDraftAddress is null ? missing : string.Empty;
            lines.Add(new BLecternLine(
                [($"{picture} {image.CImageDraftName}{gone}", gone.Length > 0 ? "muted" : "text", null)],
                indent));
        }

        string film = BLecternHeader.BLecternGlyphRead(glyphs, "Media", 1, "V");
        foreach (CVideoDraft video in leaf.CLeafVideo.Where(static video => !video.CVideoDraftEmpty))
        {
            string gone = video.CVideoDraftScreen is null ? missing : string.Empty;
            string span = video.CVideoDraftSpan.CStateValueText;
            string text = $"{film} {video.CVideoDraftName}{gone}" + (span.Length > 0 ? "  " + span : string.Empty);
            lines.Add(new BLecternLine([(text, gone.Length > 0 ? "muted" : "text", null)], indent));
        }
    }
}
