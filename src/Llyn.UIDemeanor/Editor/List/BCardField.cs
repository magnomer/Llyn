using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BCardField
{
    private const int BCardFieldIndent = 3;

    private static readonly string[] BCardFieldHint = ["Card.DefinitionHint", "Card.ExpressionHint"];

    private readonly BLocalizationCatalog _bCardFieldCatalog;

    private readonly Action<bool, long, string> _bCardFieldGate;

    private readonly Action _bCardFieldKeeper;

    private BField? _bCardFieldEntry;

    internal BCardField(BLocalizationCatalog catalog, Action<bool, long, string> gate, Action keeper)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(keeper);

        _bCardFieldCatalog = catalog;
        _bCardFieldGate = gate;
        _bCardFieldKeeper = keeper;
    }

    internal (bool, long)? BCardFieldCard { get; private set; }

    internal void BCardFieldOpen((bool, long) card, CStateWording wording, string? hint = null)
    {
        ArgumentNullException.ThrowIfNull(wording);

        (bool collocated, long id) = card;
        BCardFieldCard = card;
        _bCardFieldEntry = new BField(
            text => _bCardFieldGate(collocated, id, text),
            wording.CStateWordingText,
            _bCardFieldCatalog,
            hint ?? wording.CStateWordingHint ?? BCardFieldHint[collocated ? 1 : 0]);
    }

    internal bool BCardFieldObserve(ConsoleKeyInfo key)
    {
        return _bCardFieldEntry?.BSParleyBodyObserve(key) ?? false;
    }

    internal void BCardFieldClose()
    {
        if (_bCardFieldEntry is not BField entry)
        {
            return;
        }

        _ = entry.BFieldPendingApply(null);
        BCardFieldReset();
        _bCardFieldKeeper();
    }

    internal void BCardFieldReset()
    {
        _bCardFieldEntry = null;
        BCardFieldCard = null;
    }

    internal IEnumerable<BCanvasText> BCardFieldDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        bool focused)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        (int line, int column, int width) = place;
        return _bCardFieldEntry?.BSParleyBodyDraw(glyphs, line, column, width, 1, focused, "field", measure) ?? [];
    }

    internal IEnumerable<BCanvasText> BCardFieldDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        ((bool, long), CStateWording) card,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(card.Item2);

        (int line, int column, int width) = place;
        ((bool, long) self, CStateWording wording) = card;
        bool collocated = self.Item1;
        (string? tone, bool focused) = look;
        if (BCardFieldCard == self)
        {
            return BCardFieldDraw(glyphs, measure,
                (line, column + BCardFieldIndent, Math.Max(width - BCardFieldIndent, 1)), focused);
        }

        (string, string) shown = wording.CStateWordingMuted
            ? (_bCardFieldCatalog.BLocalizationTextRead(
                wording.CStateWordingHint ?? BCardFieldHint[collocated ? 1 : 0]), "muted")
            : (wording.CStateWordingKey is string key ? _bCardFieldCatalog.BLocalizationTextRead(key)
                : wording.CStateWordingText, "text");
        return BEditorSight.BEditorSightDraw(
            measure, place, tone, [(new string(' ', BCardFieldIndent), "text"), shown]);
    }
}
