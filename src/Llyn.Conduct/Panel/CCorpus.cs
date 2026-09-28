using System;

namespace Llyn.Conduct;

public sealed class CCorpus
{
    private readonly CEditor _cCorpusEditor;

    private readonly CEnvoy _cCorpusEnvoy;

    private CCorpus(CAtelier atelier, CEditor editor, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCorpusEditor = editor;
        _cCorpusEnvoy = envoy;
        CCorpusDesk = new CDesk(atelier.CAtelierDraftPort, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        CCorpusAnthology = CAnthology.CAnthologyCreate(
            atelier, CCorpusDesk, shownSeam, envoy, store => CCorpusSession!.CSessionFinish(store));
        CCorpusQuotation = new CPanel(
            envoy,
            "List.LoadFailed",
            null,
            editor.CEditorDesk.CDeskChangeCheck,
            store => CCorpusSession!.CSessionFinish(store),
            shownSeam);
        CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += CCorpusQuotation.CPanelRowsUpdate;
        CCorpusSession = new CSession(
            CCorpusDesk,
            [CCorpusQuotation.CPanelChangeCheck, CCorpusAnthology.CAnthologyPanel.CPanelChangeCheck],
            editor.CEditorDesk,
            () => CCorpusQuotation.CPanelEditing,
            editor.CEditorFinish,
            static () => true,
            LCorpusStoredShow);
        CCorpusSession.CSessionHeld += () => CCorpusTranscriptChanged?.Invoke(CCorpusTranscriptRead());
        CCorpusSession.CSessionChanged += () => CCorpusChanged?.Invoke();
        CCorpusSession.CSessionFailed += envoy.CEnvoyFailureShow;
        CCorpusAnthology.CAnthologyPanel.CPanelEdited += id => CCorpusSession.CSessionStart(id);
        CCorpusAnthology.CAnthologyPanel.CPanelCleared += CCorpusSession.CSessionCancel;
        CCorpusAnthology.CAnthologyPanel.CPanelDraftChanged +=
            draft => LCorpusExampleUpdate(CAnthology.CAnthologyDraftRead(draft));
        CCorpusQuotation.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CCorpusQuotation.CPanelCleared += editor.CEditorDesk.CDeskCancel;
    }

    public static CCorpus CCorpusCreate(CAtelier atelier, CEditor editor, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CCorpus(atelier, editor, shownSeam, envoy);
    }

    public event Action? CCorpusChanged;

    public event Action<CExample?>? CCorpusTranscriptChanged;

    public event Action<CExample>? CCorpusExampleChanged;

    public event Action? CCorpusQueryCleared;

    public CDesk CCorpusDesk { get; }

    public CSession CCorpusSession { get; }

    public CAnthology CCorpusAnthology { get; }

    public CPanel CCorpusQuotation { get; }

    public bool CCorpusQuotationSide => CCorpusQuotation.CPanelModeEnabled;

    public bool CCorpusTranscriptShown => !CCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusExcerptShown => !CCorpusQuotationSide && !CCorpusAnthology.CAnthologyPanel.CPanelEditing;

    public bool CCorpusDisplayShown => CCorpusQuotationSide && !CCorpusQuotation.CPanelEditing;

    public bool CCorpusEditorShown => CCorpusQuotation.CPanelEditing;

    public bool CCorpusExcerptHeld => CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusExcerptBlank => !CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusScribeChecked => CCorpusTranscriptShown || CCorpusEditorShown;

    public bool CCorpusViewerChecked => !CCorpusScribeChecked;

    public bool CCorpusModeEnabled => CCorpusQuotationSide || CCorpusAnthology.CAnthologyPanel.CPanelModeEnabled;

    public bool CCorpusBinEnabled => !CCorpusQuotationSide && CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled;

    public bool CCorpusStoreEnabled =>
        CCorpusEditorShown ? _cCorpusEditor.CEditorDesk.CDeskStorable : CCorpusDesk.CDeskChanged;

    public bool CCorpusPressAllowed => CCorpusDisplayShown || CCorpusRowShown;

    public bool CCorpusPortraitAllowed => CCorpusDisplayShown;

    public bool CCorpusRowShown => CCorpusExcerptShown && CCorpusExcerptHeld;

    private bool LCorpusRowHeld =>
        CCorpusAnthology.CAnthologyPanel.CPanelBinEnabled || CCorpusQuotation.CPanelBinEnabled;

    public CExample? CCorpusTranscriptRead()
    {
        try
        {
            return CAnthology.CAnthologyDraftRead(CCorpusDesk.CDeskRead());
        }
        catch (Exception exception)
        {
            _cCorpusEnvoy.CEnvoyFailureShow("Example.HoldFailed", exception);
            return null;
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

    public void CCorpusExampleOpen(long id)
    {
        bool editing = CCorpusScribeChecked;
        LCorpusExampleShow(id, editing);
        if (CCorpusExcerptHeld)
        {
            return;
        }

        if (!CCorpusAnthology.CAnthologyNarrowed)
        {
            return;
        }

        LCorpusQueryClear();
        LCorpusExampleShow(id, editing);
    }

    private void LCorpusExampleShow(long id, bool editing)
    {
        CCorpusQuotation.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(editing);
        CCorpusAnthology.CAnthologyPanel.CPanelRowOpen(id);
    }

    public void CCorpusExampleSelect(long? id, Action record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (id is not long chosen)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        if (!LCorpusLeaveConfirm(false))
        {
            return;
        }

        record();
        LCorpusExampleShow(chosen, editing);
    }

    private void LCorpusQuotationOpen(long id, bool editing)
    {
        if (!CCorpusQuotation.CPanelRowOpen(id))
        {
            return;
        }

        CCorpusDesk.CDeskCancel();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        if (editing)
        {
            CCorpusQuotation.CPanelScribeToggle(true);
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

        CCorpusQuotation.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelFreshOpen();
        CCorpusSession.CSessionStart(null);
    }

    public void CCorpusScribeToggle(bool editing)
    {
        if (CCorpusQuotationSide)
        {
            CCorpusQuotation.CPanelScribeToggle(editing);
            if (!CCorpusQuotationSide)
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

        CCorpusExampleClose();
    }

    public void CCorpusExampleClose()
    {
        CCorpusQuotation.CPanelEntryClose();
        CCorpusAnthology.CAnthologyPanel.CPanelEntryClose();
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
        CCorpusExampleOpen(id);
    }

    private void LCorpusQuotationCreate()
    {
        long? chosen = CCorpusAnthology.CAnthologyChosen;
        CCorpusDesk.CDeskCancel();
        CCorpusAnthology.CAnthologyPanel.CPanelScribeSet(false);
        CCorpusQuotation.CPanelFreshOpen();
        _cCorpusEditor.CEditorEntryOpen(null);
        if (chosen is long id)
        {
            _cCorpusEditor.CEditorExampleAdd(id);
        }
    }

    public bool CCorpusLeaveConfirm()
    {
        return LCorpusLeaveConfirm(true);
    }

    private bool LCorpusLeaveConfirm(bool shown)
    {
        if (!CCorpusSession.CSessionChangeCheck())
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

        return shown ? CCorpusSession.CSessionFinish(true) : CCorpusSession.CSessionClose(true);
    }

    public void CCorpusEntryUpdate()
    {
        if (!CCorpusQuotation.CPanelBinEnabled)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        CCorpusQuotation.CPanelDraftUpdate();
        if (CCorpusQuotationSide)
        {
            return;
        }

        LCorpusExampleRestore(editing);
    }
}
