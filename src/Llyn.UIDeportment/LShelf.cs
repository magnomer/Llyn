using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LShelf
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<bool> _lShelfLeaveSeam;

    private readonly Func<int, bool> _lShelfRemovalSeam;

    private LVista? _lShelfVista;

    private int _lShelfCount;

    public LShelf(
        LDraftPort drafts, LEntryPort entries, LPortraitPort portraits, LSettingsPort settings,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(leaveSeam);
        ArgumentNullException.ThrowIfNull(removalSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lShelfLeaveSeam = leaveSeam;
        _lShelfRemovalSeam = removalSeam;
        LShelfEditor = editor;
        LShelfImprint = new LImprint(drafts, entries, settings, unreadableSeam);
        LShelfPanel = new LPanel("Source.LoadFailed", LImprintChangeCheck, shownSeam, leaveSeam, LShelfDeleteConfirm);
        LShelfFootnote = new LFootnote(entries, portraits, editor, shownSeam, leaveSeam);
        LShelfPanel.LPanelRowsChanged += LShelfFootnote.LFootnotePanel.LPanelRowsUpdate;
        LShelfPanel.LPanelCleared += LShelfImprint.LImprintCancel;
        LShelfPanel.LPanelEdited += LShelfImprintOpen;
        LShelfImprint.LImprintDesk.LDeskFinished += LShelfStoredShow;
        LShelfImprint.LImprintDesk.LDeskStateChanged += LShelfStateUpdate;
        LShelfEditor.LEditorStateChanged += LShelfStateUpdate;
    }

    public event Action? LShelfChanged;

    public LEditor LShelfEditor { get; }

    public LPanel LShelfPanel { get; }

    public LFootnote LShelfFootnote { get; }

    public LImprint LShelfImprint { get; }

    public bool LShelfEntrySide => LShelfFootnote.LFootnotePanel.LPanelModeEnabled;

    public bool LShelfEditing =>
        LShelfEntrySide ? LShelfFootnote.LFootnotePanel.LPanelEditing : LShelfPanel.LPanelEditing;

    public bool LShelfEditorShown => LShelfEntrySide && LShelfFootnote.LFootnotePanel.LPanelEditing;

    public bool LShelfDisplayShown => LShelfEntrySide && !LShelfFootnote.LFootnotePanel.LPanelEditing;

    public bool LShelfImprintShown => !LShelfEntrySide && LShelfPanel.LPanelEditing;

    public bool LShelfColophonShown => !LShelfEntrySide && !LShelfPanel.LPanelEditing;

    public bool LShelfViewerChecked => !LShelfEditing;

    public bool LShelfScribeChecked => LShelfEditing;

    public bool LShelfModeEnabled => LShelfPanel.LPanelModeEnabled || LShelfFootnote.LFootnotePanel.LPanelModeEnabled;

    public bool LShelfBinEnabled => !LShelfEntrySide && LShelfPanel.LPanelBinEnabled;

    public bool LShelfStoreEnabled =>
        LShelfEntrySide ? LShelfEditor.LEditorStorable : LShelfImprint.LImprintDesk.LDeskChangeCheck();

    public bool LShelfPressAllowed => LShelfFootnote.LFootnotePanel.LPanelPressAllowed || LShelfSourcePrintable;

    private bool LShelfSourcePrintable => !LShelfEntrySide && LShelfPanel.LPanelPressAllowed;

    private bool LShelfSourceShown => LShelfPanel.LPanelBinEnabled && !LShelfPanel.LPanelEditing;

    private bool LShelfRowChosen => LShelfPanel.LPanelBinEnabled || LShelfFootnote.LFootnotePanel.LPanelBinEnabled;

    public bool LShelfPortraitAllowed => LShelfFootnote.LFootnotePanel.LPanelPressAllowed;

    public bool LShelfEmpty => _lShelfCount == 0;

    public bool LShelfSieveActive => _lShelfVista?.LVistaFiltered ?? false;

    public void LShelfVistaRestore(LVista vista, LVista footnote)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(footnote);

        _lShelfVista = vista;
        LShelfPanel.LPanelVistaRestore(vista);
        LShelfFootnote.LFootnoteVistaRestore(vista, footnote);
        LShelfImprint.LImprintVistaRestore(vista);
    }

    public IReadOnlyList<LCatalogReference> LShelfRowsRead()
    {
        return LShelfRowsApply(_lShelfVista is LVista vista ? _lEntryPort.LEngineReferenceFind(vista) : []);
    }

    private IReadOnlyList<LCatalogReference> LShelfRowsApply(IReadOnlyList<LCatalogReference> rows)
    {
        _lShelfCount = rows.Count;
        if (LShelfSourceShown)
        {
            if (!LShelfChosenCheck(rows))
            {
                LShelfPanel.LPanelClear();
            }
        }

        return rows;
    }

    private static bool LShelfChosenCheck(IReadOnlyList<LCatalogReference> rows)
    {
        foreach (LCatalogReference row in rows)
        {
            if (row.LCatalogReferenceChosen)
            {
                return true;
            }
        }

        return false;
    }

    public void LShelfQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lShelfVista?.LVistaQuerySet(query);
    }

    public void LShelfOrderSet(LCatalogOrder? order)
    {
        if (_lShelfVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LShelfSieveSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lShelfVista?.LVistaFilterSet(filter);
    }

    public string LShelfTallyRead()
    {
        return LShelfTallyRead(_lShelfVista?.LVistaChosen);
    }

    public string LShelfTallyRead(long? id)
    {
        return LShelfTallyFormat(LShelfUsageRead(id));
    }

    private int LShelfUsageRead(long? id)
    {
        return id is long stored ? _lEntryPort.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored) : 0;
    }

    private string LShelfTallyFormat(int count)
    {
        return LReference.LReferenceUsageFormat(count, _lSettingsPort.LEngineTextRead);
    }

    public LColophon LShelfColophonRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LReference reference = draft.LDraftReference
            ?? throw new InvalidOperationException("The shelf draft holds no reference.");
        return reference.LReferenceColophonRead(
            draft.LDraftAuthor, LShelfTallyRead(), _lSettingsPort.LEngineTextRead);
    }

    public bool LShelfChangeCheck()
    {
        return LShelfPanel.LPanelChangeCheck() || LShelfFootnote.LFootnotePanel.LPanelChangeCheck();
    }

    private bool LImprintChangeCheck()
    {
        return LShelfImprint.LImprintDesk.LDeskChangeCheck();
    }

    private void LShelfStateUpdate()
    {
        LShelfChanged?.Invoke();
    }

    private void LShelfImprintOpen(long id)
    {
        LShelfImprint.LImprintOpen(id);
    }

    public bool LShelfLeaveConfirm()
    {
        if (!LShelfChangeCheck())
        {
            return true;
        }

        return _lShelfLeaveSeam();
    }

    private bool LShelfDeleteConfirm()
    {
        return _lShelfRemovalSeam(LShelfUsageRead(_lShelfVista?.LVistaChosen));
    }

    public void LShelfClear()
    {
        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelClear();
    }

    public void LShelfRowSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LShelfLeaveConfirm())
        {
            return;
        }

        LShelfSourceOpen(id, LShelfEditing);
    }

    private void LShelfSourceOpen(long? id, bool editing)
    {
        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelScribeShow(editing);
        LShelfPanel.LPanelRowShow(id);
    }

    public void LShelfStoredShow(long id)
    {
        LShelfSourceOpen(id, false);
    }

    private void LShelfSourceShow()
    {
        LShelfPanel.LPanelRowShow(_lShelfVista?.LVistaChosen);
    }

    private void LShelfSourceHide()
    {
        LShelfPanel.LPanelScribeShow(false);
        LShelfImprint.LImprintCancel();
    }

    public void LShelfEntrySelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LShelfLeaveConfirm())
        {
            return;
        }

        LShelfEntryOpen(id, LShelfEditing);
    }

    private void LShelfEntryOpen(long? id, bool editing)
    {
        LShelfSourceHide();
        LShelfFootnote.LFootnotePanel.LPanelScribeShow(editing);
        LShelfFootnote.LFootnotePanel.LPanelRowShow(id);
    }

    public void LShelfEntryUpdate()
    {
        LShelfFootnote.LFootnotePanel.LPanelDraftUpdate();
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void LShelfFreshStart()
    {
        if (!LShelfLeaveConfirm())
        {
            return;
        }

        if (LShelfRowChosen)
        {
            LShelfSourceHide();
            LShelfFootnote.LFootnoteEntryCreate();
            return;
        }

        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelFreshOpen();
        LShelfImprint.LImprintOpen(null);
    }

    public void LShelfScribeSet(bool editing)
    {
        if (LShelfEntrySide)
        {
            LShelfFootnoteSet(editing);
            return;
        }

        LShelfPanel.LPanelScribeSet(editing);
        if (!LShelfPanel.LPanelEditing)
        {
            LShelfImprint.LImprintCancel();
        }
    }

    private void LShelfFootnoteSet(bool editing)
    {
        LShelfFootnote.LFootnotePanel.LPanelScribeSet(editing);
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void LShelfScribeRestore(bool editing)
    {
        LShelfPanel.LPanelScribeRestore(editing);
    }

    public void LShelfDelete()
    {
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfPanel.LPanelDelete();
    }

    public (bool LShelfBackward, bool LShelfForward) LShelfChronicleRead()
    {
        return LShelfEntrySide
            ? LShelfEditor.LEditorDesk.LDeskChronicleRead()
            : LShelfImprint.LImprintDesk.LDeskChronicleRead();
    }

    public bool LShelfDraftFinish(bool store)
    {
        return LShelfEntrySide ? LShelfEditor.LEditorFinish(store) : LShelfImprint.LImprintDesk.LDeskFinish(store);
    }

    public void LShelfStoreRun()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorSave();
            return;
        }

        LShelfImprint.LImprintSave();
    }

    public void LShelfUndo()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorDesk.LDeskUndo();
            return;
        }

        LShelfImprint.LImprintDesk.LDeskUndo();
    }

    public void LShelfRedo()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorDesk.LDeskRedo();
            return;
        }

        LShelfImprint.LImprintDesk.LDeskRedo();
    }

    public Task LShelfPortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)
    {
        if (LShelfFootnote.LFootnotePanel.LPanelPressAllowed)
        {
            return LShelfFootnote.LFootnotePortraitPrint(label, ticket);
        }

        if (LShelfSourcePrintable)
        {
            return _lPortraitPort.LEnginePortraitPrint(_lShelfVista, legend, ticket);
        }

        return Task.CompletedTask;
    }

    public void LShelfVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LShelfVistaRestore(
            window.LWindowVistaStart("reference", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("footnote", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public string LShelfFileRead()
    {
        return LVista.LVistaFileRead(LShelfFootnote.LFootnotePanel.LPanelVista);
    }

    public Task LShelfPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(LShelfFootnote.LFootnotePanel.LPanelVista, path, format, label);
    }
}
