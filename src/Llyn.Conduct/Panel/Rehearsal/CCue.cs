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

    public void CCueAdd(IReadOnlyList<long> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        if (_cCueChosen() is long deck)
        {
            LCueSave(() => _cCuePort.LEngineCueAdd(deck, entries));
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

    public void CCueSet(long cue, int flag, bool excluded)
    {
        LCueSave(() => _cCuePort.LEngineCueSet(cue, flag, excluded));
    }

    public IReadOnlyList<CCueCell> CCueCellRead(long cue)
    {
        try
        {
            return _cCuePort.LEngineCellRead(cue)
                .Select(static cell => new CCueCell((int)cell.LCueCellColumn, cell.LCueCellIndex, cell.LCueCellValue))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCueEnvoy, _cCueSettings, "Deck.LoadFailed", exception);
            return [];
        }
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
        (string key, IReadOnlyList<string> suffixes) = (LCueColumn)column switch
        {
            LCueColumn.LCueColumnPicture => ("Stencil.Picture", CCuePicture),
            LCueColumn.LCueColumnSound => ("Stencil.Sound", CCueSound),
            _ => throw new ArgumentOutOfRangeException(nameof(column)),
        };

        if (_cCueEnvoy.CEnvoyMediaRead(key, suffixes) is string path)
        {
            CCueCellSet(cue, column, index, path);
        }
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
