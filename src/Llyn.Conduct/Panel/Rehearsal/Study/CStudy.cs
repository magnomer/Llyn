using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CStudy
{
    private readonly CAtelier _cStudyAtelier;

    private readonly LDeckPort _cStudyPort;

    private readonly CEnvoy _cStudyEnvoy;

    private readonly LSettingsPort _cStudySettings;

    private readonly Func<int, int> _cStudyChance;

    private long? _cStudyChosen;

    private CSitting? _cStudySitting;

    private int _cStudyTicket;

    public CStudy(CAtelier atelier, CEnvoy envoy, Func<int, int> chance)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(chance);

        _cStudyAtelier = atelier;
        _cStudyPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cStudyEnvoy = envoy;
        _cStudySettings = atelier.CAtelierSettingsPort;
        _cStudyChance = chance;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Study",
            static () => true,
            () => _cStudyChosen ?? 0,
            static _ => { },
            LStudyOpen);
        atelier.CAtelierWorkspace.CWorkspaceOpened += LStudyRestore;
        _ = atelier.LAtelierObserverAttach(CSubject.CSubjectDeck, _ => CStudyChanged?.Invoke());
    }

    public event Action? CStudyChanged;

    public string CStudyEmptyKey => "Study.Empty";

    public CSitting? CStudySitting => _cStudySitting;

    public CStudyRoll CStudyRead()
    {
        (IReadOnlyList<LDeckFolder> LDeckFolders, IReadOnlyList<LDeck> LDeckListed,
            IReadOnlyDictionary<long, int> LDeckActive) listed;
        try
        {
            listed = _cStudyPort.LEngineActiveRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStudyEnvoy, _cStudySettings, "Deck.LoadFailed", exception);
            return new CStudyRoll([], [], CStudyEmptyKey, _cStudySitting?.CSittingRead());
        }

        List<CStudyRow> decks = listed.LDeckListed
            .Select(deck => new CStudyRow(
                deck.LDeckId,
                deck.LDeckFolderId,
                deck.LDeckName,
                listed.LDeckActive.GetValueOrDefault(deck.LDeckId),
                deck.LDeckId == _cStudyChosen))
            .ToList();
        List<CDeckFolder> folders = listed.LDeckFolders
            .Select(static folder => new CDeckFolder(folder.LDeckFolderId, folder.LDeckFolderName))
            .ToList();
        string? empty = decks.Count == 0 ? CStudyEmptyKey
            : decks.FirstOrDefault(static deck => deck.CStudyRowChosen) is not CStudyRow chosen ? "Study.Unchosen"
            : chosen.CStudyRowActive == 0 ? "Study.Inactive"
            : null;
        return new CStudyRoll(folders, decks, empty, _cStudySitting?.CSittingRead());
    }

    public void CStudySelect(long deck)
    {
        _cStudyAtelier.CAtelierNavigation.LNavigationStationAdd();
        LStudyOpen(deck);
    }

    public void CStudyStart()
    {
        if (_cStudyChosen is not long deck)
        {
            return;
        }

        List<(CSittingSide, CSittingSide)> cards;
        try
        {
            cards = _cStudyPort.LEngineFaceRead(deck, CPortrait.LPortraitLabelRead(_cStudySettings))
                .Select(face => (LStudySideRead(face.LCueFront), LStudySideRead(face.LCueBack)))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStudyEnvoy, _cStudySettings, "Study.LoadFailed", exception);
            return;
        }

        if (cards.Count == 0)
        {
            return;
        }

        LStudySoundStop();
        _cStudySitting = new CSitting(cards, _cStudyChance, LStudySoundStop);
        CStudyChanged?.Invoke();
    }

    public void CStudySoundPlay(bool back)
    {
        if (_cStudySitting?.CSittingRead() is not { CSittingStateEnd: false } state
            || (back && !state.CSittingStateTurned)
            || (back ? state.CSittingStateBack : state.CSittingStateFront) is not { CSittingSideAudible: true } side)
        {
            return;
        }

        try
        {
            _cStudyTicket = _cStudyAtelier.CAtelierMediaPort.LEngineRecordingPlay(side.CSittingSideSounds, double.NaN);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStudyEnvoy, _cStudySettings, "Sound.PlayFailed", exception);
        }
    }

    private CSittingSide LStudySideRead(LCueSide side)
    {
        LMediaPort media = _cStudyAtelier.CAtelierMediaPort;
        List<CImageDraft> pictures = [];
        foreach (string location in side.LCueSidePictures)
        {
            if (media.LEngineLocationRead(location) is Uri address)
            {
                pictures.Add(new CImageDraft(pictures.Count, CStateValue.CStateValueEmpty, false, address));
            }
        }

        return new CSittingSide(side.LCueSideText, pictures) { CSittingSideSounds = side.LCueSideSounds };
    }

    public void CStudyFinish()
    {
        if (_cStudySitting is null)
        {
            return;
        }

        LStudySoundStop();
        _cStudySitting = null;
        CStudyChanged?.Invoke();
    }

    private void LStudyOpen(long deck)
    {
        LStudySoundStop();
        _cStudyChosen = deck;
        _cStudySitting = null;
        CStudyChanged?.Invoke();
    }

    private void LStudyRestore()
    {
        LStudySoundStop();
        _cStudyChosen = null;
        _cStudySitting = null;
        CStudyChanged?.Invoke();
    }

    private void LStudySoundStop()
    {
        _cStudyAtelier.CAtelierMediaPort.LEngineRecordingStop(_cStudyTicket);
    }
}
