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

    private CCorpus(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCorpusAtelier = atelier;
        _cCorpusEnvoy = envoy;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CCorpusEditor = editor;
        CCorpusDesk = new CDesk(atelier.CAtelierDraftPort, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        CCorpusAnthology = CAnthology.LAnthologyCreate(
            atelier, CCorpusDesk, shownSeam, envoy, store => CCorpusSession!.CSessionFinish(store));
        CCorpusQuotation = new CQuotation(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            envoy,
            editor.CEditorDesk.CDeskChangeCheck,
            store => CCorpusSession!.CSessionFinish(store),
            shownSeam);
        CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += CCorpusQuotation.CQuotationPanel.CPanelRowsUpdate;
        CCorpusSession = new CSession(
            CCorpusDesk,
            [CCorpusQuotation.CQuotationPanel.CPanelChangeCheck, CCorpusAnthology.CAnthologyPanel.CPanelChangeCheck],
            editor.CEditorDesk,
            () => CCorpusQuotation.CQuotationPanel.CPanelEditing,
            editor.CEditorFinish,
            static () => true,
            LCorpusStoredShow);
        CCorpusSession.CSessionHeld += () => CCorpusTranscriptChanged?.Invoke(CCorpusTranscriptRead());
        CCorpusSession.CSessionChanged += () => CCorpusChanged?.Invoke();
        CCorpusSession.CSessionFailed += envoy.CEnvoyFailureShow;
        CCorpusAnthology.CAnthologyPanel.CPanelEdited += id => CCorpusSession.CSessionStart(id);
        CCorpusAnthology.CAnthologyPanel.CPanelCleared += CCorpusSession.CSessionCancel;
        CCorpusAnthology.CAnthologyPanel.CPanelDraftChanged +=
            draft => LCorpusExampleUpdate(CAnthology.LAnthologyDraftRead(draft));
        CCorpusQuotation.CQuotationPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        CCorpusQuotation.CQuotationPanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
    }

    public static CCorpus CCorpusCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CCorpus(atelier, shownSeam, envoy);
    }

    public event Action? CCorpusChanged;

    public event Action<CExample?>? CCorpusTranscriptChanged;

    public event Action<CExample>? CCorpusExampleChanged;

    public event Action? CCorpusQueryCleared;

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

    public CExample? CCorpusTranscriptRead()
    {
        try
        {
            return CAnthology.LAnthologyDraftRead(CCorpusDesk.CDeskRead());
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

        CCorpusExampleClose();
    }

    public void CCorpusExampleClose()
    {
        CCorpusQuotation.CQuotationPanel.CPanelEntryClose();
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
        CCorpusQuotation.CQuotationPanel.CPanelFreshOpen();
        CCorpusEditor.CEditorDesk.LDeskQuotationStart(chosen);
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
        if (!CCorpusQuotation.CQuotationPanel.CPanelBinEnabled)
        {
            return;
        }

        bool editing = CCorpusScribeChecked;
        CCorpusQuotation.CQuotationPanel.CPanelDraftUpdate();
        if (LCorpusQuotationSide)
        {
            return;
        }

        LCorpusExampleRestore(editing);
    }

    public IReadOnlyList<CCatalogExample> CCorpusRowsRead(string unknown, string unwritten)
    {
        IReadOnlyList<CCatalogExample> rows = CCorpusAnthology.LAnthologyRowsRead(unknown, unwritten);
        if (LCorpusRowShown && !rows.Any(static row => row.CCatalogExampleChosen))
        {
            CCorpusExampleClose();
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

    public Task CCorpusPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (CCorpusDisplayShown)
        {
            return CCorpusQuotation.LQuotationPortraitPrint(label, ticket);
        }

        if (CCorpusPressAllowed)
        {
            return CCorpusAnthology.LAnthologyPortraitPrint(legend, ticket);
        }

        return Task.CompletedTask;
    }

    public Task CCorpusPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        if (!CCorpusPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return CCorpusQuotation.LQuotationPortraitExport(path, format, label);
    }

    public void CCorpusVistaRestore()
    {
        LVista vista = _cCorpusAtelier.CAtelierVistaStart(
            "corpus", CSubject.CSubjectExample, CCatalogOrder.CCatalogOrderText);
        LVista quotation = _cCorpusAtelier.CAtelierVistaStart(
            "quotation", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CCorpusAnthology.LAnthologyVistaRestore(vista);
        CCorpusQuotation.LQuotationVistaRestore(vista, quotation);
        CCorpusEditor.CEditorVistaRestore(quotation);
    }
}
