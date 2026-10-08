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
        CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += CCorpusQuotation.CQuotationPanel.CPanelRowsResonate;
        CCorpusSession = new CSession(
            CCorpusDesk,
            [CCorpusQuotation.CQuotationPanel.LPanelChangeCheck, CCorpusAnthology.CAnthologyPanel.LPanelChangeCheck],
            editor.CEditorDesk,
            () => CCorpusQuotation.CQuotationPanel.CPanelEditing,
            editor.LEditorFinish,
            static () => true,
            LCorpusStoredShow);
        CCorpusSession.CSessionHeld += () =>
            CCorpusTranscriptChanged?.Invoke(
                CCorpusAnthology.LAnthologyTextShow(
                    CCorpusTranscript.LTranscriptRead()
                    ?? CExample.LExampleBlankRead(CCorpusAnthology.CAnthologyPanel.CPanelTallyRead())));
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
            () => LCorpusLeaveConfirm(true),
            CCorpusAnthology.CAnthologyPanel.LPanelChosenRead,
            CCorpusAnthology.CAnthologyPanel.LPanelScribeRestore,
            LCorpusExampleOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CCorpusSession.LSessionChangeCheck, CCorpusSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LCorpusVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LCorpusClose);
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

    public bool CCorpusTranscriptShown => !LCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusExcerptShown => !LCorpusQuotationSide && !CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusDisplayShown => CCorpusQuotation.CQuotationShown;

    public bool CCorpusEditorShown => CCorpusQuotation.CQuotationPanel.CPanelEditing;

    public bool CCorpusExcerptHeld => CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusExcerptBlank => !CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusScribeChecked => CCorpusTranscriptShown || CCorpusEditorShown;

    public bool CCorpusViewerChecked => !CCorpusScribeChecked;

    public bool CCorpusModeEnabled => LCorpusQuotationSide || CCorpusAnthology.CAnthologyPanel.CPanelModeEnabled;

    public bool CCorpusBinEnabled => !LCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusStoreEnabled =>
        CCorpusEditorShown ? CCorpusEditor.CEditorDesk.CDeskStorable : CCorpusDesk.CDeskStorable;

    public bool CCorpusTranscriptEnabled => CCorpusDesk.CDeskRunning;

    public bool CCorpusPressAllowed => CCorpusDisplayShown || LCorpusRowShown;

    public bool CCorpusPortraitAllowed => CCorpusDisplayShown;

    private bool LCorpusQuotationSide => CCorpusQuotation.CQuotationPanel.CPanelModeEnabled;

    private bool LCorpusRowShown => CCorpusExcerptShown && CCorpusExcerptHeld;

    private bool LCorpusRowHeld =>
        CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled || CCorpusQuotation.CQuotationPanel.CPanelBinEnabled;

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
        bool editing = CCorpusScribeChecked;
        LCorpusExampleShow(id, editing);
        if (CCorpusExcerptHeld)
        {
            return;
        }

        if (!CCorpusAnthology.LAnthologyNarrowed)
        {
            return;
        }

        LCorpusQueryClear();
        LCorpusExampleShow(id, editing);
    }

    private void LCorpusExampleShow(long id, bool editing)
    {
        CCorpusQuotation.CQuotationPanel.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(editing);
        CCorpusAnthology.CAnthologyPanel.CPanelRowOpen(id);
    }

    public CMentionOffer? CCorpusMentionFind(string text, int unit)
    {
        return LCorpusLeaveConfirm(true) ? CCorpusAnthology.LAnthologyMentionFind(text, unit) : null;
    }

    public void CCorpusExampleSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        if (!LCorpusLeaveConfirm(false))
        {
            return;
        }

        _cCorpusAtelier.CAtelierNavigation.LNavigationStationAdd();
        LCorpusExampleShow(chosen, editing);
    }

    private void LCorpusQuotationOpen(long id, bool editing)
    {
        if (!CCorpusQuotation.CQuotationPanel.CPanelRowOpen(id))
        {
            return;
        }

        CCorpusDesk.CDeskCancel();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        if (editing)
        {
            CCorpusQuotation.CQuotationPanel.CPanelScribeToggle(true);
        }
    }

    public void CCorpusQuotationSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        if (!LCorpusLeaveConfirm(true))
        {
            return;
        }

        LCorpusQuotationOpen(chosen, editing);
    }

    public void CCorpusExampleCreate()
    {
        if (!LCorpusLeaveConfirm(true))
        {
            return;
        }

        if (LCorpusRowHeld)
        {
            LCorpusQuotationCreate();
            return;
        }

        CCorpusQuotation.CQuotationPanel.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelFreshOpen();
        CCorpusSession.CSessionStart(null);
    }

    public void CCorpusScribeToggle(bool editing)
    {
        if (LCorpusQuotationSide)
        {
            CCorpusQuotation.CQuotationPanel.CPanelScribeToggle(editing);
            if (!LCorpusQuotationSide)
            {
                LCorpusExampleRestore(editing);
            }

            return;
        }

        CCorpusAnthology.CAnthologyPanel.CPanelScribeToggle(editing);
        if (!CCorpusAnthology.CAnthologyPanel.CPanelEditing)
        {
            CCorpusDesk.CDeskCancel();
        }
    }

    private void LCorpusExampleRestore(bool editing)
    {
        if (CCorpusAnthology.CAnthologyChosen is long chosen)
        {
            LCorpusExampleShow(chosen, editing);
            return;
        }

        LCorpusExampleClose();
    }

    private void LCorpusExampleClose()
    {
        CCorpusQuotation.CQuotationPanel.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelEntryClose();
    }

    private void LCorpusClose()
    {
        CCorpusEditor.CEditorClose();
        CCorpusEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    private void LCorpusQueryClear()
    {
        CCorpusAnthology.CAnthologyQuerySet(string.Empty);
        CCorpusAnthology.CAnthologyFilterSet(new CCatalogFilter([]));
        CCorpusQueryCleared?.Invoke();
    }

    private void LCorpusStoredShow(long id)
    {
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        LCorpusExampleOpen(id);
    }

    private void LCorpusQuotationCreate()
    {
        long? chosen = CCorpusAnthology.CAnthologyChosen;
        CCorpusDesk.CDeskCancel();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        CCorpusQuotation.CQuotationPanel.CPanelFreshOpen();
        CCorpusEditor.CEditorDesk.LDeskQuotationStart(chosen);
    }

    internal bool LCorpusLeaveConfirm(bool shown)
    {
        if (!CCorpusSession.LSessionChangeCheck())
        {
            return true;
        }

        if (_cCorpusEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        if (!store)
        {
            return true;
        }

        return shown ? CCorpusSession.LSessionFinish(true) : CCorpusSession.CSessionClose(true);
    }

    internal void LCorpusEntryResonate()
    {
        if (!CCorpusQuotation.CQuotationPanel.CPanelBinEnabled)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        CCorpusQuotation.CQuotationPanel.CPanelDraftResonate();
        if (LCorpusQuotationSide)
        {
            return;
        }

        LCorpusExampleRestore(editing);
    }

    public IReadOnlyList<CCatalogExample> CCorpusRowsRead()
    {
        if (CCorpusAnthology.LAnthologyRowsRead() is not IReadOnlyList<CCatalogExample> rows)
        {
            return [];
        }

        if (LCorpusRowShown && !rows.Any(static row => row.CCatalogExampleChosen))
        {
            LCorpusExampleClose();
        }

        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogExample>>> CCorpusRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cCorpusEnvoy, _cCorpusSettingsPort, "Example.LoadFailed", store, CCorpusRowsRead);

    public void CCorpusExampleDelete()
    {
        if (LCorpusQuotationSide)
        {
            return;
        }

        CCorpusAnthology.CAnthologyPanel.CPanelEntryDelete();
    }

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
            _cCorpusMarshal, CCorpusAnthology.CAnthologyPanel.CPanelRowsResonate, LCorpusEntryResonate);
    }

    private void LCorpusWorkspaceResonate()
    {
        LCorpusExampleClose();
        CCorpusWorkspaceChanged?.Invoke();
    }
}
