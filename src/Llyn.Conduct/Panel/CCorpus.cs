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
        CCorpusDesk = new CDesk(atelier.CAtelierDraftPort, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        CCorpusAnthology = CAnthology.LAnthologyCreate(
            atelier, CCorpusDesk, shownSeam, envoy, store => CCorpusSession!.LSessionFinish(store));
        CCorpusQuotation = new CQuotation(
            atelier.CAtelierEntryPort,
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
                LCorpusTranscriptRead()
                ?? CExample.LExampleBlankRead(CCorpusAnthology.CAnthologyPanel.CPanelTallyRead()));
        CCorpusSession.CSessionChanged += () => CCorpusChanged?.Invoke();
        CCorpusSession.CSessionFailed +=
            (key, exception) => CLedger.LLedgerFailureShow(envoy, _cCorpusSettingsPort, key, exception);
        CCorpusAnthology.CAnthologyPanel.CPanelEdited += id => CCorpusSession.CSessionStart(id);
        CCorpusAnthology.CAnthologyPanel.CPanelCleared += CCorpusSession.CSessionCancel;
        CCorpusAnthology.CAnthologyPanel.CPanelDraftChanged +=
            draft => LCorpusExampleUpdate(
                CCorpusAnthology.LAnthologyDraftRead(draft, CCorpusAnthology.CAnthologyPanel.CPanelTallyRead()));
        CCorpusQuotation.CQuotationPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CCorpusQuotation.CQuotationPanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Corpus",
            () => LCorpusLeaveConfirm(true),
            CCorpusAnthology.CAnthologyPanel.LPanelChosenRead,
            CCorpusAnthology.CAnthologyPanel.LPanelScribeRestore,
            LCorpusExampleOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CCorpusSession.LSessionChangeCheck, CCorpusSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(CCorpusVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LCorpusClose);
        CCorpusDesk.CDeskObserverAttach(marshal, LCorpusDraftResonate);
    }

    public static CCorpus CCorpusCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CCorpus(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CCorpusChanged;

    public event Action<CExample>? CCorpusTranscriptChanged;

    public event Action<CExample>? CCorpusDraftChanged;

    public event Action<CExample>? CCorpusExampleChanged;

    public event Action? CCorpusQueryCleared;

    public event Action? CCorpusWorkspaceChanged;

    public event Action<CProspect>? CCorpusMentionOffered;

    public CEditor CCorpusEditor { get; }

    public CDesk CCorpusDesk { get; }

    public CSession CCorpusSession { get; }

    public CAnthology CCorpusAnthology { get; }

    public CQuotation CCorpusQuotation { get; }

    public bool CCorpusTranscriptShown => !LCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusExcerptShown => !LCorpusQuotationSide && !CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusDisplayShown => LCorpusQuotationSide && !CCorpusQuotation.CQuotationPanel.CPanelEditing;

    public bool CCorpusEditorShown => CCorpusQuotation.CQuotationPanel.CPanelEditing;

    public bool CCorpusExcerptHeld => CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusExcerptBlank => !CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusScribeChecked => CCorpusTranscriptShown || CCorpusEditorShown;

    public bool CCorpusViewerChecked => !CCorpusScribeChecked;

    public bool CCorpusModeEnabled => LCorpusQuotationSide || CCorpusAnthology.CAnthologyPanel.CPanelModeEnabled;

    public bool CCorpusBinEnabled => !LCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusStoreEnabled =>
        CCorpusEditorShown ? CCorpusEditor.CEditorDesk.CDeskStorable : CCorpusDesk.CDeskChanged;

    public bool CCorpusPressAllowed => CCorpusDisplayShown || LCorpusRowShown;

    public bool CCorpusPortraitAllowed => CCorpusDisplayShown;

    private bool LCorpusQuotationSide => CCorpusQuotation.CQuotationPanel.CPanelModeEnabled;

    private bool LCorpusRowShown => CCorpusExcerptShown && CCorpusExcerptHeld;

    private bool LCorpusRowHeld =>
        CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled || CCorpusQuotation.CQuotationPanel.CPanelBinEnabled;

    internal CExample? LCorpusTranscriptRead()
    {
        try
        {
            return CCorpusAnthology.LAnthologyDraftRead(
                CCorpusDesk.CDeskRead(), CCorpusAnthology.CAnthologyPanel.CPanelTallyRead());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCorpusEnvoy, _cCorpusSettingsPort, "Example.HoldFailed", exception);
            return null;
        }
    }

    private void LCorpusDraftResonate()
    {
        if (LCorpusTranscriptRead() is CExample example)
        {
            CCorpusDraftChanged?.Invoke(example);
        }
    }

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

    public void CCorpusMentionOpen(string word)
    {
        CCorpusMentionOffered?.Invoke(
            CMention.LMentionProspectRead(CCorpusDesk, word, _cCorpusEnvoy, _cCorpusSettingsPort));
    }

    public void CCorpusMentionAdd(string text, int start, int length, long entryId)
    {
        CCorpusDesk.CDeskChip?.LQuillMentionAdd(0, 0, text, start, length, entryId);
    }

    public void CCorpusSenseSet(string text, int start, int length, long senseId)
    {
        CCorpusDesk.CDeskChip?.LQuillSenseSet(0, 0, text, start, length, senseId);
    }

    public void CCorpusMentionRemove(long mentionId)
    {
        CCorpusDesk.CDeskQuill?.LQuillMentionRemove(0, 0, mentionId);
    }

    public void CCorpusMentionRemove(string text, int start, int length)
    {
        CCorpusDesk.CDeskChip?.LQuillMentionRemove(0, 0, text, start, length);
    }

    public bool CCorpusMentionCheck(string text, int start, int length)
    {
        return CCorpusDesk.CDeskTenure?.LTenureMentionCheck(0, 0, text, start, length) is true;
    }

    public bool CCorpusSenseCheck(string text, int start, int length)
    {
        return CCorpusDesk.CDeskTenure?.LTenureSenseCheck(0, 0, text, start, length) is true;
    }

    public CMentionSense? CCorpusSenseRead(string text, int start, int length)
    {
        return CMention.LMentionSenseRead(
            _cCorpusAtelier.CAtelierDraftPort,
            CCorpusDesk,
            0,
            0,
            text,
            start,
            length,
            _cCorpusEnvoy,
            _cCorpusSettingsPort);
    }

    public IReadOnlyList<CMentionLabel> CCorpusMentionRead()
    {
        return CMention.LMentionChipRead(
            _cCorpusAtelier.CAtelierDraftPort, CCorpusDesk, 0, 0, _cCorpusEnvoy, _cCorpusSettingsPort);
    }

    public CMentionOffer? CCorpusMentionFind(int offset)
    {
        return LCorpusLeaveConfirm(true) ? CCorpusAnthology.LAnthologyMentionFind(offset) : null;
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

    public Task CCorpusPortraitExport()
    {
        if (!CCorpusPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return CCorpusQuotation.LQuotationPortraitExport(_cCorpusEnvoy, _cCorpusSettingsPort);
    }

    public void CCorpusVistaRestore()
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
