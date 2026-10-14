using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDeck
{
    private readonly CAtelier _cDeckAtelier;

    private readonly LDeckPort _cDeckPort;

    private readonly CEnvoy _cDeckEnvoy;

    private readonly LSettingsPort _cDeckSettings;

    private long? _cDeckChosen;

    public CDeck(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDeckAtelier = atelier;
        _cDeckPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cDeckEnvoy = envoy;
        _cDeckSettings = atelier.CAtelierSettingsPort;
        CDeckCharter = new CCharter(atelier, envoy);
        CDeckCue = new CCue(atelier, envoy, () => _cDeckChosen);
        CDeckStencil = new CStencil(atelier, envoy, () => _cDeckChosen);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Deck",
            CDeckCharter.LCharterLeaveConfirm,
            () => _cDeckChosen ?? 0,
            static _ => { },
            LDeckOpen);
        atelier.CAtelierWorkspace.CWorkspaceOpened += LDeckRestore;
    }

    public event Action? CDeckChanged;

    public CCharter CDeckCharter { get; }

    public CCue CDeckCue { get; }

    public CStencil CDeckStencil { get; }

    public CDeckRoll CDeckRead()
    {
        (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) listed;
        try
        {
            listed = _cDeckPort.LEngineDeckRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.LoadFailed", exception);
            return new CDeckRoll([], [], true);
        }

        List<CDeckRow> decks = listed.LDeckListed
            .Select(deck => new CDeckRow(
                deck.LDeckId, deck.LDeckFolderId, deck.LDeckName, (int)deck.LDeckGrain, deck.LDeckId == _cDeckChosen))
            .ToList();
        List<CDeckFolder> folders = listed.LDeckFolders
            .Select(static folder => new CDeckFolder(folder.LDeckFolderId, folder.LDeckFolderName))
            .ToList();
        return new CDeckRoll(folders, decks, decks.Count == 0);
    }

    public void CDeckSelect(long deck)
    {
        if (!CDeckCharter.LCharterLeaveConfirm())
        {
            return;
        }

        _cDeckAtelier.CAtelierNavigation.LNavigationStationAdd();
        LDeckOpen(deck);
    }

    public void CDeckCreate(long? folder, int grain)
    {
        if (!CDeckCharter.LCharterLeaveConfirm())
        {
            return;
        }

        if (_cDeckEnvoy.CEnvoyCoinageRead("Coinage.Deck") is not string name)
        {
            return;
        }

        long id;
        try
        {
            id = _cDeckPort.LEngineDeckCreate(name, folder, (LGrain)grain);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.SaveFailed", exception);
            return;
        }

        LDeckOpen(id);
    }

    public void CDeckNameSet(long deck, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        LDeckSave(() => _cDeckPort.LEngineDeckUpdate(deck, name));
    }

    public void CDeckMove(long deck, long? folder)
    {
        LDeckSave(() => _cDeckPort.LEngineDeckMove(deck, folder));
    }

    public void CDeckOrderSet(long? folder, IReadOnlyList<long> decks)
    {
        ArgumentNullException.ThrowIfNull(decks);

        LDeckSave(() => _cDeckPort.LEngineDeckMove(folder, decks));
    }

    public void CDeckDelete(long deck)
    {
        int tally;
        try
        {
            tally = _cDeckPort.LEngineCueRead(deck).Count;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.LoadFailed", exception);
            return;
        }

        bool confirmed = tally > 0
            ? _cDeckEnvoy.CEnvoyConfirm("Deck.DeleteConfirm", "Deck.DeleteCount", tally)
            : _cDeckEnvoy.CEnvoyConfirm("Deck.DeleteConfirm");
        if (!confirmed)
        {
            return;
        }

        try
        {
            _cDeckPort.LEngineDeckDelete(deck);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.SaveFailed", exception);
            return;
        }

        if (_cDeckChosen == deck)
        {
            _cDeckChosen = null;
            CDeckCharter.LCharterClear();
        }

        CDeckChanged?.Invoke();
    }

    public void CDeckFolderCreate()
    {
        if (_cDeckEnvoy.CEnvoyCoinageRead("Coinage.DeckFolder") is string name)
        {
            LDeckSave(() => _cDeckPort.LEngineFolderCreate(name));
        }
    }

    public void CDeckFolderUpdate(long folder, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        LDeckSave(() => _cDeckPort.LEngineFolderUpdate(folder, name));
    }

    public void CDeckFolderDelete(long folder)
    {
        LDeckSave(() => _cDeckPort.LEngineFolderDelete(folder));
    }

    private void LDeckSave(Action save)
    {
        try
        {
            save();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.SaveFailed", exception);
            return;
        }

        CDeckChanged?.Invoke();
    }

    private void LDeckOpen(long deck)
    {
        _cDeckChosen = deck;
        CDeckCharter.LCharterOpen(deck);
        CDeckChanged?.Invoke();
    }

    private void LDeckRestore()
    {
        _cDeckChosen = null;
        CDeckCharter.LCharterClear();
        CDeckChanged?.Invoke();
    }
}
