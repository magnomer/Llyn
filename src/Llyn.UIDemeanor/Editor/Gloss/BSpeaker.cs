using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSpeaker
{
    private const int BSpeakerPage = 8;

    private readonly CEntry _bSpeakerGate;

    private readonly Func<Task<IReadOnlyList<string>>> _bSpeakerLanguage;

    private readonly Action _bSpeakerChanged;

    private int _bSpeakerTurn;

    private bool _bSpeakerLoading;

    internal BSpeaker(CEntry entry, Func<Task<IReadOnlyList<string>>> languages, Action changed)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(changed);

        _bSpeakerGate = entry;
        _bSpeakerLanguage = languages;
        _bSpeakerChanged = changed;
    }

    internal BDrop? BSpeakerDrop { get; private set; }

    internal async Task BSpeakerOpen()
    {
        if (_bSpeakerLoading)
        {
            return;
        }

        _bSpeakerLoading = true;
        int turn = _bSpeakerTurn;
        IReadOnlyList<string> names = await _bSpeakerLanguage().ConfigureAwait(true);
        if (turn != _bSpeakerTurn)
        {
            return;
        }

        _bSpeakerLoading = false;
        BSpeakerDrop = new BDrop(
            names, names.ToList().IndexOf(_bSpeakerGate.CEntryLanguage), true, BSpeakerPage);
        _bSpeakerChanged();
    }

    internal void BSpeakerSelect(Action<string> take)
    {
        ArgumentNullException.ThrowIfNull(take);

        BDrop drop = BSpeakerDrop ?? throw new InvalidOperationException("The language list is not open.");
        BSpeakerDrop = null;
        take(drop.BDropLabel[drop.BDropLit]);
    }

    internal void BSpeakerClose()
    {
        _bSpeakerTurn++;
        _bSpeakerLoading = false;
        BSpeakerDrop = null;
    }

    internal bool BSpeakerObserve(ConsoleKeyInfo key)
    {
        return BSpeakerDrop?.BDropSeek(key.KeyChar) ?? false;
    }

    internal IEnumerable<BCanvasText> BSpeakerDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        string? tone)
    {
        ArgumentNullException.ThrowIfNull(measure);

        string arrow = BLecternHeader.BLecternGlyphRead(glyphs, "Order", 1, "v");
        return BEditorSight.BEditorSightDraw(
            measure, place, tone, [($"{_bSpeakerGate.CEntryLanguage} {arrow}", "text")]);
    }
}
