using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCorpus
{
    private readonly Func<Func<bool, bool>, bool> _lCorpusLeaveSeam;

    internal LCorpus(
        LDraftPort drafts,
        LEntryPort entries,
        LPortraitPort portraits,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<Func<bool, bool>, bool> leaveSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lCorpusLeaveSeam = leaveSeam;
        LCorpusEditor = editor;
        LCorpusDesk = new CDesk(drafts, "Example", envoy, "Corpus", CSubject.CSubjectExample);
        LCorpusAnthology = new LAnthology(
            entries,
            portraits,
            LCorpusDesk,
            shownSeam,
            envoy,
            store => LCorpusSession!.CSessionFinish(store));
        LCorpusQuotation = new LQuotation(
            entries,
            portraits,
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck,
            shownSeam,
            envoy,
            store => LCorpusSession!.CSessionFinish(store));
        LCorpusAnthology.LAnthologyPanel.CPanelRowsChanged += LCorpusQuotation.LQuotationPanel.CPanelRowsUpdate;
        LCorpusSession = new CSession(
            LCorpusDesk,
            [LCorpusQuotation.LQuotationPanel.CPanelChangeCheck, LCorpusAnthology.LAnthologyPanel.CPanelChangeCheck],
            editor.LEditorStudio.CEditorDesk,
            () => LCorpusQuotation.LQuotationPanel.CPanelEditing,
            editor.LEditorStudio.CEditorFinish,
            static () => true,
            LCorpusStoredShow);
        LCorpusSession.CSessionHeld += () => LCorpusTranscriptChanged?.Invoke(LCorpusTranscriptRead());
        LCorpusSession.CSessionChanged += () => LCorpusChanged?.Invoke();
        LCorpusSession.CSessionFailed += (key, exception) => LCorpusFailed?.Invoke(key, exception);
        LCorpusAnthology.LAnthologyPanel.CPanelEdited += id => LCorpusSession.CSessionStart(id);
        LCorpusAnthology.LAnthologyPanel.CPanelCleared += LCorpusSession.CSessionCancel;
        LCorpusAnthology.LAnthologyPanel.CPanelDraftChanged += LCorpusExampleUpdate;
        LCorpusQuotation.LQuotationPanel.CPanelEdited += LCorpusEditorOpen;
        LCorpusQuotation.LQuotationPanel.CPanelCleared += editor.LEditorStudio.CEditorDesk.CDeskCancel;
        LCorpusQuotation.LQuotationPanel.CPanelCleared += lectern.LLecternClear;
        LCorpusQuotation.LQuotationPanel.CPanelDraftChanged += lectern.LLecternDraftShow;
    }

    public event Action? LCorpusChanged;

    public event Action<CExample?>? LCorpusTranscriptChanged;

    public event Action<CExample>? LCorpusExampleChanged;

    public event Action<string, Exception>? LCorpusFailed;

    public event Action? LCorpusQueryCleared;

    private LEditor LCorpusEditor { get; }

    public CDesk LCorpusDesk { get; }

    public CSession LCorpusSession { get; }

    public LAnthology LCorpusAnthology { get; }

    public LQuotation LCorpusQuotation { get; }

    private bool LCorpusQuotationSide => LCorpusQuotation.LQuotationPanel.CPanelModeEnabled;

    public bool LCorpusTranscriptShown => !LCorpusQuotationSide && LCorpusAnthology.LAnthologyPanel.CPanelEditing;

    public bool LCorpusExcerptShown => !LCorpusQuotationSide && !LCorpusAnthology.LAnthologyPanel.CPanelEditing;

    public bool LCorpusDisplayShown => LCorpusQuotationSide && !LCorpusQuotation.LQuotationPanel.CPanelEditing;

    public bool LCorpusEditorShown => LCorpusQuotation.LQuotationPanel.CPanelEditing;

    public bool LCorpusExcerptHeld => LCorpusAnthology.LAnthologyPanel.CPanelBinEnabled;

    public bool LCorpusExcerptBlank => !LCorpusAnthology.LAnthologyPanel.CPanelBinEnabled;

    public bool LCorpusScribeChecked => LCorpusTranscriptShown || LCorpusEditorShown;

    public bool LCorpusViewerChecked => !LCorpusScribeChecked;

    public bool LCorpusModeEnabled => LCorpusQuotationSide || LCorpusAnthology.LAnthologyPanel.CPanelModeEnabled;

    public bool LCorpusBinEnabled => !LCorpusQuotationSide && LCorpusAnthology.LAnthologyPanel.CPanelBinEnabled;

    public bool LCorpusStoreEnabled => LCorpusEditorShown ? LCorpusEditor.LEditorStorable : LCorpusDesk.CDeskChanged;

    public bool LCorpusPressAllowed => LCorpusDisplayShown || (LCorpusExcerptShown && LCorpusExcerptHeld);

    public bool LCorpusPortraitAllowed => LCorpusDisplayShown;

    public IReadOnlyList<CCitationRow> LCorpusCitationFind(string word) =>
        LCorpusAnthology.LAnthologyCitationFind(word, LCorpusTranscriptRead()?.CExampleSource);

    public CExample? LCorpusTranscriptRead()
    {
        try
        {
            return LAnthology.LAnthologyExampleRead(LCorpusDesk.CDeskRead()?.LDraftExample);
        }
        catch (Exception exception)
        {
            LCorpusFailed?.Invoke("Example.HoldFailed", exception);
            return null;
        }
    }

    private void LCorpusExampleUpdate(LDraft draft)
    {
        if (LAnthology.LAnthologyExampleRead(draft.LDraftExample) is CExample example)
        {
            LCorpusExampleChanged?.Invoke(example);
        }
    }

    private void LCorpusEditorOpen(long id)
    {
        LCorpusEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public void LCorpusExampleShow(long id)
    {
        bool editing = LCorpusScribeChecked;
        LCorpusExampleShow(id, editing);
        if (LCorpusExcerptHeld)
        {
            return;
        }

        if (!LCorpusAnthology.LAnthologyNarrowed)
        {
            return;
        }

        LCorpusQueryClear();
        LCorpusExampleShow(id, editing);
    }

    private void LCorpusExampleShow(long id, bool editing)
    {
        LCorpusQuotation.LQuotationPanel.CPanelEntryClose();
        LCorpusAnthology.LAnthologyPanel.CPanelScribeSet(editing);
        LCorpusAnthology.LAnthologyPanel.CPanelRowOpen(id);
    }

    public void LCorpusSelect(long? id, Action record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (id is not long chosen)
        {
            return;
        }

        bool editing = LCorpusScribeChecked;
        if (!LCorpusLeaveConfirm(false))
        {
            return;
        }

        record();
        LCorpusExampleShow(chosen, editing);
    }

    private void LCorpusQuotationOpen(long id, bool editing)
    {
        if (!LCorpusQuotation.LQuotationPanel.CPanelRowOpen(id))
        {
            return;
        }

        LCorpusDesk.CDeskCancel();
        LCorpusAnthology.LAnthologyPanel.CPanelScribeSet(false);
        if (editing)
        {
            LCorpusQuotation.LQuotationPanel.CPanelScribeToggle(true);
        }
    }

    public void LCorpusQuotationSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = LCorpusScribeChecked;
        if (!LCorpusLeaveConfirm())
        {
            return;
        }

        LCorpusQuotationOpen(chosen, editing);
    }

    public void LCorpusFreshStart()
    {
        if (!LCorpusLeaveConfirm())
        {
            return;
        }

        if (LCorpusRowHeld)
        {
            LCorpusQuotationCreate();
            return;
        }

        LCorpusQuotation.LQuotationPanel.CPanelEntryClose();
        LCorpusAnthology.LAnthologyPanel.CPanelFreshOpen();
        LCorpusSession.CSessionStart(null);
    }

    public void LCorpusScribeSet(bool editing)
    {
        if (LCorpusQuotationSide)
        {
            LCorpusQuotation.LQuotationPanel.CPanelScribeToggle(editing);
            if (!LCorpusQuotationSide)
            {
                LCorpusExampleRestore(editing);
            }

            return;
        }

        LCorpusAnthology.LAnthologyPanel.CPanelScribeToggle(editing);
        if (!LCorpusAnthology.LAnthologyPanel.CPanelEditing)
        {
            LCorpusDesk.CDeskCancel();
        }
    }

    private void LCorpusExampleRestore(bool editing)
    {
        if (LCorpusAnthology.LAnthologyChosen is long chosen)
        {
            LCorpusExampleShow(chosen, editing);
            return;
        }

        LCorpusClear();
    }

    public void LCorpusClear()
    {
        LCorpusQuotation.LQuotationPanel.CPanelEntryClose();
        LCorpusAnthology.LAnthologyPanel.CPanelEntryClose();
    }

    public void LCorpusRowsApply(IReadOnlyList<CCatalogExample> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (!LCorpusRowShown)
        {
            return;
        }

        foreach (CCatalogExample row in rows)
        {
            if (row.CCatalogExampleChosen)
            {
                return;
            }
        }

        LCorpusClear();
    }

    private bool LCorpusRowShown => LCorpusExcerptShown && LCorpusExcerptHeld;

    private bool LCorpusRowHeld =>
        LCorpusAnthology.LAnthologyPanel.CPanelBinEnabled || LCorpusQuotation.LQuotationPanel.CPanelBinEnabled;

    private void LCorpusQueryClear()
    {
        LCorpusAnthology.LAnthologyQuerySet(string.Empty);
        LCorpusAnthology.LAnthologyGauzeSet(new CCatalogFilter([]));
        LCorpusQueryCleared?.Invoke();
    }

    private void LCorpusStoredShow(long id)
    {
        LCorpusAnthology.LAnthologyPanel.CPanelScribeSet(false);
        LCorpusExampleShow(id);
    }

    public void LCorpusDelete()
    {
        if (LCorpusQuotationSide)
        {
            return;
        }

        LCorpusAnthology.LAnthologyPanel.CPanelEntryDelete();
    }

    private void LCorpusQuotationCreate()
    {
        long? chosen = LCorpusAnthology.LAnthologyChosen;
        LCorpusDesk.CDeskCancel();
        LCorpusAnthology.LAnthologyPanel.CPanelScribeSet(false);
        LCorpusQuotation.LQuotationPanel.CPanelFreshOpen();
        LCorpusEditor.LEditorStudio.CEditorEntryOpen(null);
        if (chosen is long id)
        {
            LCorpusEditor.LEditorStudio.CEditorExampleAdd(id);
        }
    }

    public bool LCorpusLeaveConfirm()
    {
        return LCorpusLeaveConfirm(true);
    }

    private bool LCorpusLeaveConfirm(bool shown)
    {
        if (!LCorpusSession.CSessionChangeCheck())
        {
            return true;
        }

        if (shown)
        {
            return _lCorpusLeaveSeam(LCorpusSession.CSessionFinish);
        }

        return _lCorpusLeaveSeam(LCorpusSession.CSessionClose);
    }

    public void LCorpusEntryUpdate()
    {
        if (!LCorpusQuotation.LQuotationPanel.CPanelBinEnabled)
        {
            return;
        }

        bool editing = LCorpusScribeChecked;
        LCorpusQuotation.LQuotationPanel.CPanelDraftUpdate();
        if (LCorpusQuotationSide)
        {
            return;
        }

        LCorpusExampleRestore(editing);
    }

    public Task LCorpusPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (LCorpusDisplayShown)
        {
            return LCorpusQuotation.LQuotationPortraitPrint(label, ticket);
        }

        if (LCorpusPressAllowed)
        {
            return LCorpusAnthology.LAnthologyPortraitPrint(legend, ticket);
        }

        return Task.CompletedTask;
    }

    public Task LCorpusPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        if (!LCorpusPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return LCorpusQuotation.LQuotationPortraitExport(path, format, label);
    }

    internal void LCorpusVistaRestore(LVista vista, LVista quotation)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(quotation);

        LCorpusAnthology.LAnthologyVistaRestore(vista);
        LCorpusQuotation.LQuotationVistaRestore(vista, quotation);
        LCorpusEditor.LEditorStudio.CEditorVistaRestore(quotation);
    }

    internal void LCorpusVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LCorpusVistaRestore(
            atelier.CAtelierVistaStart(
                "corpus", CSubject.CSubjectExample, CCatalogOrder.CCatalogOrderText),
            atelier.CAtelierVistaStart(
                "quotation", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }
}
