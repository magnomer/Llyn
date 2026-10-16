using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CScoreboard
{
    private readonly CAtelier _cScoreboardAtelier;

    private readonly LDeckPort _cScoreboardPort;

    private readonly CEnvoy _cScoreboardEnvoy;

    private readonly LSettingsPort _cScoreboardSettings;

    private long? _cScoreboardChosen;

    public CScoreboard(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cScoreboardAtelier = atelier;
        _cScoreboardPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cScoreboardEnvoy = envoy;
        _cScoreboardSettings = atelier.CAtelierSettingsPort;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Scoreboard",
            static () => true,
            () => _cScoreboardChosen ?? 0,
            static _ => { },
            LScoreboardOpen);
        atelier.CAtelierWorkspace.CWorkspaceOpened += LScoreboardRestore;
        _ = atelier.LAtelierObserverAttach(CSubject.CSubjectDeck, _ => CScoreboardChanged?.Invoke());
    }

    public event Action? CScoreboardChanged;

    public string CScoreboardEmptyKey => "Scoreboard.Empty";

    public CScoreboardRoll CScoreboardRead()
    {
        (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed) listed;
        try
        {
            listed = _cScoreboardPort.LEngineDeckRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cScoreboardEnvoy, _cScoreboardSettings, "Deck.LoadFailed", exception);
            return new CScoreboardRoll([], []);
        }

        List<CScoreboardRow> decks =
        [
            new CScoreboardRow(null, null, string.Empty, _cScoreboardChosen is null),
            .. listed.LDeckListed.Select(deck => new CScoreboardRow(
                deck.LDeckId, deck.LDeckFolderId, deck.LDeckName, deck.LDeckId == _cScoreboardChosen)),
        ];
        List<CDeckFolder> folders = listed.LDeckFolders
            .Select(static folder => new CDeckFolder(folder.LDeckFolderId, folder.LDeckFolderName))
            .ToList();
        return new CScoreboardRoll(folders, decks);
    }

    public void CScoreboardSelect(long? deck)
    {
        _cScoreboardAtelier.CAtelierNavigation.LNavigationStationAdd();
        LScoreboardOpen(deck ?? 0);
    }

    public CScoreboardCensus? CScoreboardCensusRead()
    {
        LDeckCensus census;
        CultureInfo culture;
        try
        {
            census = _cScoreboardPort.LEngineCensusRead(_cScoreboardChosen);
            culture = _cScoreboardSettings.LEngineCultureRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cScoreboardEnvoy, _cScoreboardSettings, "Deck.LoadFailed", exception);
            return null;
        }

        return new CScoreboardCensus(
            census.LDeckCensusTotal.ToString(culture),
            census.LDeckCensusActive.ToString(culture),
            census.LDeckCensusExcluded.ToString(culture),
            census.LDeckCensusOrphaned.ToString(culture),
            census.LDeckCensusFlagged.ToString(culture),
            census.LDeckCensusSynced is DateTimeOffset synced
                ? synced.ToLocalTime().ToString("g", culture)
                : _cScoreboardSettings.LEngineTextRead("Scoreboard.Unsynced"),
            census.LDeckCensusWeeks
                .Select(week => new CScoreboardWeek(
                    week.LDeckWeekMonday.ToString("M", culture),
                    week.LDeckWeekAdded,
                    week.LDeckWeekAdded.ToString(culture)))
                .ToList(),
            census.LDeckCensusTotal != 0 ? null
            : _cScoreboardChosen is null ? CScoreboardEmptyKey
            : "Scoreboard.Vacant");
    }

    private void LScoreboardOpen(long deck)
    {
        _cScoreboardChosen = deck == 0 ? null : deck;
        CScoreboardChanged?.Invoke();
    }

    private void LScoreboardRestore()
    {
        _cScoreboardChosen = null;
        CScoreboardChanged?.Invoke();
    }
}
