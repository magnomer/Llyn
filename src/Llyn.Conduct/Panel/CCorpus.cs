using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCorpus
{
    private readonly CAtelier _cCorpusAtelier;

    private readonly CEnvoy _cCorpusEnvoy;

    private readonly LSettingsPort _cCorpusSettingsPort;

    private readonly Action<Action> _cCorpusMarshal;

    private CCorpus(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cCorpusAtelier = atelier;
        _cCorpusEnvoy = envoy;
        _cCorpusSettingsPort = atelier.CAtelierSettingsPort;
        _cCorpusMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CCorpusEditor = editor;
        CCorpusDesk = new CDesk(
            atelier.CAtelierDraftPort,
            atelier.CAtelierSettingsPort,
            "Example",
            envoy,
            "Corpus",
            CSubject.CSubjectExample);
        CCorpusAnthology = CAnthology.LAnthologyCreate(
            atelier, CCorpusDesk, shownSeam, envoy, store => CCorpusSession!.LSessionFinish(store));
        CCorpusTranscript = new CTranscript(
            CCorpusDesk, CCorpusAnthology, atelier.CAtelierDraftPort, atelier.CAtelierSettingsPort, envoy);
        CCorpusQuotation = new CQuotation(
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            store => CCorpusSession!.LSessionFinish(store),
            shownSeam);
        CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged +=
            CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureRowsResonate;
        CCorpusSession = new CSession(
            CCorpusDesk,
            [CCorpusQuotation.CQuotationPanel.LPanelChangeCheck, CCorpusAnthology.CAnthologyPanel.LPanelChangeCheck],
            editor,
            () => CCorpusQuotation.CQuotationPanel.CPanelEditing,
            editor.LEditorFinish,
            static () => true,
            LCorpusStoredShow,
            envoy);
        CCorpusDiptych = new CDiptych(
            CCorpusAnthology.CAnthologyPanel,
            CCorpusQuotation.CQuotationPanel,
            CCorpusSession,
            atelier.CAtelierNavigation,
            CCorpusSession.CSessionStart,
            CCorpusDesk.CDeskCancel,
            LCorpusQuotationCreate);
        CCorpusSession.CSessionHeld += () =>
            CCorpusTranscriptChanged?.Invoke(
                CCorpusAnthology.LAnthologyTextShow(
                    CCorpusTranscript.LTranscriptRead()
                    ?? CExample.LExampleBlankRead(
                        CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead())));
        CCorpusSession.CSessionChanged += () => CCorpusChanged?.Invoke();
        CCorpusAnthology.CAnthologyPanel.CPanelEdited += id => CCorpusSession.CSessionStart(id);
        CCorpusAnthology.CAnthologyPanel.CPanelCleared += CCorpusSession.CSessionCancel;
        CCorpusAnthology.CAnthologyPanel.CPanelDraftChanged +=
            draft => LCorpusExampleUpdate(
                CCorpusAnthology.LAnthologyDraftRead(draft));
        CCorpusQuotation.CQuotationPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CCorpusQuotation.CQuotationPanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Corpus",
            () => CCorpusSession.LSessionLeaveConfirm(true),
            CCorpusAnthology.CAnthologyPanel.LPanelChosenRead,
            CCorpusAnthology.CAnthologyPanel.LPanelScribeRestore,
            LCorpusExampleOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CCorpusSession.LSessionChangeCheck, CCorpusSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LCorpusVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(CCorpusSession.LSessionEditorClose);
        CCorpusTranscript.LTranscriptObserverAttach(marshal);
        LCorpusVistaRestore();
    }

    public static CCorpus CCorpusCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CCorpus(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CCorpusChanged;

    public event Action<CExample>? CCorpusTranscriptChanged;

    public event Action<CExample>? CCorpusExampleChanged;

    public event Action? CCorpusQueryCleared;

    public event Action? CCorpusWorkspaceChanged;

    public CEditor CCorpusEditor { get; }

    internal CDesk CCorpusDesk { get; }

    public CSession CCorpusSession { get; }

    public CAnthology CCorpusAnthology { get; }

    public CTranscript CCorpusTranscript { get; }

    public CQuotation CCorpusQuotation { get; }

    public CDiptych CCorpusDiptych { get; }

    public bool CCorpusDisplayShown => CCorpusQuotation.CQuotationShown;

    public bool CCorpusExcerptHeld => CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusExcerptBlank => !CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusStoreEnabled =>
        CCorpusDiptych.CDiptychChildEditing
            ? CCorpusEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable
            : CCorpusDesk.CDeskDraft.CDeskDraftStorable;

    public bool CCorpusTranscriptEnabled => CCorpusDesk.CDeskChronicle.CDeskChronicleRunning;

    public bool CCorpusPressAllowed => CCorpusDisplayShown || LCorpusRowShown;

    public bool CCorpusPortraitAllowed => CCorpusDisplayShown;

    private bool LCorpusRowShown => CCorpusDiptych.CDiptychParentShown && CCorpusExcerptHeld;

    private void LCorpusExampleUpdate(CExample? example)
    {
        if (example is null)
        {
            return;
        }

        CCorpusExampleChanged?.Invoke(example);
    }

    internal void LCorpusExampleOpen(long id)
    {
        bool editing = CCorpusDiptych.CDiptychScribeChecked;
        CCorpusDiptych.LDiptychParentShow(id, editing);
        if (CCorpusExcerptHeld)
        {
            return;
        }

        if (!CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureNarrowed)
        {
            return;
        }

        LCorpusQueryClear();
        CCorpusDiptych.LDiptychParentShow(id, editing);
    }

    public CMentionOffer? CCorpusMentionFind(string text, int unit)
    {
        return CCorpusSession.LSessionLeaveConfirm(true) ? CCorpusAnthology.LAnthologyMentionFind(text, unit) : null;
    }

    private void LCorpusQueryClear()
    {
        CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureQuerySet(string.Empty);
        CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter([]));
        CCorpusQueryCleared?.Invoke();
    }

    private void LCorpusStoredShow(long id)
    {
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        LCorpusExampleOpen(id);
    }

    private void LCorpusQuotationCreate()
    {
        long? chosen = CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureChosen;
        CCorpusDesk.CDeskCancel();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        CCorpusQuotation.CQuotationPanel.CPanelFreshOpen();
        CCorpusEditor.CEditorDesk.LDeskRun((drafts, vista) => drafts.LEngineQuotationStart(vista, chosen));
    }

    public IReadOnlyList<CCatalogExample> CCorpusRowsRead()
    {
        if (CCorpusAnthology.LAnthologyRowsRead() is not IReadOnlyList<CCatalogExample> rows)
        {
            return [];
        }

        if (LCorpusRowShown && !rows.Any(static row => row.CCatalogExampleChosen))
        {
            CCorpusDiptych.LDiptychEntryClose();
        }

        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogExample>>> CCorpusRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cCorpusEnvoy, _cCorpusSettingsPort, "Example.LoadFailed", store, CCorpusRowsRead);

    public Task CCorpusPortraitPrint()
    {
        if (CCorpusDisplayShown)
        {
            return CCorpusQuotation.LQuotationPortraitPrint(_cCorpusEnvoy, _cCorpusSettingsPort);
        }

        if (CCorpusPressAllowed)
        {
            return CCorpusAnthology.LAnthologyPortraitPrint(_cCorpusEnvoy, _cCorpusSettingsPort);
        }

        return Task.CompletedTask;
    }

    internal void LCorpusVistaRestore()
    {
        LVista vista = _cCorpusAtelier.CAtelierVistaStart(
            "corpus", CSubject.CSubjectExample, CCatalogOrder.CCatalogOrderText);
        LVista quotation = _cCorpusAtelier.CAtelierVistaStart(
            "quotation", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CCorpusAnthology.LAnthologyVistaRestore(vista);
        CCorpusQuotation.LQuotationVistaRestore(vista, quotation);
        CCorpusEditor.LEditorVistaRestore(quotation);
        CCorpusAnthology.LAnthologyObserverAttach(_cCorpusMarshal, LCorpusWorkspaceResonate);
        CCorpusQuotation.LQuotationObserverAttach(
            _cCorpusMarshal,
            CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsResonate,
            CCorpusDiptych.LDiptychChildResonate);
    }

    private void LCorpusWorkspaceResonate()
    {
        CCorpusDiptych.LDiptychEntryClose();
        CCorpusWorkspaceChanged?.Invoke();
    }
}
