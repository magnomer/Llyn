using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCue
{
    private static readonly IReadOnlyList<string> CCuePicture =
        [".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".tif", ".tiff"];

    private static readonly IReadOnlyList<string> CCueSound = [".mp3", ".m4a", ".aac", ".ogg", ".wav"];

    private readonly LDeckPort _cCuePort;

    private readonly CEnvoy _cCueEnvoy;

    private readonly LSettingsPort _cCueSettings;

    private readonly Func<long?> _cCueChosen;

    internal CCue(CAtelier atelier, CEnvoy envoy, Func<long?> chosen)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(chosen);

        _cCuePort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cCueEnvoy = envoy;
        _cCueSettings = atelier.CAtelierSettingsPort;
        _cCueChosen = chosen;
    }

    public event Action? CCueChanged;

    public IReadOnlyList<CCueRow> CCueRead()
    {
        if (_cCueChosen() is not long deck)
        {
            return [];
        }

        try
        {
            return _cCuePort.LEngineLineRead(deck, CPortrait.LPortraitLabelRead(_cCueSettings))
                .Select(static line => new CCueRow(
                    line.LCueStored.LCueId,
                    line.LCueStored.LCuePosition,
                    line.LCueStored.LCueFlag != 0,
                    line.LCueStored.LCueExcluded,
                    line.LCueStored.LCueOrphaned,
                    line.LCueLine))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCueEnvoy, _cCueSettings, "Deck.LoadFailed", exception);
            return [];
        }
    }

    public void CCueCreate()
    {
        if (_cCueChosen() is long deck)
        {
            LCueSave(() => _cCuePort.LEngineCueCreate(deck, []));
        }
    }

    public void CCueRemove(IReadOnlyList<long> cues)
    {
        ArgumentNullException.ThrowIfNull(cues);

        if (cues.Count > 0 && _cCueEnvoy.CEnvoyConfirm("Cue.RemoveConfirm"))
        {
            LCueSave(() => _cCuePort.LEngineCueRemove(cues));
        }
    }

    public void CCueMove(long cue, int position)
    {
        if (_cCueChosen() is long deck)
        {
            LCueSave(() => _cCuePort.LEngineCueMove(deck, cue, position));
        }
    }

    public void CCueSet(long cue, bool flag, bool excluded)
    {
        LCueSave(() => _cCuePort.LEngineCueSet(cue, Convert.ToInt32(flag), excluded));
    }

    public IReadOnlyList<CCueCell>? CCueCellRead(IReadOnlyList<long> cues)
    {
        ArgumentNullException.ThrowIfNull(cues);

        if (cues is not [long cue] || _cCueChosen() is not long deck)
        {
            return null;
        }

        IReadOnlyList<LCueCell> stored;
        try
        {
            if (_cCuePort.LEngineCueRead(deck).FirstOrDefault(found => found.LCueId == cue)
                is not { LCueGrain: LGrain.LGrainFree })
            {
                return null;
            }

            stored = _cCuePort.LEngineCellRead(cue);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCueEnvoy, _cCueSettings, "Deck.LoadFailed", exception);
            return null;
        }

        return CStencil.CStencilOffer
            .Select(offer => new CCueCell(
                offer.CStencilSlotColumn,
                offer.CStencilSlotIndex,
                stored
                    .FirstOrDefault(cell => (int)cell.LCueCellColumn == offer.CStencilSlotColumn
                        && cell.LCueCellIndex == offer.CStencilSlotIndex)?
                    .LCueCellValue ?? string.Empty,
                offer.CStencilSlotCaption,
                offer.CStencilSlotNumber,
                LCueMediaRead((LCueColumn)offer.CStencilSlotColumn) is not null))
            .ToList();
    }

    public void CCueCellSet(long cue, int column, int index, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        LCueSave(() =>
        {
            LCueCell written = new((LCueColumn)column, index, value);
            List<LCueCell> cells = _cCuePort.LEngineCellRead(cue)
                .Where(cell => cell.LCueCellColumn != written.LCueCellColumn || cell.LCueCellIndex != index)
                .Append(written)
                .ToList();
            _cCuePort.LEngineCellSave(cue, cells);
        });
    }

    public void CCueCellOpen(long cue, int column, int index)
    {
        (string key, IReadOnlyList<string> suffixes) = LCueMediaRead((LCueColumn)column)
            ?? throw new ArgumentOutOfRangeException(nameof(column));

        if (_cCueEnvoy.CEnvoyMediaRead(key, suffixes) is string path)
        {
            CCueCellSet(cue, column, index, path);
        }
    }

    private static (string LCueMediaKey, IReadOnlyList<string> LCueMediaSuffixes)? LCueMediaRead(LCueColumn column)
    {
        return column switch
        {
            LCueColumn.LCueColumnPicture => ("Stencil.Picture", CCuePicture),
            LCueColumn.LCueColumnSound => ("Stencil.Sound", CCueSound),
            _ => null,
        };
    }

    private void LCueSave(Action save)
    {
        try
        {
            save();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCueEnvoy, _cCueSettings, "Deck.SaveFailed", exception);
            return;
        }

        CCueChanged?.Invoke();
    }
}
