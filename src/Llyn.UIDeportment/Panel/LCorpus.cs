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
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lCorpusLeaveSeam = leaveSeam;
        LCorpusEditor = editor;
        LCorpusDesk = new LDesk(drafts, "Example", unreadableSeam);
        LCorpusAnthology = new LAnthology(
            entries, portraits, LCorpusDesk, shownSeam, LCorpusLeaveConfirm, removalSeam);
        LCorpusQuotation = new LQuotation(
            entries, portraits, editor.LEditorDesk.LDeskChangeCheck, shownSeam, LCorpusLeaveConfirm);
        LCorpusAnthology.LAnthologyPanel.LPanelRowsChanged += LCorpusQuotation.LQuotationPanel.LPanelRowsUpdate;
        LCorpusSession = new QSession(
            LCorpusDesk,
            [LCorpusQuotation.LQuotationPanel, LCorpusAnthology.LAnthologyPanel],
            editor,
            LCorpusQuotation.LQuotationPanel,
            "Example.HoldFailed",
            id => LCorpusDesk.LDeskStart("Corpus", CSubject.CSubjectExample, id),
            static () => true,
            LCorpusStoredShow);
        LCorpusSession.QSessionHeld += () => LCorpusTranscriptChanged?.Invoke(LCorpusTranscriptRead());
        LCorpusSession.QSessionChanged += () => LCorpusChanged?.Invoke();
        LCorpusSession.QSessionFailed += (key, exception) => LCorpusFailed?.Invoke(key, exception);
        LCorpusAnthology.LAnthologyPanel.LPanelEdited += id => LCorpusSession.QSessionStart(id);
        LCorpusAnthology.LAnthologyPanel.LPanelCleared += LCorpusSession.QSessionCancel;
        LCorpusAnthology.LAnthologyPanel.LPanelDraftChanged += LCorpusExampleUpdate;
        LCorpusQuotation.LQuotationPanel.LPanelEdited += LCorpusEditorOpen;
        LCorpusQuotation.LQuotationPanel.LPanelCleared += editor.LEditorClose;
        LCorpusQuotation.LQuotationPanel.LPanelCleared += lectern.LLecternClear;
        LCorpusQuotation.LQuotationPanel.LPanelDraftChanged += lectern.LLecternDraftShow;
    }

    public event Action? LCorpusChanged;

    public event Action<CExample?>? LCorpusTranscriptChanged;

    public event Action<CExample>? LCorpusExampleChanged;

    public event Action<string, Exception>? LCorpusFailed;

    public event Action? LCorpusQueryCleared;

    private LEditor LCorpusEditor { get; }

    public LDesk LCorpusDesk { get; }

    public QSession LCorpusSession { get; }

    public LAnthology LCorpusAnthology { get; }

    public LQuotation LCorpusQuotation { get; }

    private bool LCorpusQuotationSide => LCorpusQuotation.LQuotationPanel.LPanelModeEnabled;

    public bool LCorpusTranscriptShown => !LCorpusQuotationSide && LCorpusAnthology.LAnthologyPanel.LPanelEditing;

    public bool LCorpusExcerptShown => !LCorpusQuotationSide && !LCorpusAnthology.LAnthologyPanel.LPanelEditing;

    public bool LCorpusDisplayShown => LCorpusQuotationSide && !LCorpusQuotation.LQuotationPanel.LPanelEditing;

    public bool LCorpusEditorShown => LCorpusQuotation.LQuotationPanel.LPanelEditing;

    public bool LCorpusExcerptHeld => LCorpusAnthology.LAnthologyPanel.LPanelBinEnabled;

    public bool LCorpusExcerptBlank => !LCorpusAnthology.LAnthologyPanel.LPanelBinEnabled;

    public bool LCorpusScribeChecked => LCorpusTranscriptShown || LCorpusEditorShown;

    public bool LCorpusViewerChecked => !LCorpusScribeChecked;

    public bool LCorpusModeEnabled => LCorpusQuotationSide || LCorpusAnthology.LAnthologyPanel.LPanelModeEnabled;

    public bool LCorpusBinEnabled => !LCorpusQuotationSide && LCorpusAnthology.LAnthologyPanel.LPanelBinEnabled;

    public bool LCorpusStoreEnabled => LCorpusEditorShown ? LCorpusEditor.LEditorStorable : LCorpusDesk.LDeskChanged;

    public bool LCorpusPressAllowed => LCorpusDisplayShown || (LCorpusExcerptShown && LCorpusExcerptHeld);

    public bool LCorpusPortraitAllowed => LCorpusDisplayShown;

    public IReadOnlyList<CCitationRow> LCorpusCitationFind(string word) =>
        LCorpusAnthology.LAnthologyCitationFind(word, LCorpusTranscriptRead()?.CExampleSource);

    public CExample? LCorpusTranscriptRead()
    {
        try
        {
            return LAnthology.LAnthologyExampleRead(LCorpusDesk.LDeskRead()?.LDraftExample);
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
        LCorpusEditor.LEditorOpen(id);
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
        LCorpusQuotation.LQuotationPanel.LPanelClear();
        LCorpusAnthology.LAnthologyPanel.LPanelScribeShow(editing);
        LCorpusAnthology.LAnthologyPanel.LPanelRowShow(id);
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
        if (!LCorpusQuotation.LQuotationPanel.LPanelRowShow(id))
        {
            return;
        }

        LCorpusDesk.LDeskCancel();
        LCorpusAnthology.LAnthologyPanel.LPanelScribeShow(false);
        if (editing)
        {
            LCorpusQuotation.LQuotationPanel.LPanelScribeSet(true);
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

        LCorpusQuotation.LQuotationPanel.LPanelClear();
        LCorpusAnthology.LAnthologyPanel.LPanelFreshOpen();
        LCorpusSession.QSessionStart(null);
    }

    public void LCorpusScribeSet(bool editing)
    {
        if (LCorpusQuotationSide)
        {
            LCorpusQuotation.LQuotationPanel.LPanelScribeSet(editing);
            if (!LCorpusQuotationSide)
            {
                LCorpusExampleRestore(editing);
            }

            return;
        }

        LCorpusAnthology.LAnthologyPanel.LPanelScribeSet(editing);
        if (!LCorpusAnthology.LAnthologyPanel.LPanelEditing)
        {
            LCorpusDesk.LDeskCancel();
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
        LCorpusQuotation.LQuotationPanel.LPanelClear();
        LCorpusAnthology.LAnthologyPanel.LPanelClear();
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
        LCorpusAnthology.LAnthologyPanel.LPanelBinEnabled || LCorpusQuotation.LQuotationPanel.LPanelBinEnabled;

    private void LCorpusQueryClear()
    {
        LCorpusAnthology.LAnthologyQuerySet(string.Empty);
        LCorpusAnthology.LAnthologyGauzeSet(new CCatalogFilter([]));
        LCorpusQueryCleared?.Invoke();
    }

    private void LCorpusStoredShow(long id)
    {
        LCorpusAnthology.LAnthologyPanel.LPanelScribeShow(false);
        LCorpusExampleShow(id);
    }

    public void LCorpusDelete()
    {
        if (LCorpusQuotationSide)
        {
            return;
        }

        LCorpusAnthology.LAnthologyPanel.LPanelDelete();
    }

    private void LCorpusQuotationCreate()
    {
        long? chosen = LCorpusAnthology.LAnthologyChosen;
        LCorpusDesk.LDeskCancel();
        LCorpusAnthology.LAnthologyPanel.LPanelScribeShow(false);
        LCorpusQuotation.LQuotationPanel.LPanelFreshOpen();
        LCorpusEditor.LEditorOpen(null);
        if (chosen is long id)
        {
            LCorpusEditor.LEditorExampleAdd(id);
        }
    }

    public bool LCorpusLeaveConfirm()
    {
        return LCorpusLeaveConfirm(true);
    }

    private bool LCorpusLeaveConfirm(bool shown)
    {
        if (!LCorpusSession.QSessionChangeCheck())
        {
            return true;
        }

        if (shown)
        {
            return _lCorpusLeaveSeam(LCorpusSession.QSessionFinish);
        }

        return _lCorpusLeaveSeam(LCorpusSession.QSessionClose);
    }

    public void LCorpusEntryUpdate()
    {
        if (!LCorpusQuotation.LQuotationPanel.LPanelBinEnabled)
        {
            return;
        }

        bool editing = LCorpusScribeChecked;
        LCorpusQuotation.LQuotationPanel.LPanelDraftUpdate();
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
        LCorpusEditor.LEditorVistaRestore(quotation);
    }

    internal void LCorpusVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LCorpusVistaRestore(
            window.LWindowAtelier.CAtelierVistaStart(
                "corpus", CSubject.CSubjectExample, CCatalogOrder.CCatalogOrderText),
            window.LWindowAtelier.CAtelierVistaStart(
                "quotation", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }
}
