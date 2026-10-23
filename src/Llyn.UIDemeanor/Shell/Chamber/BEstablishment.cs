using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEstablishment
{
    private readonly BLocalizationCatalog _bEstablishmentCatalog;
    private readonly BBlueprintPart _bEstablishmentPart;
    private CEstablishment? _bEstablishmentHeard;

    internal BEstablishment(BBlueprintPart window, BLocalizationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(catalog);

        _bEstablishmentCatalog = catalog;
        _bEstablishmentPart = BContract.BContractFind(window, "AEstablishment");
    }

    internal void BEstablishmentObserve(CEstablishment establishment)
    {
        ArgumentNullException.ThrowIfNull(establishment);

        _bEstablishmentHeard = establishment;
    }

    internal IReadOnlyList<BCanvasText> BEstablishmentDraw(
        CCourierState? courier, IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        BBlueprintPart part = _bEstablishmentPart;
        (int row, int column, int width) =
            (part.BBlueprintPartRow, part.BBlueprintPartColumn, part.BBlueprintPartWidth);
        string theme = part.BBlueprintPartTheme;
        if (_bEstablishmentHeard is not CEstablishment establishment || courier is not CCourierState state)
        {
            return [new(row, column, width, _bEstablishmentCatalog.BLocalizationTextRead("Window.Opening"), theme)];
        }

        Func<string, int> cells = text => measure.BCanvasWidthResolve(text);
        Func<string, object, string> format = (key, value) => string.Format(
            CultureInfo.CurrentCulture, _bEstablishmentCatalog.BLocalizationTextRead(key), value);
        string dot = glyphs.GetValueOrDefault("Dot") is [string middle, ..] ? middle : "·";
        string ellipsis = glyphs.GetValueOrDefault("Ellipsis") is [string trail, ..] ? trail : "…";
        string pending = glyphs.GetValueOrDefault("Pending") is [string mark, ..] ? mark : "●";

        string joplin = format(
            "Establishment.Courier",
            _bEstablishmentCatalog.BLocalizationTextRead(
                state.CCourierStateAttached ? "Courier.Connected" : "Courier.Attach"));
        string unsaved = format("Establishment.Unsaved", establishment.CEstablishmentUnsaved);
        string entry = format(establishment.CEstablishmentEntryKey, establishment.CEstablishmentEntry);
        string size = format(establishment.CEstablishmentSizeKey, establishment.CEstablishmentAmount);
        string tally = $"{entry} {dot} {size}";

        int right = column + width - 1 - cells(tally);
        int at = column + cells(joplin);
        List<BCanvasText> texts =
        [
            new(row, column, width, new string(' ', width), theme),
            new(row, right, cells(tally), tally, "muted"),
            new(row, column, cells(joplin), joplin, theme),
        ];

        int marked = establishment.CEstablishmentPending ? 2 + cells(pending) + 1 + cells(unsaved) : 0;
        const string separator = " / ";
        int room = right - 2 - marked - at - separator.Length;
        string line = state.CCourierStateLine;
        if (line.Length > 0)
        {
            string cut = room > cells(ellipsis)
                ? measure.BCanvasWidthDivide(line, room - cells(ellipsis), string.Empty)[0]
                : string.Empty;
            string shown = cells(line) <= room ? line
                : cells(cut) <= room - cells(ellipsis) ? cut + ellipsis
                : ellipsis;
            texts.Add(new(row, at, separator.Length, separator, "muted"));
            at += separator.Length;
            texts.Add(new(row, at, cells(shown), shown, "muted"));
            at += cells(shown);
        }

        if (establishment.CEstablishmentPending)
        {
            texts.Add(new(row, at + 2, cells(pending), pending, "pending"));
            texts.Add(new(row, at + 2 + cells(pending) + 1, cells(unsaved), unsaved, "muted"));
        }

        return texts;
    }
}
