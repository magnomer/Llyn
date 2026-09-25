using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCorpus
{
    private readonly Func<Func<bool, bool>, bool> _lCorpusLeaveSeam;

    public LCorpus(
        LDraftPort drafts,
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        Func<bool> shownSeam,
        Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lCorpusLeaveSeam = leaveSeam;
        LCorpusEditor = editor;
        LCorpusDesk = new LDesk(drafts, "Example", unreadableSeam);
        LCorpusAnthology = new LAnthology(
            entries, portraits, settings, LCorpusDesk, shownSeam, LCorpusLeaveConfirm, removalSeam);
        LCorpusQuotation = new LQuotation(
            entries, portraits, editor.LEditorDesk.LDeskChangeCheck, shownSeam, LCorpusLeaveConfirm);
        LCorpusAnthology.LAnthologyPanel.LPanelRowsChanged += LCorpusQuotation.LQuotationPanel.LPanelRowsUpdate;
        LCorpusAnthology.LAnthologyPanel.LPanelEdited += LCorpusDraftOpen;
        LCorpusAnthology.LAnthologyPanel.LPanelCleared += LCorpusDraftCancel;
        LCorpusAnthology.LAnthologyPanel.LPanelDraftChanged += LCorpusExampleUpdate;
        LCorpusQuotation.LQuotationPanel.LPanelEdited += LCorpusEditorOpen;
        LCorpusQuotation.LQuotationPanel.LPanelCleared += editor.LEditorClose;
        LCorpusDesk.LDeskStateChanged += LCorpusStateUpdate;
        editor.LEditorStateChanged += LCorpusStateUpdate;
    }

    public event Action? LCorpusChanged;

    public event Action<LExample?>? LCorpusTranscriptChanged;

    public event Action<LExample>? LCorpusExampleChanged;

    public event Action<string, Exception>? LCorpusFailed;

    public event Action? LCorpusQueryCleared;

    private LEditor LCorpusEditor { get; }

    public LDesk LCorpusDesk { get; }

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

    private void LCorpusDraftStart(long? id)
    {
        LCorpusDesk.LDeskStart("Corpus", LSubject.LSubjectExample, id);
        LCorpusTranscriptChanged?.Invoke(LCorpusTranscriptRead());
    }

    private LExample? LCorpusTranscriptRead()
    {
        try
        {
            return LCorpusDesk.LDeskRead()?.LDraftExample;
        }
        catch (Exception exception)
        {
            LCorpusFailed?.Invoke("Example.HoldFailed", exception);
            return null;
        }
    }

    private void LCorpusDraftOpen(long id)
    {
        LCorpusDraftStart(id);
    }

    private void LCorpusDraftCancel()
    {
        LCorpusDesk.LDeskCancel();
        LCorpusTranscriptChanged?.Invoke(null);
    }

    private void LCorpusExampleUpdate(LDraft draft)
    {
        if (draft.LDraftExample is LExample example)
        {
            LCorpusExampleChanged?.Invoke(example);
        }
    }

    private void LCorpusEditorOpen(long id)
    {
        LCorpusEditor.LEditorOpen(id);
    }

    private void LCorpusStateUpdate()
    {
        LCorpusChanged?.Invoke();
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
        LCorpusDraftStart(null);
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

    public void LCorpusRowsApply(IReadOnlyList<LCatalogExample> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (!LCorpusRowShown)
        {
            return;
        }

        foreach (LCatalogExample row in rows)
        {
            if (row.LCatalogExampleChosen)
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
        LCorpusAnthology.LAnthologyGauzeSet(LCatalogFilter.LCatalogFilterEmpty);
        LCorpusQueryCleared?.Invoke();
    }

    public void LCorpusSave()
    {
        if (LCorpusEditorShown)
        {
            LCorpusEditor.LEditorSave();
            return;
        }

        if (LCorpusDesk.LDeskChangeCheck())
        {
            LCorpusDesk.LDeskFinish(true, LCorpusStoredShow);
        }
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

    public bool LCorpusChangeCheck()
    {
        LPanel quotation = LCorpusQuotation.LQuotationPanel;
        LPanel anthology = LCorpusAnthology.LAnthologyPanel;
        return quotation.LPanelChangeCheck() || anthology.LPanelChangeCheck();
    }

    public bool LCorpusLeaveConfirm()
    {
        return LCorpusLeaveConfirm(true);
    }

    private bool LCorpusLeaveConfirm(bool shown)
    {
        if (!LCorpusChangeCheck())
        {
            return true;
        }

        if (shown)
        {
            return _lCorpusLeaveSeam(LCorpusDraftFinish);
        }

        return _lCorpusLeaveSeam(LCorpusDraftClose);
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

    public bool LCorpusDraftFinish(bool store)
    {
        return LCorpusEditorShown
            ? LCorpusEditor.LEditorFinish(store)
            : LCorpusDesk.LDeskFinish(store, LCorpusStoredShow);
    }

    private bool LCorpusDraftClose(bool store)
    {
        return LCorpusEditorShown ? LCorpusEditor.LEditorFinish(store) : LCorpusDesk.LDeskFinish(store);
    }

    public (bool LDeskBackward, bool LDeskForward) LCorpusChronicleRead()
    {
        return LCorpusEditorShown
            ? LCorpusEditor.LEditorDesk.LDeskChronicleRead()
            : LCorpusDesk.LDeskChronicleRead();
    }

    public void LCorpusUndo()
    {
        if (LCorpusEditorShown)
        {
            LCorpusEditor.LEditorDesk.LDeskUndo();
            return;
        }

        LCorpusChronicleRun(LCorpusDesk.LDeskUndo);
    }

    public void LCorpusRedo()
    {
        if (LCorpusEditorShown)
        {
            LCorpusEditor.LEditorDesk.LDeskRedo();
            return;
        }

        LCorpusChronicleRun(LCorpusDesk.LDeskRedo);
    }

    private void LCorpusChronicleRun(Action step)
    {
        try
        {
            step();
        }
        catch (Exception exception)
        {
            LCorpusFailed?.Invoke("Example.HoldFailed", exception);
        }
    }

    public Task LCorpusPortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)
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

    public Task LCorpusPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        if (!LCorpusPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return LCorpusQuotation.LQuotationPortraitExport(path, format, label);
    }

    public void LCorpusVistaRestore(LVista vista, LVista quotation)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(quotation);

        LCorpusAnthology.LAnthologyVistaRestore(vista);
        LCorpusQuotation.LQuotationVistaRestore(vista, quotation);
        LCorpusEditor.LEditorVistaRestore(quotation);
    }

    public void LCorpusVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LCorpusVistaRestore(
            window.LWindowVistaStart("corpus", LSubject.LSubjectExample, LCatalogOrder.LCatalogOrderText),
            window.LWindowVistaStart("quotation", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }
}
