using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CRepertoire
{
    private readonly CAtelier _cRepertoireAtelier;

    private readonly CEnvoy _cRepertoireEnvoy;

    private readonly LSettingsPort _cRepertoireSettingsPort;

    private CRepertoire(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cRepertoireAtelier = atelier;
        _cRepertoireEnvoy = envoy;
        _cRepertoireSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CRepertoireEditor = editor;
        CRepertoireDesk = new CDesk(
            atelier.CAtelierDraftPort, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation);
        CRepertoireAtlas = new CAtlas(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            CRepertoireDesk,
            shownSeam,
            envoy,
            store => CRepertoireSession!.CSessionFinish(store));
        CRepertoireOccurrence = new COccurrence(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            envoy,
            editor.CEditorDesk.CDeskChangeCheck,
            store => CRepertoireSession!.CSessionFinish(store),
            shownSeam);
        CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += CRepertoireOccurrence.COccurrencePanel.CPanelRowsResonate;
        CRepertoireSession = new CSession(
            CRepertoireDesk,
            [CRepertoireOccurrence.COccurrencePanel.CPanelChangeCheck, CRepertoireAtlas.CAtlasPanel.CPanelChangeCheck],
            editor.CEditorDesk,
            () => CRepertoireOccurrence.COccurrencePanel.CPanelEditing,
            editor.CEditorFinish,
            static () => true,
            LRepertoireStoredShow);
        CRepertoireSession.CSessionHeld += () => CRepertoireScenarioChanged?.Invoke(CRepertoireScenarioRead());
        CRepertoireSession.CSessionChanged += () => CRepertoireChanged?.Invoke();
        CRepertoireSession.CSessionFailed += envoy.CEnvoyFailureShow;
        CRepertoireAtlas.CAtlasPanel.CPanelEdited += id => CRepertoireSession.CSessionStart(id);
        CRepertoireAtlas.CAtlasPanel.CPanelCleared += CRepertoireSession.CSessionCancel;
        CRepertoireAtlas.CAtlasPanel.CPanelDraftChanged +=
            draft => LRepertoireVignetteShow(CAtlas.LAtlasDraftRead(draft));
        CRepertoireOccurrence.COccurrencePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CRepertoireOccurrence.COccurrencePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Repertoire",
            () => LRepertoireLeaveConfirm(true),
            CRepertoireAtlas.CAtlasPanel.LPanelChosenRead,
            CRepertoireAtlas.CAtlasPanel.LPanelScribeRestore,
            LRepertoireSituationOpen);
    }

    public static CRepertoire CRepertoireCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CRepertoire(atelier, shownSeam, envoy);
    }

    public event Action? CRepertoireChanged;

    public event Action<CSituationDraft?>? CRepertoireScenarioChanged;

    public event Action<CSituationDraft>? CRepertoireSituationChanged;

    public event Action? CRepertoireQueryCleared;

    public CEditor CRepertoireEditor { get; }

    public CDesk CRepertoireDesk { get; }

    public CSession CRepertoireSession { get; }

    public CAtlas CRepertoireAtlas { get; }

    public COccurrence CRepertoireOccurrence { get; }

    public bool CRepertoireScenarioShown =>
        !LRepertoireOccurrenceSide && CRepertoireAtlas.CAtlasPanel.CPanelEditing;

    public bool CRepertoireVignetteShown =>
        !LRepertoireOccurrenceSide && !CRepertoireAtlas.CAtlasPanel.CPanelEditing;

    public bool CRepertoireDisplayShown =>
        LRepertoireOccurrenceSide && !CRepertoireOccurrence.COccurrencePanel.CPanelEditing;

    public bool CRepertoireEditorShown => CRepertoireOccurrence.COccurrencePanel.CPanelEditing;

    public bool CRepertoireVignetteHeld => CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool CRepertoireVignetteBlank => !CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool CRepertoireScribeChecked => CRepertoireScenarioShown || CRepertoireEditorShown;

    public bool CRepertoireViewerChecked => !CRepertoireScribeChecked;

    public bool CRepertoireModeEnabled =>
        LRepertoireOccurrenceSide || CRepertoireAtlas.CAtlasPanel.CPanelModeEnabled;

    public bool CRepertoireBinEnabled =>
        !LRepertoireOccurrenceSide && CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool CRepertoireStoreEnabled =>
        CRepertoireEditorShown ? CRepertoireEditor.CEditorDesk.CDeskStorable : CRepertoireDesk.CDeskChanged;

    public bool CRepertoirePressAllowed => CRepertoireDisplayShown || LRepertoireRowShown;

    public bool CRepertoirePortraitAllowed => CRepertoireDisplayShown;

    private bool LRepertoireOccurrenceSide => CRepertoireOccurrence.COccurrencePanel.CPanelModeEnabled;

    private bool LRepertoireRowShown => CRepertoireVignetteShown && CRepertoireVignetteHeld;

    private bool LRepertoireRowHeld =>
        CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled || CRepertoireOccurrence.COccurrencePanel.CPanelBinEnabled;

    public CSituationDraft? CRepertoireScenarioRead()
    {
        try
        {
            return CAtlas.LAtlasDraftRead(CRepertoireDesk.CDeskRead());
        }
        catch (Exception exception)
        {
            _cRepertoireEnvoy.CEnvoyFailureShow("Situation.HoldFailed", exception);
            return null;
        }
    }

    private void LRepertoireVignetteShow(CSituationDraft? situation)
    {
        if (situation is null)
        {
            return;
        }

        CRepertoireSituationChanged?.Invoke(situation);
    }

    internal void LRepertoireSituationOpen(long id)
    {
        bool editing = CRepertoireScribeChecked;
        LRepertoireSituationShow(id, editing);
        if (CRepertoireVignetteHeld)
        {
            return;
        }

        if (!CRepertoireAtlas.LAtlasNarrowed)
        {
            return;
        }

        LRepertoireQueryClear();
        LRepertoireSituationShow(id, editing);
    }

    private void LRepertoireSituationShow(long id, bool editing)
    {
        CRepertoireOccurrence.COccurrencePanel.CPanelEntryClose();
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(editing);
        CRepertoireAtlas.CAtlasPanel.CPanelRowOpen(id);
    }

    public void CRepertoireSituationSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CRepertoireScribeChecked;
        if (!LRepertoireLeaveConfirm(false))
        {
            return;
        }

        _cRepertoireAtelier.CAtelierNavigation.LNavigationStationAdd();
        LRepertoireSituationShow(chosen, editing);
    }

    private void LRepertoireOccurrenceOpen(long id, bool editing)
    {
        if (!CRepertoireOccurrence.COccurrencePanel.CPanelRowOpen(id))
        {
            return;
        }

        CRepertoireDesk.CDeskCancel();
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        if (editing)
        {
            CRepertoireOccurrence.COccurrencePanel.CPanelScribeToggle(true);
        }
    }

    public void CRepertoireOccurrenceSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CRepertoireScribeChecked;
        if (!LRepertoireLeaveConfirm(true))
        {
            return;
        }

        LRepertoireOccurrenceOpen(chosen, editing);
    }

    public void CRepertoireSituationCreate()
    {
        if (!LRepertoireLeaveConfirm(true))
        {
            return;
        }

        if (LRepertoireRowHeld)
        {
            LRepertoireOccurrenceCreate();
            return;
        }

        CRepertoireOccurrence.COccurrencePanel.CPanelEntryClose();
        CRepertoireAtlas.CAtlasPanel.CPanelFreshOpen();
        CRepertoireSession.CSessionStart(null);
    }

    public void CRepertoireScribeToggle(bool editing)
    {
        if (LRepertoireOccurrenceSide)
        {
            CRepertoireOccurrence.COccurrencePanel.CPanelScribeToggle(editing);
            if (!LRepertoireOccurrenceSide)
            {
                LRepertoireSituationRestore(editing);
            }

            return;
        }

        CRepertoireAtlas.CAtlasPanel.CPanelScribeToggle(editing);
        if (!CRepertoireAtlas.CAtlasPanel.CPanelEditing)
        {
            CRepertoireDesk.CDeskCancel();
        }
    }

    private void LRepertoireSituationRestore(bool editing)
    {
        if (CRepertoireAtlas.CAtlasChosen is long chosen)
        {
            LRepertoireSituationShow(chosen, editing);
            return;
        }

        CRepertoireSituationClose();
    }

    public void CRepertoireSituationClose()
    {
        CRepertoireOccurrence.COccurrencePanel.CPanelEntryClose();
        CRepertoireAtlas.CAtlasPanel.CPanelEntryClose();
    }

    private void LRepertoireQueryClear()
    {
        CRepertoireAtlas.CAtlasQuerySet(string.Empty);
        CRepertoireAtlas.CAtlasFilterSet(new CCatalogFilter([]));
        CRepertoireQueryCleared?.Invoke();
    }

    private void LRepertoireStoredShow(long id)
    {
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        LRepertoireSituationOpen(id);
    }

    private void LRepertoireOccurrenceCreate()
    {
        long? chosen = CRepertoireAtlas.CAtlasChosen;
        CRepertoireDesk.CDeskCancel();
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        CRepertoireOccurrence.COccurrencePanel.CPanelFreshOpen();
        CRepertoireEditor.CEditorDesk.LDeskOccurrenceStart(chosen);
    }

    internal bool LRepertoireLeaveConfirm(bool shown)
    {
        if (!CRepertoireSession.CSessionChangeCheck())
        {
            return true;
        }

        if (_cRepertoireEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        if (!store)
        {
            return true;
        }

        return shown ? CRepertoireSession.CSessionFinish(true) : CRepertoireSession.CSessionClose(true);
    }

    public void CRepertoireEntryResonate()
    {
        if (!CRepertoireOccurrence.COccurrencePanel.CPanelBinEnabled)
        {
            return;
        }

        bool editing = CRepertoireScribeChecked;
        CRepertoireOccurrence.COccurrencePanel.CPanelDraftResonate();
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireSituationRestore(editing);
    }

    public IReadOnlyList<CCatalogSituation> CRepertoireRowsRead(string unknown, string untitled)
    {
        IReadOnlyList<CCatalogSituation> rows = CRepertoireAtlas.LAtlasRowsRead(unknown, untitled);
        if (LRepertoireRowShown && !rows.Any(static row => row.CCatalogSituationChosen))
        {
            CRepertoireSituationClose();
        }

        return rows;
    }

    public void CRepertoireSituationDelete()
    {
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        CRepertoireAtlas.CAtlasPanel.CPanelEntryDelete();
    }

    public Task CRepertoirePortraitPrint()
    {
        if (CRepertoireDisplayShown)
        {
            return CRepertoireOccurrence.LOccurrencePortraitPrint(
                _cRepertoireEnvoy, _cRepertoireSettingsPort);
        }

        if (CRepertoirePressAllowed)
        {
            return CRepertoireAtlas.LAtlasPortraitPrint(_cRepertoireEnvoy, _cRepertoireSettingsPort);
        }

        return Task.CompletedTask;
    }

    public Task CRepertoirePortraitExport()
    {
        if (!CRepertoirePortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return CRepertoireOccurrence.LOccurrencePortraitExport(_cRepertoireEnvoy, _cRepertoireSettingsPort);
    }

    public void CRepertoireVistaRestore()
    {
        LVista vista = _cRepertoireAtelier.CAtelierVistaStart(
            "repertoire", CSubject.CSubjectSituation, CCatalogOrder.CCatalogOrderName);
        LVista occurrence = _cRepertoireAtelier.CAtelierVistaStart(
            "occurrence", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CRepertoireAtlas.LAtlasVistaRestore(vista);
        CRepertoireOccurrence.LOccurrenceVistaRestore(vista, occurrence);
        CRepertoireEditor.LEditorVistaRestore(occurrence);
    }
}
