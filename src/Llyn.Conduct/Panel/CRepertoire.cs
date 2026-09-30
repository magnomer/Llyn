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

    private readonly Action<Action> _cRepertoireMarshal;

    private CRepertoire(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cRepertoireAtelier = atelier;
        _cRepertoireEnvoy = envoy;
        _cRepertoireSettingsPort = atelier.CAtelierSettingsPort;
        _cRepertoireMarshal = marshal;
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
            store => CRepertoireSession!.LSessionFinish(store));
        CRepertoireOccurrence = new COccurrence(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            store => CRepertoireSession!.LSessionFinish(store),
            shownSeam);
        CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += CRepertoireOccurrence.COccurrencePanel.CPanelRowsResonate;
        CRepertoireSession = new CSession(
            CRepertoireDesk,
            [CRepertoireOccurrence.COccurrencePanel.LPanelChangeCheck, CRepertoireAtlas.CAtlasPanel.LPanelChangeCheck],
            editor.CEditorDesk,
            () => CRepertoireOccurrence.COccurrencePanel.CPanelEditing,
            editor.LEditorFinish,
            static () => true,
            LRepertoireStoredShow);
        CRepertoireSession.CSessionHeld += () =>
            CRepertoireScenarioChanged?.Invoke(
                new CScenario(LRepertoireScenarioRead() ?? CScenario.LScenarioBlankRead()));
        CRepertoireSession.CSessionChanged += () => CRepertoireChanged?.Invoke();
        CRepertoireSession.CSessionFailed +=
            (key, exception) => CLedger.LLedgerFailureShow(envoy, _cRepertoireSettingsPort, key, exception);
        CRepertoireAtlas.CAtlasPanel.CPanelEdited += id => CRepertoireSession.CSessionStart(id);
        CRepertoireAtlas.CAtlasPanel.CPanelCleared += CRepertoireSession.CSessionCancel;
        CRepertoireAtlas.CAtlasPanel.CPanelDraftChanged +=
            draft => LRepertoireVignetteShow(
                CAtlas.LAtlasSituationRead(draft, atelier.CAtelierMediaPort, atelier.CAtelierEntryPort));
        CRepertoireOccurrence.COccurrencePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CRepertoireOccurrence.COccurrencePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Repertoire",
            () => LRepertoireLeaveConfirm(true),
            CRepertoireAtlas.CAtlasPanel.LPanelChosenRead,
            CRepertoireAtlas.CAtlasPanel.LPanelScribeRestore,
            LRepertoireSituationOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(
            CRepertoireSession.LSessionChangeCheck, CRepertoireSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LRepertoireVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LRepertoireClose);
        CRepertoireDesk.CDeskObserverAttach(marshal, LRepertoireDraftResonate);
        LRepertoireVistaRestore();
    }

    public static CRepertoire CRepertoireCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CRepertoire(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CRepertoireChanged;

    public event Action<CScenario>? CRepertoireScenarioChanged;

    public event Action<CScenario>? CRepertoireDraftChanged;

    public event Action<CSituation>? CRepertoireSituationChanged;

    public event Action? CRepertoireQueryCleared;

    public event Action? CRepertoireWorkspaceChanged;

    public CEditor CRepertoireEditor { get; }

    public CDesk CRepertoireDesk { get; }

    public CSession CRepertoireSession { get; }

    public CAtlas CRepertoireAtlas { get; }

    public COccurrence CRepertoireOccurrence { get; }

    public CImage CRepertoireImage => new(CRepertoireDesk);

    public CVideo CRepertoireVideo => new(CRepertoireDesk);

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

    private LQuillSituation? LRepertoireQuill =>
        !CRepertoireDesk.CDeskFilling && CRepertoireDesk.CDeskTenure is LTenure held ? new LQuillSituation(held) : null;

    private bool LRepertoireOccurrenceSide => CRepertoireOccurrence.COccurrencePanel.CPanelModeEnabled;

    private bool LRepertoireRowShown => CRepertoireVignetteShown && CRepertoireVignetteHeld;

    private bool LRepertoireRowHeld =>
        CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled || CRepertoireOccurrence.COccurrencePanel.CPanelBinEnabled;

    internal CSituationDraft? LRepertoireScenarioRead()
    {
        try
        {
            return CAtlas.LAtlasDraftRead(CRepertoireDesk.CDeskRead(), _cRepertoireAtelier.CAtelierMediaPort);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cRepertoireEnvoy, _cRepertoireSettingsPort, "Situation.HoldFailed", exception);
            return null;
        }
    }

    private void LRepertoireDraftResonate()
    {
        if (LRepertoireScenarioRead() is CSituationDraft situation)
        {
            CRepertoireDraftChanged?.Invoke(new CScenario(situation));
        }
    }

    private void LRepertoireWorkspaceResonate()
    {
        LRepertoireSituationClose();
        CRepertoireWorkspaceChanged?.Invoke();
    }

    private void LRepertoireClose()
    {
        CRepertoireEditor.CEditorClose();
        CRepertoireEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    private void LRepertoireVignetteShow(CSituation? situation)
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

        LRepertoireSituationClose();
    }

    private void LRepertoireSituationClose()
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
        if (!CRepertoireSession.LSessionChangeCheck())
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

        return shown ? CRepertoireSession.LSessionFinish(true) : CRepertoireSession.CSessionClose(true);
    }

    private void LRepertoireEntryResonate()
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

    public IReadOnlyList<CCatalogSituation> CRepertoireRowsRead()
    {
        if (CRepertoireAtlas.LAtlasRowsRead() is not IReadOnlyList<CCatalogSituation> rows)
        {
            return [];
        }

        if (LRepertoireRowShown && !rows.Any(static row => row.CCatalogSituationChosen))
        {
            LRepertoireSituationClose();
        }

        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogSituation>>> CRepertoireRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(_cRepertoireSettingsPort, store, CRepertoireRowsRead);

    public CScenarioLine CRepertoireTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LRepertoireQuill?.LQuillTitleSet(text);
        return CScenario.LScenarioTitleRead(text, false);
    }

    public CScenarioLine CRepertoireKindSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LRepertoireQuill?.LQuillKindSet(text);
        return CScenario.LScenarioKindRead(text, false);
    }

    public CScenarioLine CRepertoireDescriptionSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LRepertoireQuill?.LQuillDescriptionSet(text);
        return CScenario.LScenarioDescriptionRead(text, false);
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

    internal void LRepertoireVistaRestore()
    {
        LVista vista = _cRepertoireAtelier.CAtelierVistaStart(
            "repertoire", CSubject.CSubjectSituation, CCatalogOrder.CCatalogOrderName);
        LVista occurrence = _cRepertoireAtelier.CAtelierVistaStart(
            "occurrence", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CRepertoireAtlas.LAtlasVistaRestore(vista);
        CRepertoireOccurrence.LOccurrenceVistaRestore(vista, occurrence);
        CRepertoireEditor.LEditorVistaRestore(occurrence);
        CRepertoireAtlas.LAtlasObserverAttach(_cRepertoireMarshal, LRepertoireWorkspaceResonate);
        CRepertoireOccurrence.LOccurrenceObserverAttach(
            _cRepertoireMarshal, CRepertoireAtlas.CAtlasPanel.CPanelRowsResonate, LRepertoireEntryResonate);
    }
}
