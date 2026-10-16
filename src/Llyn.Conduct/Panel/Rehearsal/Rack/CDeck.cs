using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDeck
{
    private static readonly LGrain[] CDeckGrains =
    [
        LGrain.LGrainEntry, LGrain.LGrainMeaning, LGrain.LGrainCollocation, LGrain.LGrainExample, LGrain.LGrainPair,
    ];

    private readonly CAtelier _cDeckAtelier;

    private readonly LDeckPort _cDeckPort;

    private readonly CEnvoy _cDeckEnvoy;

    private readonly LSettingsPort _cDeckSettings;

    private long? _cDeckChosen;

    private LGrain _cDeckGrain = CDeckGrains[0];

    private CDeckView _cDeckView = CDeckView.CDeckViewAccession;

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
        CDeckAccession = new CAccession(atelier, envoy, () => _cDeckChosen);
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

    public CAccession CDeckAccession { get; }

    public CDeckView CDeckView => _cDeckView;

    public CDeckRoll CDeckRead()
    {
        (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) listed;
        List<CDeckRow> decks;
        try
        {
            listed = _cDeckPort.LEngineDeckRead();
            decks = listed.LDeckListed
                .Select(deck =>
                {
                    int count = _cDeckPort.LEngineCensusRead(deck.LDeckId).LDeckCensusTotal;
                    return new CDeckRow(
                        deck.LDeckId,
                        deck.LDeckFolderId,
                        deck.LDeckName,
                        deck.LDeckId == _cDeckChosen,
                        count,
                        count == 1 ? "Deck.CountOne" : "Deck.Count");
                })
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.LoadFailed", exception);
            return new CDeckRoll([], [], CDeckGrainRead(), true);
        }

        List<CDeckFolder> folders = listed.LDeckFolders
            .Select(static folder => new CDeckFolder(folder.LDeckFolderId, folder.LDeckFolderName))
            .ToList();
        return new CDeckRoll(folders, decks, CDeckGrainRead(), decks.Count == 0);
    }

    public IReadOnlyList<CDeckGrain> CDeckGrainRead()
    {
        return CDeckGrains
            .Select(grain => new CDeckGrain(LGrainKeyRead(grain), grain == _cDeckGrain))
            .ToList();
    }

    public void CDeckGrainSelect(int index)
    {
        if (index < 0 || index >= CDeckGrains.Length)
        {
            return;
        }

        _cDeckGrain = CDeckGrains[index];
        CDeckChanged?.Invoke();
    }

    public void CDeckViewSet(int view)
    {
        if (!Enum.IsDefined((CDeckView)view))
        {
            return;
        }

        _cDeckView = (CDeckView)view;
        CDeckChanged?.Invoke();
    }

    public IReadOnlyList<CDeckDestination> CDeckDestinationRead(long deck)
    {
        (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) listed;
        try
        {
            listed = _cDeckPort.LEngineDeckRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.LoadFailed", exception);
            return [];
        }

        if (listed.LDeckListed.FirstOrDefault(row => row.LDeckId == deck) is not LDeck own)
        {
            return [];
        }

        List<CDeckDestination> destinations = [];
        if (own.LDeckFolderId is not null)
        {
            destinations.Add(new CDeckDestination(deck, null, "Deck.MoveTop", string.Empty));
        }

        destinations.AddRange(listed.LDeckFolders
            .Where(folder => folder.LDeckFolderId != own.LDeckFolderId)
            .Select(folder => new CDeckDestination(deck, folder.LDeckFolderId, null, folder.LDeckFolderName)));
        return destinations;
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

    public void CDeckCreate(long? folder)
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
            id = _cDeckPort.LEngineDeckCreate(name, folder, _cDeckGrain);
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

    public void CDeckMove(CDeckDestination destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        LDeckSave(() => _cDeckPort.LEngineDeckMove(
            destination.CDeckDestinationDeck, destination.CDeckDestinationFolder));
    }

    public void CDeckOrderMove(long deck, int offset)
    {
        IReadOnlyList<LDeck> listed;
        try
        {
            listed = _cDeckPort.LEngineDeckRead().LDeckListed;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDeckEnvoy, _cDeckSettings, "Deck.LoadFailed", exception);
            return;
        }

        if (listed.FirstOrDefault(row => row.LDeckId == deck) is not LDeck moved)
        {
            return;
        }

        List<long> order = listed
            .Where(row => row.LDeckFolderId == moved.LDeckFolderId)
            .Select(static row => row.LDeckId)
            .ToList();
        int from = order.IndexOf(deck);
        int to = from + offset;
        if (offset == 0 || to < 0 || to >= order.Count)
        {
            return;
        }

        order.RemoveAt(from);
        order.Insert(to, deck);
        LDeckSave(() => _cDeckPort.LEngineDeckMove(moved.LDeckFolderId, order));
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

    internal static string LGrainKeyRead(LGrain grain)
    {
        return grain switch
        {
            LGrain.LGrainEntry => "Grain.Entry",
            LGrain.LGrainMeaning => "Grain.Meaning",
            LGrain.LGrainCollocation => "Grain.Collocation",
            LGrain.LGrainExample => "Grain.Example",
            LGrain.LGrainPair => "Grain.Pair",
            _ => throw new ArgumentOutOfRangeException(nameof(grain), grain, null),
        };
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
