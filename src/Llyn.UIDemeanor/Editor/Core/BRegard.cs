using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BRegard
{
    private const string BRegardGap = "   ";

    private readonly BLocalizationCatalog _bRegardCatalog;

    private readonly CDesk _bRegardDesk;

    private readonly CEsteem _bRegardGate;

    private bool _bRegardMarked;

    private CGrasp? _bRegardGrasp;

    private CFrequency? _bRegardFrequency;

    internal BRegard(BLocalizationCatalog catalog, CDesk desk, CEsteem esteem, Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(esteem);
        ArgumentNullException.ThrowIfNull(changed);

        _bRegardCatalog = catalog;
        _bRegardDesk = desk;
        _bRegardGate = esteem;
        Func<Action, Action> refine = read => () =>
        {
            read();
            changed();
        };
        esteem.CEsteemFavoriteChanged += refine(BRegardFavoriteRefine);
        esteem.CEsteemGraspChanged += refine(BRegardGraspRefine);
        esteem.CEsteemFrequencyChanged += refine(BRegardFrequencyRefine);
        desk.CDeskStarted += BRegardFavoriteRefine;
        desk.CDeskStarted += BRegardGraspRefine;
        desk.CDeskStarted += BRegardFrequencyRefine;
    }

    internal void BRegardObserve(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.F6:
                _bRegardGate.CEsteemFavoriteSet(!_bRegardGate.CEsteemFavorite);
                break;
            case ConsoleKey.LeftArrow or ConsoleKey.RightArrow:
                _bRegardGate.CEsteemGraspMove(key.Key == ConsoleKey.LeftArrow ? -1 : 1);
                break;
            case >= ConsoleKey.D1 and <= ConsoleKey.D9:
                _bRegardGate.CEsteemGraspSet(_bRegardGate.CEsteemGraspHit[key.Key - ConsoleKey.D1][^1]);
                break;
        }
    }

    internal IEnumerable<BCanvasText> BRegardDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        string? tone)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        bool stored = _bRegardDesk.CDeskStored;
        string heart = BLecternHeader.BLecternGlyphRead(glyphs, "Favorite", 0, "*");
        string named = _bRegardCatalog.BLocalizationTextRead("Editor.Favorite");
        string favorite = _bRegardMarked && stored
            ? $"{heart} {named}"
            : $"{new string(' ', measure.BCanvasWidthResolve(heart))} {named}";
        int step = _bRegardGrasp?.CGraspStep ?? default;
        string said = _bRegardGrasp?.CGraspLabel ?? string.Empty;
        IReadOnlyList<int> fills = _bRegardGate.CEsteemGraspFill.ElementAtOrDefault(step) ?? [];
        string stars = string.Concat(fills.Select(fill => fill switch
        {
            2 => BLecternHeader.BLecternGlyphRead(glyphs, "Grasp", 0, "#"),
            1 => BLecternHeader.BLecternGlyphRead(glyphs, "Grasp", 1, "+"),
            _ => BLecternHeader.BLecternGlyphRead(glyphs, "Grasp", 2, "."),
        }));
        string grasp = said.Length > 0 ? $"{stars}  {said}" : stars;
        bool unrated = _bRegardGate.CEsteemGraspUnrated.ElementAtOrDefault(step);
        List<(string, string)> pieces =
        [
            (favorite, !stored ? "disabled" : _bRegardMarked ? "favorite" : "muted"),
            (BRegardGap, "text"),
            (grasp, !stored ? "disabled" : unrated ? "muted" : "text"),
        ];
        if (_bRegardFrequency is CFrequency frequency && stored)
        {
            StringBuilder label = new(_bRegardCatalog.BLocalizationTextRead(frequency.CFrequencyKey));
            if (frequency.CFrequencyRanked)
            {
                label.Append(' ')
                    .Insert(label.Length, BLecternHeader.BLecternGlyphRead(glyphs, "Grasp", 0, "#"),
                        frequency.CFrequencyBand)
                    .Insert(label.Length, BLecternHeader.BLecternGlyphRead(glyphs, "Grasp", 2, "."),
                        frequency.CFrequencySpare);
            }

            pieces.Add((BRegardGap, "text"));
            pieces.Add((label.ToString(), frequency.CFrequencyRank switch
            {
                CFrequencyTier.CFrequencyTierRare => "frequency-1",
                CFrequencyTier.CFrequencyTierAdvanced => "frequency-2",
                CFrequencyTier.CFrequencyTierEveryday => "frequency-3",
                CFrequencyTier.CFrequencyTierCore => "frequency-4",
                _ => "muted",
            }));
        }

        return BEditorSight.BEditorSightDraw(measure, place, tone, pieces);
    }

    private void BRegardFavoriteRefine()
    {
        _bRegardMarked = _bRegardGate.CEsteemFavorite;
    }

    private void BRegardGraspRefine()
    {
        _bRegardGrasp = _bRegardGate.CEsteemGraspRead();
    }

    private void BRegardFrequencyRefine()
    {
        _bRegardFrequency = _bRegardGate.CEsteemFrequencyRead(_bRegardCatalog.BLocalizationTextRead("Frequency.Once"));
    }
}
