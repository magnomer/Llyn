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
        CRepertoirePlaywright = new CPlaywright(atelier, envoy, marshal);
        CRepertoireAtlas = new CAtlas(
            atelier.CAtelierEntryBundle.CEntryBundleSituation,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            CRepertoirePlaywright.LPlaywrightDesk,
            shownSeam,
            envoy,
            store => CRepertoireSession!.LSessionFinish(store));
        CRepertoireOccurrence = new COccurrence(
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            store => CRepertoireSession!.LSessionFinish(store),
            shownSeam);
        CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureRowsChanged +=
            CRepertoireOccurrence.COccurrencePanel.CPanelAperture.CApertureRowsResonate;
        CRepertoireSession = new CSession(
            CRepertoirePlaywright.LPlaywrightDesk,
            [CRepertoireOccurrence.COccurrencePanel.LPanelChangeCheck, CRepertoireAtlas.CAtlasPanel.LPanelChangeCheck],
            editor.CEditorDesk,
            () => CRepertoireOccurrence.COccurrencePanel.CPanelEditing,
            editor.LEditorFinish,
            static () => true,
            LRepertoireStoredShow,
            envoy);
        CRepertoireDiptych = new CDiptych(
            CRepertoireAtlas.CAtlasPanel,
            CRepertoireOccurrence.COccurrencePanel,
            CRepertoireSession,
            atelier.CAtelierNavigation,
            CRepertoireSession.CSessionStart,
            CRepertoirePlaywright.LPlaywrightDesk.CDeskCancel,
            LRepertoireOccurrenceCreate);
        CRepertoireSession.CSessionHeld +=CRepertoirePlaywright.LPlaywrightHeldResonate;
        CRepertoireSession.CSessionChanged += () => CRepertoireChanged?.Invoke();
        CRepertoireAtlas.CAtlasPanel.CPanelEdited += id => CRepertoireSession.CSessionStart(id);
        CRepertoireAtlas.CAtlasPanel.CPanelCleared += CRepertoireSession.CSessionCancel;
        CRepertoireAtlas.CAtlasPanel.CPanelDraftChanged +=
            draft => LRepertoireVignetteShow(
                CAtlas.LAtlasSituationRead(
                    draft, atelier.CAtelierMediaPort, atelier.CAtelierEntryBundle.CEntryBundleMarkdown));
        CRepertoireOccurrence.COccurrencePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CRepertoireOccurrence.COccurrencePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        editor.CEditorDisplay.CDisplayPanelAttach(CRepertoireOccurrence.COccurrencePanel);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Repertoire",
            () => CRepertoireSession.LSessionLeaveConfirm(true),
            CRepertoireAtlas.CAtlasPanel.LPanelChosenRead,
            CRepertoireAtlas.CAtlasPanel.LPanelScribeRestore,
            LRepertoireSituationOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(
            CRepertoireSession.LSessionChangeCheck, CRepertoireSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LRepertoireVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(() =>
        {
            editor.CEditorClose();
            editor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
        });
        LRepertoireVistaRestore();
    }

    public static CRepertoire CRepertoireCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CRepertoire(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CRepertoireChanged;

    public event Action<CSituation>? CRepertoireSituationChanged;

    public event Action? CRepertoireQueryCleared;

    public event Action? CRepertoireWorkspaceChanged;

    public CEditor CRepertoireEditor { get; }

    public CPlaywright CRepertoirePlaywright { get; }

    public CSession CRepertoireSession { get; }

    public CAtlas CRepertoireAtlas { get; }

    public COccurrence CRepertoireOccurrence { get; }

    public CDiptych CRepertoireDiptych { get; }

    public bool CRepertoireVignetteHeld => CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool CRepertoireVignetteBlank => !CRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool CRepertoireStoreEnabled =>
        CRepertoireDiptych.CDiptychChildEditing
            ? CRepertoireEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable
            : CRepertoirePlaywright.LPlaywrightDesk.CDeskDraft.CDeskDraftStorable;

    public bool CRepertoireScenarioEnabled =>
        CRepertoirePlaywright.LPlaywrightDesk.CDeskChronicle.CDeskChronicleRunning;

    public bool CRepertoirePressAllowed => CRepertoireDiptych.CDiptychChildShown || LRepertoireRowShown;

    public bool CRepertoirePortraitAllowed => CRepertoireDiptych.CDiptychChildShown;

    private bool LRepertoireRowShown => CRepertoireDiptych.CDiptychParentShown && CRepertoireVignetteHeld;

    private void LRepertoireWorkspaceResonate()
    {
        CRepertoireDiptych.LDiptychEntryClose();
        CRepertoireWorkspaceChanged?.Invoke();
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
        bool editing = CRepertoireDiptych.CDiptychScribeChecked;
        CRepertoireDiptych.LDiptychParentShow(id, editing);
        if (CRepertoireVignetteHeld)
        {
            return;
        }

        if (!CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureNarrowed)
        {
            return;
        }

        LRepertoireQueryClear();
        CRepertoireDiptych.LDiptychParentShow(id, editing);
    }

    private void LRepertoireQueryClear()
    {
        CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureQuerySet(string.Empty);
        CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter([]));
        CRepertoireQueryCleared?.Invoke();
    }

    private void LRepertoireStoredShow(long id)
    {
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        LRepertoireSituationOpen(id);
    }

    private void LRepertoireOccurrenceCreate()
    {
        long? chosen = CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureChosen;
        CRepertoirePlaywright.LPlaywrightDesk.CDeskCancel();
        CRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        CRepertoireOccurrence.COccurrencePanel.CPanelFreshOpen();
        CRepertoireEditor.CEditorDesk.LDeskRun((drafts, vista) => drafts.LEngineOccurrenceStart(vista, chosen));
    }

    public IReadOnlyList<CCatalogSituation> CRepertoireRowsRead()
    {
        if (CRepertoireAtlas.LAtlasRowsRead() is not IReadOnlyList<CCatalogSituation> rows)
        {
            return [];
        }

        if (LRepertoireRowShown && !rows.Any(static row => row.CCatalogSituationChosen))
        {
            CRepertoireDiptych.LDiptychEntryClose();
        }

        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogSituation>>> CRepertoireRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cRepertoireEnvoy, _cRepertoireSettingsPort, "Situation.LoadFailed", store, CRepertoireRowsRead);

    public Task CRepertoirePortraitPrint()
    {
        if (CRepertoireDiptych.CDiptychChildShown)
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
            _cRepertoireMarshal,
            CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureRowsResonate,
            CRepertoireDiptych.LDiptychChildResonate);
    }
}
