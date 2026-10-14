using System;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CStencil
{
    private readonly LDeckPort _cStencilPort;

    private readonly CEnvoy _cStencilEnvoy;

    private readonly LSettingsPort _cStencilSettings;

    private readonly Func<long?> _cStencilChosen;

    internal CStencil(CAtelier atelier, CEnvoy envoy, Func<long?> chosen)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(chosen);

        _cStencilPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cStencilEnvoy = envoy;
        _cStencilSettings = atelier.CAtelierSettingsPort;
        _cStencilChosen = chosen;
    }

    public event Action? CStencilChanged;

    public CStencilState CStencilRead()
    {
        if (_cStencilChosen() is not long deck)
        {
            return new CStencilState([]);
        }

        LStencil owned;
        try
        {
            owned = _cStencilPort.LEngineStencilRead(deck);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStencilEnvoy, _cStencilSettings, "Deck.LoadFailed", exception);
            return new CStencilState([]);
        }

        return new CStencilState(owned.LStencilSlots
            .Select(static slot => new CCueCell(
                (int)slot.LStencilSlotColumn, slot.LStencilSlotIndex, slot.LStencilSlotBody))
            .ToList());
    }

    public void CStencilSlotSet(int column, int index, string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineSlotSet(deck, new LStencilSlot((LCueColumn)column, index, body)));
        }
    }

    public void CStencilSave()
    {
        if (_cStencilChosen() is long deck && _cStencilEnvoy.CEnvoyCoinageRead("Coinage.Stencil") is string name)
        {
            LStencilSave(() => _cStencilPort.LEngineStencilSave(deck, name));
        }
    }

    public void CStencilApply(long stencil)
    {
        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineStencilApply(deck, stencil));
        }
    }

    private void LStencilSave(Action save)
    {
        try
        {
            save();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStencilEnvoy, _cStencilSettings, "Stencil.SaveFailed", exception);
            return;
        }

        CStencilChanged?.Invoke();
    }
}
