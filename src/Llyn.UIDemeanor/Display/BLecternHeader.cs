using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternHeader
{
    internal const string BLecternItemFavorite = "Favorite";

    internal const string BLecternItemGrasp = "Grasp";

    internal const string BLecternItemFrequency = "Frequency";

    private const string BLecternGap = "   ";

    private readonly BLocalizationCatalog _bLecternCatalog;

    private readonly CDisplay _bLecternDisplay;

    private readonly Dictionary<string, CGlyphCell> _bLecternHeaderGlyph = new(StringComparer.Ordinal);

    private CLecternGlyph? _bLecternGlyph;

    private bool _bLecternMarked;

    private CGrasp? _bLecternGrasp;

    private CFrequency? _bLecternFrequency;

    internal BLecternHeader(BLocalizationCatalog catalog, CDisplay display, Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(changed);

        _bLecternCatalog = catalog;
        _bLecternDisplay = display;
        Func<Action, Action> refine = run => () =>
        {
            run();
            changed();
        };
        display.CDisplayOpened += refine(BLecternFavoriteRefine);
        display.CDisplayOpened += refine(BLecternGraspRefine);
        display.CDisplayOpened += refine(BLecternFrequencyRefine);
        display.CDisplayClosed += refine(BLecternFavoriteRefine);
        display.CDisplayClosed += refine(BLecternGraspRefine);
        display.CDisplayClosed += refine(BLecternFrequencyRefine);
        display.CDisplayFrequencyChanged += _ => refine(BLecternFrequencyRefine)();
        display.CDisplayGrasp.CDisplayFavoriteChanged += _ => refine(BLecternFavoriteRefine)();
        display.CDisplayGrasp.CDisplayGraspChanged += _ => refine(BLecternGraspRefine)();
    }

    internal void BLecternFavoriteToggle()
    {
        _bLecternMarked = _bLecternDisplay.CDisplayGrasp.CDisplayFavoriteToggle();
    }

    internal void BLecternFavoriteRefine()
    {
        _bLecternMarked = _bLecternDisplay.CDisplayGrasp.CDisplayFavoriteRead();
    }

    internal void BLecternGraspRefine()
    {
        _bLecternGrasp = _bLecternDisplay.CDisplayGrasp.CDisplayGraspRead();
    }

    internal void BLecternGraspMove(int step)
    {
        _bLecternGrasp = _bLecternDisplay.CDisplayGrasp.CDisplayGraspMove(step);
    }

    internal void BLecternFrequencyRefine()
    {
        _bLecternFrequency = _bLecternDisplay.CDisplayFrequencyRead(_bLecternCatalog.BLocalizationTextRead);
    }

    internal void BLecternGlyphRefine()
    {
        _bLecternGlyph = _bLecternDisplay.CDisplaySound.CDisplayGlyphRead();
    }

    internal IReadOnlyList<BLecternLine> BLecternHeaderBuild(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, int width, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        bool marked = _bLecternMarked;
        CLectern shown = _bLecternDisplay.CDisplayShown;
        string heart = BLecternGlyphRead(glyphs, "Favorite", marked ? 0 : 1, marked ? "*" : "-");
        int room = width - measure.BCanvasWidthResolve(shown.CLecternHeadword) - measure.BCanvasWidthResolve(heart);
        BLecternLine headword = new(
            [
                (shown.CLecternHeadword, "heading", null),
                (new string(' ', Math.Max(1, room)), "text", null),
                (heart, marked ? "favorite" : "muted", BLecternItemFavorite),
            ],
            0);

        return
        [
            headword,
            BLecternSpeechBuild(_bLecternFrequency, glyphs, shown),
            BLecternGraspBuild(_bLecternGrasp, glyphs),
        ];
    }

    internal IReadOnlyList<BLecternLine> BLecternStampBuild()
    {
        CLectern shown = _bLecternDisplay.CDisplayShown;
        if (!shown.CLecternStamped)
        {
            return [];
        }

        string created = _bLecternCatalog.BLocalizationTextRead("Display.Created");
        string updated = _bLecternCatalog.BLocalizationTextRead("Display.Updated");
        return
        [
            new BLecternLine([(string.Empty, "muted", null)], 0),
            new BLecternLine([($"{created}  {shown.CLecternAdded}", "muted", null)], 0),
            new BLecternLine([($"{updated}  {shown.CLecternUpdated}", "muted", null)], 0),
        ];
    }

    internal string BLecternFootRead(string? item)
    {
        return item == BLecternItemFrequency && _bLecternFrequency is { } frequency
            ? string.Join(BLecternGap, frequency.CFrequencySource.Split('\n'))
            : string.Empty;
    }

    private BLecternLine BLecternSpeechBuild(
        CFrequency? frequency, IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, CLectern shown)
    {
        List<string> words = [shown.CLecternLanguage];
        if (shown.CLecternMarked)
        {
            words.AddRange(shown.CLecternSpeeches);
            words.AddRange(
                new[] { shown.CLecternUnit, shown.CLecternFreedom }
                    .Where(static key => key.Length > 0)
                    .Select(_bLecternCatalog.BLocalizationTextRead));
        }

        List<(string BLecternLineText, string BLecternLineRole, string? BLecternLineItem)> pieces =
            [(string.Join(BLecternGap, words.Where(static word => word.Length > 0)), "text", null)];
        if (frequency is not null)
        {
            StringBuilder label = new(_bLecternCatalog.BLocalizationTextRead(frequency.CFrequencyKey));
            if (frequency.CFrequencyRanked)
            {
                label.Append(' ')
                    .Insert(label.Length, BLecternGlyphRead(glyphs, "Grasp", 0, "#"), frequency.CFrequencyBand)
                    .Insert(label.Length, BLecternGlyphRead(glyphs, "Grasp", 2, "."), frequency.CFrequencySpare);
            }

            string role = frequency.CFrequencyRank switch
            {
                CFrequencyTier.CFrequencyTierRare => "frequency-1",
                CFrequencyTier.CFrequencyTierAdvanced => "frequency-2",
                CFrequencyTier.CFrequencyTierEveryday => "frequency-3",
                CFrequencyTier.CFrequencyTierCore => "frequency-4",
                _ => "muted",
            };
            pieces.Add((pieces[0].BLecternLineText.Length > 0 ? BLecternGap : string.Empty, "text", null));
            pieces.Add((label.ToString(), role, BLecternItemFrequency));
        }

        return new BLecternLine(pieces, 0);
    }

    private BLecternLine BLecternGraspBuild(
        CGrasp? grasp, IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs)
    {
        CDisplayGrasp area = _bLecternDisplay.CDisplayGrasp;
        int step = grasp?.CGraspStep ?? default;
        IReadOnlyList<int> fills = area.CDisplayGraspFill.ElementAtOrDefault(step) ?? [];
        string stars = string.Concat(fills.Select(fill => fill switch
        {
            2 => BLecternGlyphRead(glyphs, "Grasp", 0, "#"),
            1 => BLecternGlyphRead(glyphs, "Grasp", 1, "+"),
            _ => BLecternGlyphRead(glyphs, "Grasp", 2, "."),
        }));
        bool unrated = area.CDisplayGraspUnrated.ElementAtOrDefault(step);
        return new BLecternLine(
            [($"{stars}  {grasp?.CGraspLabel}", unrated ? "muted" : "text", BLecternItemGrasp)],
            0);
    }

    internal IReadOnlyList<BLecternLine> BLecternGlyphBuild()
    {
        _bLecternHeaderGlyph.Clear();
        if (_bLecternGlyph is not { CLecternGlyphShown: true } glyph)
        {
            return [];
        }

        string label = _bLecternCatalog.BLocalizationTextRead(glyph.CLecternGlyphKey);
        List<(string, string, string?)> pieces =
            [(label == glyph.CLecternGlyphKey ? glyph.CLecternGlyphName : label, "muted", null)];
        foreach (CGlyphCell cell in glyph.CLecternGlyphCells)
        {
            string? item = null;
            if (cell.CGlyphCellLinked)
            {
                item = "Glyph" + _bLecternHeaderGlyph.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
                _bLecternHeaderGlyph.Add(item, cell);
            }

            pieces.Add(("  ", "text", null));
            pieces.Add((cell.CGlyphCellText, cell.CGlyphCellLinked ? "cjk" : "muted", item));
        }

        return [new BLecternLine(pieces, 0)];
    }

    internal string? BLecternGlyphOpen(string item)
    {
        CGlyphCell cell = _bLecternHeaderGlyph[item];
        _ = _bLecternDisplay.CDisplaySound.CDisplayGlyphOpen(cell.CGlyphCellText, cell.CGlyphCellLanguage);
        return null;
    }

    internal bool BLecternGlyphCheck(string? item)
    {
        return item is not null && _bLecternHeaderGlyph.ContainsKey(item);
    }

    internal static string BLecternGlyphRead(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, string set, int index, string fallback)
    {
        return glyphs.GetValueOrDefault(set) is { } marks && index < marks.Count ? marks[index] : fallback;
    }
}
